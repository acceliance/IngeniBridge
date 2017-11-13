using CommandLine;
using IngeniBridge.Core.Diags;
using IngeniBridge.Core.Iterator;
using IngeniBridge.Core.MetaHelper;
using IngeniBridge.Core.StagingData;
using log4net;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace IngeniBridge.IBDatabaseParser
{
    class Program
    {
        private static readonly ILog log = LogManager.GetLogger ( System.Reflection.MethodBase.GetCurrentMethod ().DeclaringType );
        static int Main ( string [] args )
        {
            int ret = 0;
            log4net.Config.XmlConfigurator.Configure ();
            ParserResult<CommandLineOptions> result = CommandLine.Parser.Default.ParseArguments<CommandLineOptions> ( args );
            if ( result.Errors.Any () )
            {
                ret = 1;
                return ( ret );
            }
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo ( Assembly.GetEntryAssembly ().Location );
            log.Info ( fvi.ProductName + " v" + fvi.FileVersion + " -- " + fvi.LegalCopyright );
            log.Info ( "INgeniBridgeDBFile => " + result.Value.INgeniBridgeDBFile );
            try
            {
                Core.StagingData.Serializer ser = new Core.StagingData.Serializer ();
                Console.WriteLine ( "Deserialzing IB database => " + result.Value.INgeniBridgeDBFile );
                DataModelVersion dmv = ser.DeserializeTree ( result.Value.INgeniBridgeDBFile );
                MetaHelper helper = new MetaHelper ( ser.DataModelAssembly );
                log.Info ( "DataModel Name => " + dmv.Name );
                log.Info ( "DataModel Date => " + dmv.Generated.ToString () );
                log.Info ( "DataModel Version Major => " + dmv.Major.ToString () );
                log.Info ( "DataModel Version Minor => " + dmv.Minor.ToString () );
                log.Info ( "DataModel Version Build => " + dmv.Build.ToString () );
                TreeChecker tc = new TreeChecker ( helper );
                Console.WriteLine ( "Verifying the tree..." );
                tc.CheckTree ( ser.Frame, false, message => log.Error ( message ) );
                new Core.Iterator.NodesHelper ( helper ).IterateTree ( ser.Frame.TreeRoot, ( inode ) =>
                {
                    Console.WriteLine ( "Tree pos => " + inode.FlatPath );
                    Console.WriteLine ( "\tObject => " + inode.vemdNode.EntityType.Name + " - " + helper.RetrieveCodeValue ( inode.Node ) + " - " + helper.RetrieveLabelValue ( inode.Node ) );
                    helper.RetrieveCodeValue ( inode.Node );
                    EntityMetaDescription mdv = helper.GetMetaDataFromType ( inode.GetType () );
                    new MetaHelper ( ser.DataModelAssembly ).ParseAttributes ( inode.Node, ( attribute, val ) =>
                    {
                        Console.WriteLine ( "\t\tAttribute => " + attribute.AttributeType.Name + " - " + val.ToString () );
                        return ( true );
                    }, true, true );
                    return ( true ); 
                } );
                log.Info ( "Terminated OK." );
            }
            catch ( Exception ex )
            {
                log.Error ( ex );
                Console.WriteLine ( ex.Message );
                log.Error ( "Terminated FAILED." );
                ret = 1;
            }
            return ( ret );
        }
    }
}
