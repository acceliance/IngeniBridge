using IngeniBridge.Core.MetaHelper.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyCompanyDataModel
{
    public abstract class IOT : MyCompanyAsset
    {
        [PropagatePropertyOnChildrenNodes]
        public string TelephoneNumber { get; set; }
    }
    public class PressureSensor : IOT
    {
    }
    public class MultiFunctionSensor : IOT
    {
    }

}
