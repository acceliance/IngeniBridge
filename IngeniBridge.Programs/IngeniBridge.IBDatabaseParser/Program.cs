using CommandLine;
using IngeniBridge.Core.Diags;
using IngeniBridge.Core.MetaHelper;
using IngeniBridge.Core.StagingData;
using IngeniBridge.Core.Storage;
using log4net;
using log4net.Config;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
            int exitCode = 0;
            XmlConfigurator.Configure ( LogManager.GetRepository ( Assembly.GetEntryAssembly () ), new FileInfo ( "log4net.config" ) );
            CommandLineOptions options = null;
            ParserResult<CommandLineOptions> result = CommandLine.Parser.Default.ParseArguments<CommandLineOptions> ( args );
            result.WithNotParsed<CommandLineOptions> ( ( errs ) =>
            {
                errs.All ( err => { log.FatalFormat ( err.ToString () ); return ( true ); } );
                exitCode = 1;
            } );
            if ( exitCode != 0 ) return ( exitCode );
            result.WithParsed<CommandLineOptions> ( opts => { options = opts; } );
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo ( Assembly.GetEntryAssembly ().Location );
            Console.WriteLine ( fvi.ProductName + " v" + fvi.FileVersion + "\n" + fvi.LegalCopyright );
            log.Info ( "Starting " + Assembly.GetEntryAssembly ().GetName ().Name + " v" + Assembly.GetEntryAssembly ().GetName ().Version );
            log.Info ( "StorageAccessorAssembly => " + options.StorageAccessorAssembly );
            log.Info ( "IBDatabase => " + options.IBDatabase );
            try
            {
                #region init IngeniBridge
                UriBuilder uri = new UriBuilder ( Assembly.GetExecutingAssembly ().CodeBase );
                string path = Path.GetDirectoryName ( Uri.UnescapeDataString ( uri.Path ) );
                Assembly accessorasm = Assembly.LoadFile ( path + "\\" + options.StorageAccessorAssembly );
                Core.Storage.StorageAccessor accessor = Core.Storage.StorageAccessor.InstantiateFromAccessorAssembly ( accessorasm );
                AssetExtension.StorageAccessor = accessor;
                TimedDataExtension.StorageAccessor = accessor;
                accessor.OpenDB ( options.IBDatabase );
                #endregion
                log.Info ( "DataModel Name => " + accessor.Version.Name );
                log.Info ( "DataModel Date => " + accessor.Version.Generated.ToString () );
                log.Info ( "DataModel Version Majour => " + accessor.Version.Major.ToString () );
                log.Info ( "DataModel Version Minor => " + accessor.Version.Minor.ToString () );
                log.Info ( "DataModel Version Build => " + accessor.Version.Build.ToString () );
                TreeChecker tc = new TreeChecker ( accessor );
                Console.WriteLine ( "Vérification de l'arbre..." );
                tc.CheckTree ( true, message => log.Error ( message ) );
                accessor.IterateTree ( accessor.RootAsset.Entity, ( inode ) =>
                {
                    Console.WriteLine ( "Tree pos => " + inode.NodePath );
                    Console.WriteLine ( "Parent attribute containing node => " + inode.AttributeInParent );
                    Console.WriteLine ( "\tObject => " + inode.Entity.GetType () .Name + " - " + accessor.ContentHelper.RetrieveCodeValue ( inode.Entity ) + " - " + accessor.ContentHelper.RetrieveLabelValue ( inode.Entity ) );
                    accessor.ContentHelper.ParseAttributes ( inode.Entity, ( attribute, val ) =>
                    {
                        Console.WriteLine ( "\t\tAttribute => " + attribute.AttributeName + " (" + attribute.AttributeType.Name + ") = " + val.ToString () );
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
                exitCode = 1;
            }
            return ( exitCode );
        }
    }
}
