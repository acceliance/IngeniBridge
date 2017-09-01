using IngeniBridge.Core.Diags;
using IngeniBridge.Core.Iterator;
using IngeniBridge.Core.MetaHelper;
using IngeniBridge.BuildUtils.Arrays;
using IngeniBridge.Core.StagingData;
using IngeniBridge.BuildUtils;
using MyCompanyDataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using log4net;
using System.Diagnostics;

namespace IngeniBridge.Samples.MyCompany
{
    class Program
    {
        public static MetaHelper metahelper;
        public static NodesHelper nodeshelper;
        private static readonly ILog log = LogManager.GetLogger ( System.Reflection.MethodBase.GetCurrentMethod ().DeclaringType );
        static int Main ( string [] args )
        {
            log4net.Config.XmlConfigurator.Configure ();
            int ret = 0;
            try
            {
                FileVersionInfo fvi = FileVersionInfo.GetVersionInfo ( Assembly.GetEntryAssembly ().Location );
                log.Info ( fvi.ProductName + " v" + fvi.FileVersion + " -- " + fvi.LegalCopyright );
                log.Info ( "Starting => " + Assembly.GetEntryAssembly ().GetName ().Name + " v" + Assembly.GetEntryAssembly ().GetName ().Version );
                log.Info ( "Data Model => " + Assembly.GetAssembly ( typeof ( ProductionSite ) ).GetName ().Name + " v" + Assembly.GetAssembly ( typeof ( ProductionSite ) ) .GetName ().Version );
                TypeOfMeasure [] tom = new TypeOfMeasure [ 0 ];
                Type t = tom.GetType ();
                #region init IngeniBridge
                metahelper = new MetaHelper ( Assembly.GetAssembly ( typeof ( TypeOfMeasure ) ) );
                ArraysHelper.SetMetaHelper ( metahelper );
                nodeshelper = new NodesHelper ( metahelper );
                #endregion
                #region Init root asset
                MyCompanyRootAsset root = new MyCompanyRootAsset () { Code = "Root Asset", Label = "The root of my company's assests and measures" };
                DataModelFrame frame = new DataModelFrame () { TreeRoot = root };
                #endregion
                #region nomenclatures
                Dictionary<string, TypeOfMeasure> measures = new Dictionary<string, TypeOfMeasure> ();
                measures.Add ( "TMP", new TypeOfMeasure () { Code = "TMP", Label = "Temperature ", Unit = "°C" } );
                measures.Add ( "PRESS", new TypeOfMeasure () { Code = "PRESS", Label = "Pressure", Unit = "bar" } );
                measures.Add ( "ELEC", new TypeOfMeasure () { Code = "ELEC", Label = "Electricity", Unit = "kw/h" } );
                measures.Add ( "WT", new TypeOfMeasure () { Code = "WT", Label = "Water throuput", Unit = "m3/h" } );
                measures.Add ( "CL", new TypeOfMeasure () { Code = "CL", Label = "Clorine", Unit = "mg/l" } );
                Dictionary<string, Sector> sectors = new Dictionary<string, Sector> ();
                sectors.Add ( "W", new Sector () { Code = "W", Label = "West" } );
                sectors.Add ( "S", new Sector () { Code = "S", Label = "South" } );
                Dictionary<string, Organization> orgas = new Dictionary<string, Organization> ();
                orgas.Add ( "O1", new Organization () { Code = "O1", Label = "Organization 1" } );
                orgas.Add ( "O2", new Organization () { Code = "O2", Label = "Organization 2" } );
                // add all nomenclatures at one time
                frame.Nomenclatures = new Core.Nomenclature [] [] { measures.Values.ToArray (), sectors.Values.ToArray (), orgas.Values.ToArray () };
                #endregion
                #region assets
                ProductionSite siteParis = new ProductionSite () { Code = "Site of Paris", Label = "Site of Paris, production of water", Location = "Paris", Sector = sectors [ "W" ], Organization = orgas [ "O1" ] };
                root.AddElementToArray ( siteParis );
                ProductionSite siteLivry = new ProductionSite () { Code = "Site of Livry", Label = "Site of Livry-Gargan, quality of water", Location = "Livry-Gargan", Sector = sectors [ "S" ], Organization = orgas [ "O2" ] };
                root.AddElementToArray ( siteLivry );
                GroupOfPumps grouppumps = new GroupOfPumps () { Code = "GP 001" };
                siteParis.AddElementToArray ( grouppumps );
                PressureSensor iot1 = new PressureSensor () { Code = "PS 001", TelephoneNumber = "0123456789" };
                grouppumps.AddElementToArray ( iot1 );
                WaterPump wp1 = new WaterPump () { Code = "WP 001" };
                grouppumps.AddElementToArray ( wp1 );
                WaterPump wp2 = new WaterPump () { Code = "WP 002" };
                grouppumps.AddElementToArray ( wp2 );
                ClorineInjector cl1 = new ClorineInjector () { Code = "CI 001" };
                siteLivry.AddElementToArray (  cl1 );
                MultiFunctionSensor iot2 = new MultiFunctionSensor () { Code = "MFS 001", TelephoneNumber = "1234567890" };
                cl1.AddElementToArray ( iot2 );
                #endregion
                #region measures
                AcquiredMeasure am1 = new AcquiredMeasure () { Code = "M 001", ScadaExternalReference = "EXTREF 001", TypeOfMeasure = measures [ "WT" ], ConsolidationType = ConsolidationType.None };
                grouppumps.AddElementToArray ( am1 );
                AcquiredMeasure am2 = new AcquiredMeasure () { Code = "M 002", ScadaExternalReference = "EXTREF 002", TypeOfMeasure = measures [ "ELEC" ], ConsolidationType = ConsolidationType.None };
                wp1.AddElementToArray ( am2 );
                AcquiredMeasure am3 = new AcquiredMeasure () { Code = "M 003", ScadaExternalReference = "EXTREF 003", TypeOfMeasure = measures [ "ELEC" ], ConsolidationType = ConsolidationType.None };
                wp2.AddElementToArray ( am3 );
                AcquiredMeasure am4 = new AcquiredMeasure () { Code = "M 004", ScadaExternalReference = "EXTREF 004", TypeOfMeasure = measures [ "PRESS" ], ConsolidationType = ConsolidationType.None };
                iot1.AddElementToArray ( am4 );
                ComputedMeasure am5 = new ComputedMeasure () { Code = "M 005", ScadaExternalReference = "EXTREF 005", TypeOfMeasure = measures [ "ELEC" ], ConsolidationType = ConsolidationType.Average };
                siteParis.AddElementToArray ( am5 );
                AcquiredMeasure am6 = new AcquiredMeasure () { Code = "M 006", ScadaExternalReference = "EXTREF 006", TypeOfMeasure = measures [ "CL" ], ConsolidationType = ConsolidationType.None };
                iot2.AddElementToArray ( am6 );
                AcquiredMeasure am7 = new AcquiredMeasure () { Code = "M 007", ScadaExternalReference = "EXTREF 007", TypeOfMeasure = measures [ "PRESS" ], ConsolidationType = ConsolidationType.None };
                cl1.AddElementToArray ( am7 );
                AcquiredMeasure am8 = new AcquiredMeasure () { Code = "M 008", ScadaExternalReference = "EXTREF 008", TypeOfMeasure = measures [ "ELEC" ], ConsolidationType = ConsolidationType.None };
                cl1.AddElementToArray ( am8 );
                AcquiredMeasure am9 = new AcquiredMeasure () { Code = "M 009", ScadaExternalReference = "EXTREF 010", TypeOfMeasure = measures [ "CL" ], ConsolidationType = ConsolidationType.None };
                cl1.AddElementToArray ( am9 );
                AcquiredMeasure am10 = new AcquiredMeasure () { Code = "M 010", ScadaExternalReference = "EXTREF 009", TypeOfMeasure = measures [ "ELEC" ], ConsolidationType = ConsolidationType.None };
                //ArraysHelper.AddElementToArray ( cl1, am10 );
                #endregion
                #region check and generation (generic script)
                TreeChecker tc = new TreeChecker ( metahelper );
                tc.CheckTree ( frame, message =>
                {
                    Console.WriteLine ( "error => " + message );
                } );
                Serializer ser = new Serializer () { DataModelAssembly = Assembly.GetAssembly ( typeof ( MyCompanyRootAsset ) ) };
                ser.Frame = frame;
                string fimastername = FileDater.SetFileNameDateTime ( "..\\..\\MasterAssetMyCompany.ibdb" );
                ser.SerializeTree ( fimastername );
                #endregion
            }
            catch ( Exception e )
            {
                log.Error ( "Exception => " + e.GetType () + " = " + e.Message );
                ret = 1;
            }
            log.Info ( "Terminated => " + ret.ToString () );
            return ( ret );
        }
    }
}
