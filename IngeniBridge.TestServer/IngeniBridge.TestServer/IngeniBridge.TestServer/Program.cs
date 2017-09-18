using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using System.Diagnostics;
using System.Reflection;
using System.Net.Http;
using System.Net;
using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using IngeniBridge.Core.MetaHelper;
using System.Runtime.Serialization;
using IngeniBridge.Core.Service;
using System.Runtime.Serialization.Formatters;
using IngeniBridge.Core.Mining;
using IngeniBridge.Core.StagingData;

namespace IngeniBridge.TestServer
{
    class Program
    {
        private static readonly ILog log = LogManager.GetLogger ( System.Reflection.MethodBase.GetCurrentMethod ().DeclaringType );
        static string url = "http://www.deagital.com:8091/IngeniBridge/PrivateDemo/Deagital";
        static int Main ( string [] args )
        {
            log4net.Config.XmlConfigurator.Configure ();
            int ret = 0;
            try
            {
                FileVersionInfo fvi = FileVersionInfo.GetVersionInfo ( Assembly.GetEntryAssembly ().Location );
                log.Info ( fvi.ProductName + " v" + fvi.FileVersion + " -- " + fvi.LegalCopyright );
                log.Info ( "Starting " + Assembly.GetEntryAssembly ().GetName ().Name + " v" + Assembly.GetEntryAssembly ().GetName ().Version );
                Console.Write ( "Login => " );
                string login = Console.ReadLine ();
                Console.Write ( "Password => " );
                string password = "";
                while ( true )
                {
                    var key = Console.ReadKey ( true );
                    if ( key.Key == ConsoleKey.Enter ) break;
                    password += key.KeyChar;
                }
                HttpClientHandler handler = new HttpClientHandler () { UseDefaultCredentials = true };
                HttpClient client = new HttpClient ();
                client.BaseAddress = new Uri (url );
                client.DefaultRequestHeaders.Accept.Add ( new MediaTypeWithQualityHeaderValue ( "application/json" ) );
                var byteArray = Encoding.ASCII.GetBytes ( login + ":" + password );
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue ( "Basic", Convert.ToBase64String ( byteArray ) );
                log.Info ( "Connecting => " + url );
                Task<HttpResponseMessage> response = client.GetAsync ( url + "/REST/RetrieveDatas?PageNumber=0&PageSize=2&CallingApplication=IngeniBridge.TestServer" ); // here find all datas
                string buf = response.Result.Content.ReadAsStringAsync ().Result;
                MethodREST ( client, buf );
                MethodMapping ( client, buf );
                // here find data from Historian reference EXTREF 004, the acquisistion platform detected an exceeding threshold, now we must correlate this alarm with an existing alarm
                response = client.GetAsync ( url + "/REST/RetrieveDatas?CorrelationCriteria=TimedData.ScadaExternalReference=EXTREF 004&PageNumber=0&PageSize=10&CallingApplication=IngeniBridge.TestServer" ); 
                buf = response.Result.Content.ReadAsStringAsync ().Result;
                CorrelationInfluenceZoneBusinessUseCase ( client, buf );
            }
            catch ( Exception e )
            {
                log.Error ( "Exception => " + e.GetType () + " = " + e.Message );
                ret = 1;
            }
            log.Info ( "Terminated => " + ret.ToString () );
            return ( ret );
        }
        static void MethodREST ( HttpClient client, string buf )
        {
            log.Info ( "MethodREST ==============================" );
            JArray jsonRoot = ( JArray ) JsonConvert.DeserializeObject ( buf );
            if ( jsonRoot.Children ().Count () == 0 ) Console.WriteLine ( "last page reached" );
            jsonRoot.Children ().All ( contexteddata =>
            {
                StringBuilder path = new StringBuilder ();
                JArray parents = ( JArray ) ( contexteddata [ "Parents" ] [ "$values" ] );
                parents.Children ().Reverse ().All ( hierarchie =>
                {
                    string code = ( string ) hierarchie [ "Code" ];
                    path.Append ( code + "\\" );
                    return ( true );
                } );
                JToken data = ( JToken ) contexteddata [ "Data" ];
                string type = contexteddata [ "Data" ] [ "$type" ].ToString ();
                NodeInfo ni = NodeInfo.RetrieveNodeInfo ( data, path, type );
                Console.Write ( "Path => " + ni.path + "\n" );
                Console.Write ( "Type => " + ni.type + "\n" );
                Console.Write ( "Code => " + ni.attributes [ "Code" ] + "\n" );
                ni.attributes.All ( attr => { Console.WriteLine ( "\t" + attr.Key + " => " + attr.Value + "\n" ); return ( true ); } );
                return ( true );
            } );
        }
        static void MethodMapping ( HttpClient client, string buf )
        {
            log.Info ( "MethodMapping ==============================" );
            Task<HttpResponseMessage> response = client.GetAsync ( url + "/DataModel" );
            byte [] buffer = response.Result.Content.ReadAsByteArrayAsync ().Result;
            Assembly DataModelAssembly = Serializer.RebuildDataModel ( buffer );
            MetaHelper helper = new MetaHelper ( DataModelAssembly );
            ContextedData [] cds = ContextedAssetSerializer.DeserializeContextedDatasFromString ( buf );
            cds.All ( cd =>
            {
                Console.Write ( "Path => " );
                cd.Parents.Reverse ().All ( parent => { Console.Write ( parent.Code + "\\" ); return ( true ); } );
                Console.WriteLine ();
                Console.Write ( "Type => " + cd.Data.GetType () .FullName + "\n" );
                Console.Write ( "Code => " + cd.Data.Code + "\n" );
                Console.WriteLine ();
                EntityMetaDescription emd = helper.GetMetaDataFromType ( cd.Data.GetType () );
                helper.ParseAttributes ( cd.Data, ( attribute, val ) =>
                {
                    Console.WriteLine ( "\t" + attribute + " (type=" + val.GetType () .Name + ") => " + val.ToString () + "\n" );
                    return ( true );
                }, true, true, true );
                object [] vals = helper.RetrieveValuesFromType ( cd.Data, "TypeOfMeasure" );
                if ( vals?.Count () > 0 ) Console.WriteLine ( "Found type TypeOfMeasure in object, value is => " + helper.RetrieveCodeValue ( vals [ 0 ] ) );
                return ( true );
            } );
        }
        static void CorrelationInfluenceZoneBusinessUseCase ( HttpClient client, string buf )
        {
            log.Info ( "CorrelationInfluenceZoneBusinessUseCase ==============================" );
            Task<HttpResponseMessage> response = client.GetAsync ( url + "/DataModel" );
            byte [] buffer = response.Result.Content.ReadAsByteArrayAsync ().Result;
            Assembly DataModelAssembly = Serializer.RebuildDataModel ( buffer );
            MetaHelper helper = new MetaHelper ( DataModelAssembly );
            ContextedData cd = ContextedAssetSerializer.DeserializeContextedDatasFromString ( buf ) [ 0 ]; // here get the first and unique data returned by the request : TimedData.ScadaExternalReference=EXTREF 004
            //
            // the business use case states that:
            // - correlating two alarms should be made on the influence zone (the same influence zone for 2 or more alarms)
            // - the influence zone is set on an equipement asset but that could change as the metamodel gets more accurate
            // - the influence zone is set on a parent of the data but the exact position is variable is a data come from an iot or from from an equipement or any other case
            //
            // conclusion:
            // - reading the use case specification, to identify the influence zone for correlation it needs to make use of discovery features of IngeniBridge
            // 
            // now see code below find influence zone for correlation
            //
            // IngeniBridge is fully metadata driven platform
            string influencezonecode = "";
            string path = "";
            cd.Parents.Reverse ().All ( parent => 
            {
                path += parent.Code + "\\";
                response = client.GetAsync ( url + "/REST/RetrieveAsset?PathInTree=" + path + "&CallingApplication=IngeniBridge.TestServer" );
                buf = response.Result.Content.ReadAsStringAsync ().Result;
                ContextedAsset ca = ContextedAssetSerializer.DeserializeContextedAssetsFromString ( buf ) [ 0 ];
                object [] vals = helper.RetrieveValuesFromType ( ca.Asset, "InfluenceZone" );
                if ( vals?.Count () > 0 ) influencezonecode = helper.RetrieveCodeValue ( vals [ 0 ] );
                return ( influencezonecode.Length == 0 );
            } );
            Console.WriteLine ( "Influence Zone found = " + influencezonecode );
        }
    }
}
