namespace MyCompanyDataModel
{
    public abstract class Equipment : MyCompanyAsset
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
    public class WaterSwitcher : Equipment
    {
    }
    public class ClorineInjector : Equipment
    {
    }
}
