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

namespace IngeniBridge.Server.TestServer
{
    public class MethodMapping
    {
        public static void Launch ( HttpClient client, string buf )
        {
            Program.log.Info ( "MethodMapping ==============================" );
            Task<HttpResponseMessage> response = client.GetAsync ( Program.url + "/DataModel" );
            byte [ ] buffer = response.Result.Content.ReadAsByteArrayAsync ().Result;
            Assembly DataModelAssembly = Core.Storage.StorageAccessor.RebuildDataModel ( buffer );
            MetaHelper helper = new MetaHelper ( DataModelAssembly );
            EntityContentHelper contenthelper = new EntityContentHelper ( helper );
            ContextedTimeSeries [] cds = ContextedAssetSerializer.DeserializeContextedTimeSeriessFromString ( buf );
            cds.All ( cd =>
            {
                Console.Write ( "Path => " );
                cd.Parents.Reverse ().All ( parent => { Console.Write ( parent.Code + "\\" ); return ( true ); } );
                Console.WriteLine ();
                Console.Write ( "Type => " + cd.TimeSeries.GetType ().FullName + "\n" );
                Console.Write ( "Code => " + cd.TimeSeries.Code + "\n" );
                Console.WriteLine ();
                EntityMetaDescription emd = helper.GetMetaDataFromType ( cd.TimeSeries.GetType () );
                contenthelper.ParseAttributes ( cd.TimeSeries, ( attribute, val ) =>
                {
                    Console.WriteLine ( "\t" + attribute + " (type=" + val.GetType ().Name + ") => " + val.ToString () + "\n" );
                    return ( true );
                }, true, true, true );
                object [ ] vals = contenthelper.RetrieveValuesFromType ( cd.TimeSeries, "TypeOfMeasure" );
                if ( vals?.Count () > 0 ) Console.WriteLine ( "Found type TypeOfMeasure in object, value is => " + contenthelper.RetrieveCodeValue ( vals [ 0 ] ) );
                return ( true );
            } );
        }
    }
}
