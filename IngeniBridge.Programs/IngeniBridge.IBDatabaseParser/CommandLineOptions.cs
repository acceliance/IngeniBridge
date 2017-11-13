using CommandLine;
namespace IngeniBridge.IBDatabaseParser
{
    internal class CommandLineOptions
    {
        [Option ( 'm', "IngeniBridgeDBFile", Required = true, HelpText = "IngeniBridge DB file")]
        public string INgeniBridgeDBFile { get; set; }
    }
}
