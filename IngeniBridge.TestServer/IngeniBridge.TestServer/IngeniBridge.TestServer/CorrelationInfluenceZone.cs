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
    public class CorrelationInfluenceZone
    {
        public static void Launch ( HttpClient client, string buf )
        {
            Program.log.Info ( "CorrelationInfluenceZoneBusinessUseCase ==============================" );
            Task<HttpResponseMessage> response = client.GetAsync ( Program.url + "/DataModel" );
            byte [ ] buffer = response.Result.Content.ReadAsByteArrayAsync ().Result;
            Assembly DataModelAssembly = IngeniBridge.Core.Storage.StorageAccessor.RebuildDataModel ( buffer );
            MetaHelper helper = new MetaHelper ( DataModelAssembly );
            EntityContentHelper contenthelper = new EntityContentHelper ( helper );
            ContextedData cd = ContextedAssetSerializer.DeserializeContextedDatasFromString ( buf ) [ 0 ]; // here get the first and unique data returned by the request : TimedData.TimedDataExternalReference=EXTREF 004
            //
            // the business use case states that:
            // - correlating two alarms should be made on the influence zone (the same influence zone for 2 or more alarms)
            // - the influence zone is set on an equipement asset but that could change as the metamodel gets more accurate
            // - the influence zone is set on a parent of the data but the exact position is variable is a data come from an iot or from from an equipement or any other case
            //
            // conclusion:
            // - reading the use case specification, identifying the influence zone for correlation should be made using discovery features of IngeniBridge
            // 
            // now find influence zone for correlation
            string influencezonecode = "";
            string path = "";
            cd.Parents.Reverse ().All ( parent =>
            {
                path += parent.Code + "\\";
                response = client.GetAsync ( Program.url + "/REQUESTER/RetrieveEntityFromPath?PathInTree=" + path + "&CallingApplication=IngeniBridge.TestServer" );
                buf = response.Result.Content.ReadAsStringAsync ().Result;
                ContextedAsset ca = ContextedAssetSerializer.DeserializeContextedAssetsFromString ( buf ) [ 0 ];
                object [ ] vals = contenthelper.RetrieveValuesFromType ( ca.Asset, "InfluenceZone" );
                if ( vals?.Count () > 0 ) influencezonecode = contenthelper.RetrieveCodeValue ( vals [ 0 ] );
                return ( influencezonecode.Length == 0 );
            } );
            Console.WriteLine ( "Influence Zone found = " + influencezonecode );
        }
    }
}
