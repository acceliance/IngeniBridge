using IngeniBridge.Core;
using IngeniBridge.Core.MetaHelper.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCompanyDataModel
{
    public class ProductionSite : MyCompanyAsset
    {
        public Equipment [] Equipments { get; set; }
        public City City { get; set; }
        public Sector Sector { get; set; }
        [ExternalReference]
        public InfluenceZone Zone { get; set; }
    }
}
