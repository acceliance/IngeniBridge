using IngeniBridge.Core;
using IngeniBridge.Core.MetaHelper;
using IngeniBridge.Core.Mining;
using IngeniBridge.Core.Serialization;
using IngeniBridge.Core.Service;
using IngeniBridge.Core.StagingData;
using IngeniBridge.Core.Storage;
using log4net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace IngeniBridge.TestServer
{
    public class MethodRest
    {
        public static void Launch ( HttpClient client )
        {
            Program.log.Info ( "MethodREST ==============================" );
            Task<HttpResponseMessage> response = client.GetAsync ( Program.url + "/REQUESTER/RetrieveTimeSeries?PageNumber=0&PageSize=2&CallingApplication=IngeniBridge.TestServer" ); // here find all datas
            string buf = response.Result.Content.ReadAsStringAsync ().Result;
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
                JToken data = ( JToken ) contexteddata [ "TimeSeries" ];
                string type = contexteddata [ "TimeSeries" ] [ "$type" ].ToString ();
                NodeInfo ni = NodeInfo.RetrieveNodeInfo ( data, path, type );
                Console.Write ( "Path => " + ni.path + "\n" );
                Console.Write ( "Type => " + ni.type + "\n" );
                Console.Write ( "Code => " + ni.attributes [ "Code" ] + "\n" );
                ni.attributes.All ( attr => { Console.WriteLine ( "\t" + attr.Key + " => " + attr.Value + "\n" ); return ( true ); } );
                return ( true );
            } );
        }
    }
}
