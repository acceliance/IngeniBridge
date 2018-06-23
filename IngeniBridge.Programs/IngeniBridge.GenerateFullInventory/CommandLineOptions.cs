using CommandLine;
namespace IngeniBridge.GenerateFullInventory
{
    internal class CommandLineOptions
    {
        [Option ( 's', "StorageAccessorAssembly", Required = true, HelpText = "StorageAccessor Assembly dll" )]
        public string StorageAccessorAssembly { get; set; }
        [Option ( 'i', "IBDatabase", Required = true, HelpText = "IngeniBridge database" )]
        public string IBDatabase { get; set; }
        [Option ( 'i', "InventoryFile", Required = true, HelpText = "Output Excel Inventory File" )]
        public string InventoryFile { get; set; }
    }
}
