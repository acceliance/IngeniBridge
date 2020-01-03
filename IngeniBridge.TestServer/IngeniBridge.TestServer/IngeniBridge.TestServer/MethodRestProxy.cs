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
    public class MethodRestProxy
    {
        public static void Launch ( HttpClient client )
        {
            Program.log.Info ( "MethodRESTProxy ==============================" );
            IngeniBridgeProxy.RequesterClient proxy = new IngeniBridgeProxy.RequesterClient ( client );
            Task< ICollection <IngeniBridgeProxy.ContextedTimeSeries > > response = proxy.RetrieveTimeSeriesAsync ( null, 0, 10, "IngeniBridge.TestServer", null, null );
            ICollection<IngeniBridgeProxy.ContextedTimeSeries> ctss = response.Result;
            foreach (IngeniBridgeProxy.ContextedTimeSeries cts in ctss )
            {
                StringBuilder path = new StringBuilder();
                foreach ( IngeniBridgeProxy.Asset a in cts.Parents )
                {
                    path.Append( a.Code + "\\");
                }
                Console.Write("Path => " + path + "\n");
                Console.Write("Type => " + cts.TimeSeries.GetType () .Name + "\n");
                Console.Write("Code => " + cts.TimeSeries.Code + "\n");
                PropertyInfo[] pis = cts.TimeSeries.GetType().GetProperties();
                pis.All( pi =>
                {
                    Console.WriteLine( "\t" + pi.Name + " => " + pi.GetValue ( cts.TimeSeries, null ) + "\n" );
                    return ( true );
                });

            }
        }
    }
}
