using IngeniBridge.Core.MetaHelper.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCompanyDataModel
{
    public class Equipment : MyCompanyAsset
    {
        public Equipment [] SubEquipments { get; set; }
        public IOT [] IOTs { get; set; }
    }
    public class GroupOfPumps : Equipment
    {
    }
    public class WaterPump : Equipment
    {
    }
    public class ClorineInjector : Equipment
    {
    }
    public class IOT : MyCompanyAsset
    {
        [IndexPropertyOnParents]
        public string TelephoneNumber { get; set; }
    }
    public class PressureSensor : IOT
    {
    }
    public class MultiFunctionSensor : IOT
    {
    }
}
