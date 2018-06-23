using CommandLine;
namespace IngeniBridge.IBDatabaseParser
{
    internal class CommandLineOptions
    {
        [Option ( 's', "StorageAccessorAssembly", Required = true, HelpText = "StorageAccessor Assembly dll" )]
        public string StorageAccessorAssembly { get; set; }
        [Option ( 'i', "IBDatabase", Required = true, HelpText = "IngeniBridge database" )]
        public string IBDatabase { get; set; }
    }
}
