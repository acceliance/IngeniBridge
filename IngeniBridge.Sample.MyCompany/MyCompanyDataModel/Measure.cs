using IngeniBridge.Core;
using IngeniBridge.Core.MetaHelper.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCompanyDataModel
{
    public enum ConsolidationType { None, Average };
    public class MyCompanyData : TimedData
    {
        public ConsolidationType ConsolidationType { get; set; }
        [IndexAlsoByName]
        [IndexEntityAttribute ( "Unit" )]
        public TypeOfMeasure tof { get; set; }
    }
    [DisplayName ( "Acquired Measure" )]
    public class AcquiredMeasure : MyCompanyData
    {
    }
    [DisplayName ( "Computed Data" )]
    public class ComputedMeasure : MyCompanyData
    {
    }
}
