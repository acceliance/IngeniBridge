using CommandLine;
using IngeniBridge.Core;
using IngeniBridge.Core.Iterator;
using IngeniBridge.Core.MetaHelper;
using log4net;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using IngeniBridge.Core.Diags;
using IngeniBridge.Core.StagingData;
using IngeniBridge.Core.Inventory;
using System.Diagnostics;

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
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo ( Assembly.GetEntryAssembly ().Location );
            log.Info ( fvi.ProductName + " v" + fvi.FileVersion + " -- " + fvi.LegalCopyright );
            log.Info ( "INgeniBridgeDBFile => " + result.Value.INgeniBridgeDBFile );
            log.Info ( "InventoryFilee => " + result.Value.InventoryFile );
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
                tc.CheckTree ( ser.Frame, message => log.Error ( message ) );
                FileInfo fi = new FileInfo ( result.Value.InventoryFile );
                if ( fi.Exists ) fi.Delete ();
                ExcelPackage xlMatricesPatrimoines = new ExcelPackage ( fi );
                Dictionary<string, WorksheetInfo> worksheetinfos = new Dictionary<string, WorksheetInfo> ();
                new InventoryHelper ( helper ).Launch ( ser.Frame, ( Name, Headers ) =>
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
