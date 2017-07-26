using IngeniBridge.Core;
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
        public string Location { get; set; }
        public Sector Sector { get; set; }
    }
}
