using IngeniBridge.Core;
using IngeniBridge.Core.MetaHelper.Attributes;
namespace MyCompanyDataModel
{
    [RootAsset]
    public class MyCompanyRootAsset : Asset
    {
        public ProductionSite [] ProductionSites { get; set; }
        public InfluenceZone [] InfluenceZones { get; set; }
    }
}
