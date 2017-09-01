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
                Task<HttpResponseMessage> response = client.GetAsync ( url + "/REST/RetrieveDatas?PageNumber=0&PageSize=2&CallingApplication=IngeniBridge.TestServer" );
                string buf = response.Result.Content.ReadAsStringAsync ().Result;
                MethodREST ( client, buf );
                MethodMapping ( client, buf );
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
            Task<HttpResponseMessage> response = client.GetAsync ( url + "/DotnetAssembly" );
            byte [] datamodelbuffer = response.Result.Content.ReadAsByteArrayAsync ().Result;
            Assembly DataModelAssembly = Assembly.Load ( datamodelbuffer );
            MetaHelper helper = new MetaHelper ( DataModelAssembly );
            ContextedData [] cds = ContextedAssetSerializer.DeserializeContextedDatasFromString ( buf );
            cds.All ( cd =>
            {
                Console.Write ( "Path => " );
                cd.Parents.Reverse ().All ( parent => { Console.Write ( parent.Code + "\\" ); return ( true ); } );
                Console.WriteLine ();
                Console.Write ( "Type => " + cd.Data.GetType () .FullName + "\n" );
                Console.Write ( "Code => " + cd.Data.Code + "\n" );
                EntityMetaDescription emd = helper.GetMetaDataFromType ( cd.Data.GetType () );
                helper.ParseEntityAttributes ( cd.Data, ( attribute, val ) =>
                {
                    Console.WriteLine ( "\t" + attribute + " (type=" + val.GetType () .Name + ") => " + val.ToString () + "\n" );
                    return ( true );
                }, true, true, true );
                return ( true );
            } );
        }
    }
}
