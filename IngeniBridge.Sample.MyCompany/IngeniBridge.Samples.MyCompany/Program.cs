using IngeniBridge.Core.Diags;
using IngeniBridge.Core.MetaHelper;
using IngeniBridge.Core.StagingData;
using IngeniBridge.BuildUtils;
using MyCompanyDataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using IngeniBridge.Core.Util;
using IngeniBridge.Core.Storage;
using System.IO;
using OfficeOpenXml;
using IngeniBridge.Core.Inventory;
using log4net;
using System.Diagnostics;
using log4net.Config;

namespace IngeniBridge.Samples.MyCompany
{
    class Program
    {
        private static readonly ILog log = LogManager.GetLogger ( System.Reflection.MethodBase.GetCurrentMethod ().DeclaringType );
        static int Main ( string [ ] args )
        {
            int ret = 0;
            try
            {
                XmlConfigurator.Configure ( LogManager.GetRepository ( Assembly.GetEntryAssembly () ), new FileInfo ( "log4net.config" ) );
                FileVersionInfo fvi = FileVersionInfo.GetVersionInfo ( Assembly.GetEntryAssembly ().Location );
                log.Info ( fvi.ProductName + " v" + fvi.FileVersion + " -- " + fvi.LegalCopyright );
                log.Info ( "Starting => " + Assembly.GetEntryAssembly ().GetName ().Name + " v" + Assembly.GetEntryAssembly ().GetName ().Version );
                log.Info ( "Data Model => " + Assembly.GetAssembly ( typeof ( ProductionSite ) ).GetName ().Name + " v" + Assembly.GetAssembly ( typeof ( ProductionSite ) ).GetName ().Version );
                #region init IngeniBridge
                UriBuilder uri = new UriBuilder ( Assembly.GetExecutingAssembly ().CodeBase );
                string path = Path.GetDirectoryName ( Uri.UnescapeDataString ( uri.Path ) );
                Assembly accessorasm = Assembly.LoadFile ( path + "\\IngeniBridge.StorageAccessor.InMemory.dll" );
                Core.Storage.StorageAccessor accessor = Core.Storage.StorageAccessor.InstantiateFromAssembly ( accessorasm );
                AssetExtension.StorageAccessor = accessor;
                string fimastername = FileDater.SetFileNameDateTime ( "..\\..\\..\\..\\MasterAssetMyCompany.ibdb" );
                accessor.InitializeNewDB ( Assembly.GetAssembly ( typeof ( MyCompanyAsset ) ), fimastername );
                #endregion
                #region Init root asset
                MyCompanyRootAsset root = new MyCompanyRootAsset () { Code = "Root Asset", Label = "The root of my company's assests and measures" };
                accessor.SetRootAsset ( root );
                #endregion
                #region nomenclatures
                accessor.AddNomenclatureEntry ( new City () { Code = "PAR", Label = "Paris" } );
                accessor.AddNomenclatureEntry ( new City () { Code = "LIV", Label = "Livry-Gargan" } );
                accessor.AddNomenclatureEntry ( new City () { Code = "LER", Label = "Le Raincy" } );
                accessor.AddNomenclatureEntry ( new TypeOfMeasure () { Code = "TMP", Label = "Temperature", Unit = "°C" } );
                accessor.AddNomenclatureEntry ( new TypeOfMeasure () { Code = "PRESS", Label = "Pressure", Unit = "bar" } );
                accessor.AddNomenclatureEntry ( new TypeOfMeasure () { Code = "ELEC", Label = "Electricity", Unit = "kw/h" } );
                accessor.AddNomenclatureEntry ( new TypeOfMeasure () { Code = "WT", Label = "Water throuput", Unit = "m3/h" } );
                accessor.AddNomenclatureEntry ( new TypeOfMeasure () { Code = "CL", Label = "Clorine", Unit = "mg/l" } );
                accessor.AddNomenclatureEntry ( new Sector () { Code = "W", Label = "West" } );
                accessor.AddNomenclatureEntry ( new Sector () { Code = "S", Label = "South" } );
                #endregion
                #region Influence zones
                // Here you see how to access an Excel file using the EPPlus package
                FileInfo fi = new FileInfo ( "..\\..\\..\\Metadata content to consolidate.xlsx" );
                ExcelPackage xlConsolidate = new ExcelPackage ( fi );
                ExcelWorksheet wksZones = xlConsolidate.Workbook.Worksheets [ "Influence Zone" ];
                int line = 2;
                while ( wksZones.Cells [ line, 1 ].Value != null )
                {
                    InfluenceZone iz = new InfluenceZone () { Code = wksZones.Cells [ line, 1 ].Text, Label = wksZones.Cells [ line, 2 ].Text };
                    root.AddChildAsset ( iz );
                    line += 1;
                }
                xlConsolidate.Dispose ();
                #endregion
                #region assets
                ProductionSite siteParis = new ProductionSite () { Code = "Site of Paris", Label = "Site of Paris, production of water", City = accessor.RetrieveNomenclatureEntry<City> ( "PAR" ), Sector = accessor.RetrieveNomenclatureEntry<Sector> ( "W" ) };
                siteParis.Zone = accessor.RetrieveChildEntity<InfluenceZone> ( root, "InfluenceZones", "Z1" );
                root.AddChildAsset ( siteParis );
                ProductionSite siteLivry = new ProductionSite () { Code = "Site of Livry", Label = "Site of Livry-Gargan, quality of water", City = accessor.RetrieveNomenclatureEntry<City> ( "LIV" ), Sector = accessor.RetrieveNomenclatureEntry<Sector> ( "S" ) };
                siteLivry.Zone = accessor.RetrieveChildEntity<InfluenceZone> ( root, "InfluenceZones", "Z2" );
                root.AddChildAsset ( siteLivry );
                ProductionSite siteLeRaincy = new ProductionSite () { Code = "Site of Le Raincy", Label = "Site of Le Raincy, Itercommunication", City = accessor.RetrieveNomenclatureEntry<City> ( "LER" ), Sector = accessor.RetrieveNomenclatureEntry<Sector> ( "S" ) };
                siteLeRaincy.Zone = accessor.RetrieveChildEntity<InfluenceZone> ( root, "InfluenceZones", "Z2" );
                root.AddChildAsset ( siteLeRaincy );
                GroupOfPumps grouppumps = new GroupOfPumps () { Code = "GP 001" };
                siteParis.AddChildAsset ( grouppumps );
                PressureSensor iot1 = new PressureSensor () { Code = "PS 001", TelephoneNumber = "0123456789" };
                grouppumps.AddChildAsset ( iot1 );
                WaterPump wp1 = new WaterPump () { Code = "WP 001" };
                grouppumps.AddChildAsset ( wp1 );
                WaterPump wp2 = new WaterPump () { Code = "WP 002" };
                grouppumps.AddChildAsset ( wp2 );
                ClorineInjector cl1 = new ClorineInjector () { Code = "CI 001" };
                siteLivry.AddChildAsset ( cl1 );
                MultiFunctionSensor iot2 = new MultiFunctionSensor () { Code = "MFS 001", TelephoneNumber = "1234567890" };
                cl1.AddChildAsset ( iot2 );
                WaterSwitcher ws1 = new WaterSwitcher () { Code = "WS 001" };
                siteLeRaincy.AddChildAsset ( ws1 );
                #endregion
                #region measures
                AcquiredMeasure am1 = new AcquiredMeasure () { Code = "M 001", TimeSeriesExternalReference = "EXTREF 001", TypeOfMeasure = accessor.RetrieveNomenclatureEntry<TypeOfMeasure> ( "WT" ), ConsolidationType = ConsolidationType.None };
                grouppumps.AddTimeSeries ( am1 );
                AcquiredMeasure am2 = new AcquiredMeasure () { Code = "M 002", TimeSeriesExternalReference = "EXTREF 002", TypeOfMeasure = accessor.RetrieveNomenclatureEntry<TypeOfMeasure> ( "ELEC" ), ConsolidationType = ConsolidationType.None };
                wp1.AddTimeSeries ( am2 );
                AcquiredMeasure am3 = new AcquiredMeasure () { Code = "M 003", TimeSeriesExternalReference = "EXTREF 003", TypeOfMeasure = accessor.RetrieveNomenclatureEntry<TypeOfMeasure> ( "ELEC" ), ConsolidationType = ConsolidationType.None };
                wp2.AddTimeSeries ( am3 );
                AcquiredMeasure am4 = new AcquiredMeasure () { Code = "M 004", TimeSeriesExternalReference = "EXTREF 004", TypeOfMeasure = accessor.RetrieveNomenclatureEntry<TypeOfMeasure> ( "PRESS" ), ConsolidationType = ConsolidationType.None };
                iot1.AddTimeSeries ( am4 );
                ComputedMeasure am5 = new ComputedMeasure () { Code = "M 005", TimeSeriesExternalReference = "EXTREF 005", TypeOfMeasure = accessor.RetrieveNomenclatureEntry<TypeOfMeasure> ( "ELEC" ), ConsolidationType = ConsolidationType.Average };
                siteParis.AddTimeSeries ( am5 );
                AcquiredMeasure am6 = new AcquiredMeasure () { Code = "M 006", TimeSeriesExternalReference = "EXTREF 006", TypeOfMeasure = accessor.RetrieveNomenclatureEntry<TypeOfMeasure> ( "CL" ), ConsolidationType = ConsolidationType.None };
                iot2.AddTimeSeries ( am6 );
                AcquiredMeasure am7 = new AcquiredMeasure () { Code = "M 007", TimeSeriesExternalReference = "EXTREF 007", TypeOfMeasure = accessor.RetrieveNomenclatureEntry<TypeOfMeasure> ( "PRESS" ), ConsolidationType = ConsolidationType.None };
                cl1.AddTimeSeries ( am7 );
                AcquiredMeasure am8 = new AcquiredMeasure () { Code = "M 008", TimeSeriesExternalReference = "EXTREF 008", TypeOfMeasure = accessor.RetrieveNomenclatureEntry<TypeOfMeasure> ( "ELEC" ), ConsolidationType = ConsolidationType.None };
                cl1.AddTimeSeries ( am8 );
                AcquiredMeasure am9 = new AcquiredMeasure () { Code = "M 009", TimeSeriesExternalReference = "EXTREF 010", TypeOfMeasure = accessor.RetrieveNomenclatureEntry<TypeOfMeasure> ( "CL" ), ConsolidationType = ConsolidationType.None };
                cl1.AddTimeSeries ( am9 );
                AcquiredMeasure am10 = new AcquiredMeasure () { Code = "M 010", TimeSeriesExternalReference = "EXTREF 009", TypeOfMeasure = accessor.RetrieveNomenclatureEntry<TypeOfMeasure> ( "ELEC" ), ConsolidationType = ConsolidationType.None };
                cl1.AddTimeSeries ( am10 );
                AcquiredMeasure am11 = new AcquiredMeasure () { Code = "M 011", TimeSeriesExternalReference = "EXTREF 011", TypeOfMeasure = accessor.RetrieveNomenclatureEntry<TypeOfMeasure> ( "PRESS" ), ConsolidationType = ConsolidationType.None };
                ws1.AddTimeSeries ( am11 );
                #endregion
                #region check and generate IB database (generic script)
                TreeChecker tc = new TreeChecker ( accessor );
                tc.CheckTree ( true, message =>
                {
                    log.Info ( "error => " + message );
                } );
                long nbTotalAssets = 0;
                long nbTotalDatas = 0;
                ( nbTotalAssets, nbTotalDatas ) = accessor.GetStatistics ();
                log.Info ( "nb total assets => " + nbTotalAssets.ToString () );
                log.Info ( "nb total datas => " + nbTotalDatas.ToString () );
                accessor.CloseDB ();
                #endregion
            }
            catch ( Exception e )
            {
                log.Error ( "Exception => " + e.GetType () + " = " + e.Message );
                ret = 1;
            }
            return ( ret );
        }
    }
}
