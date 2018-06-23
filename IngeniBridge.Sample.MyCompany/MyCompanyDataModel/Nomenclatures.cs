using IngeniBridge.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCompanyDataModel
{
    public class TypeOfMeasure : Nomenclature
    {
        public string Unit { get; set; }
    }
    public class Sector : Nomenclature
    {
        public City City { get; set; }
    }
    public class City : Nomenclature
    {

    }
}
