using CommandLine;
namespace IngeniBridge.GenerateFullInventory
{
    internal class CommandLineOptions
    {
        [Option ( 'm', "IngeniBridgeDBFile", Required = true, HelpText = "IngeniBridge DB file")]
        public string INgeniBridgeDBFile { get; set; }
        [Option ( 'i', "InventoryFile", Required = true, HelpText = "Output Excel Inventory File" )]
        public string InventoryFile { get; set; }
    }
}
