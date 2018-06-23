using CommandLine;
using log4net;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using IngeniBridge.Core.Diags;
using IngeniBridge.Core.Inventory;
using IngeniBridge.Core.Storage;

namespace IngeniBridge.GenerateFullInventory
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
            log.Info ( "Starting " + Assembly.GetEntryAssembly ().GetName ().Name + " v" + Assembly.GetEntryAssembly ().GetName ().Version );
            log.Info ( "StorageAccessorAssembly => " + result.Value.StorageAccessorAssembly );
            log.Info ( "IBDatabase => " + result.Value.IBDatabase );
            log.Info ( "InventoryFile => " + result.Value.InventoryFile );
            try
            {
                #region init IngeniBridge
                UriBuilder uri = new UriBuilder ( Assembly.GetExecutingAssembly ().CodeBase );
                string path = Path.GetDirectoryName ( Uri.UnescapeDataString ( uri.Path ) );
                Assembly accessorasm = Assembly.LoadFile ( path + "\\" + result.Value.StorageAccessorAssembly );
                Core.Storage.StorageAccessor accessor = Core.Storage.StorageAccessor.InstantiateFromAccessorAssembly ( accessorasm );
                AssetExtension.StorageAccessor = accessor;
                TimedDataExtension.StorageAccessor = accessor;
                accessor.OpenDB ( result.Value.IBDatabase );
                #endregion
                log.Info ( "DataModel Name => " + accessor.Version.Name );
                log.Info ( "DataModel Date => " + accessor.Version.Generated.ToString () );
                log.Info ( "DataModel Version Majour => " + accessor.Version.Major.ToString () );
                log.Info ( "DataModel Version Minor => " + accessor.Version.Minor.ToString () );
                log.Info ( "DataModel Version Build => " + accessor.Version.Build.ToString () );
                TreeChecker tc = new TreeChecker ( accessor );
                Console.WriteLine ( "Vérification de l'arbre..." );
                tc.CheckTree ( true, message => log.Error ( message ) );
                FileInfo fi = new FileInfo ( result.Value.InventoryFile );
                if ( fi.Exists ) fi.Delete ();
                ExcelPackage xlMatricesPatrimoines = new ExcelPackage ( fi );
                Dictionary<string, WorksheetInfo> worksheetinfos = new Dictionary<string, WorksheetInfo> ();
                new InventoryHelper ( accessor ).Launch ( ( Name, Headers ) =>
                {
                    ExcelWorksheet wk = null;
                    int i = 0;
                    do
                    {
                        string tabname = i == 0 ? Name : i.ToString () + Name;
                        try { wk = xlMatricesPatrimoines.Workbook.Worksheets.Add ( tabname ); }
                        catch ( Exception ) { }
                        i += 1;
                    } while ( wk == null );
                    WorksheetInfo wkinfo = new WorksheetInfo () { wk = wk };
                    int col = 1;
                    Headers.All ( header =>
                    {
                        wkinfo.wk.Cells [ wkinfo.ligne, col ].Value = header;
                        col += 1;
                        return ( true );
                    } );
                    wkinfo.ligne = 2;
                    worksheetinfos.Add ( Name, wkinfo );
                    return ( true );
                },
                ( Name, Columns ) =>
                {
                    WorksheetInfo wkinfo = worksheetinfos [ Name ];
                    int col = 1;
                    Columns.All ( columns =>
                    {
                        wkinfo.wk.Cells [ wkinfo.ligne, col ].Value = columns;
                        col += 1;
                        return ( true );
                    } );
                    wkinfo.ligne += 1; ;
                    return ( true );
                } );
                xlMatricesPatrimoines.Save ();
                log.Info ( "Terminé OK." );
            }
            catch ( Exception ex )
            {
                log.Error ( ex );
                Console.WriteLine ( ex.Message );
                log.Error ( "Terminé FAILED." );
                ret = 1;
            }
            return ( ret );

        }
    }
}
