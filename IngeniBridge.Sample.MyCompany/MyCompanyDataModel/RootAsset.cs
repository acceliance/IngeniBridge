using IngeniBridge.Core;
using IngeniBridge.Core.MetaHelper.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCompanyDataModel
{
    [RootAsset]
    public class MyCompanyRootAsset : Asset
    {
        public InfluenceZone [] InfluenceZones { get; set; }
        public ProductionSite [] ProductionSites { get; set; }
    }
}
