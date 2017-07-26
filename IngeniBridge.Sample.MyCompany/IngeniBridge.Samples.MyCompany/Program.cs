using IngeniBridge.Core.Diags;
using IngeniBridge.Core.Iterator;
using IngeniBridge.Core.MetaHelper;
using IngeniBridge.Core.Arrays;
using IngeniBridge.Core.StagingData;
using IngeniBridge.Utils;
using MyCompanyDataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace IngeniBridge.Samples.MyCompany
{
    class Program
    {
        public static MetaHelper metahelper;
        public static NodesHelper nodeshelper;
        static int Main ( string [] args )
        {
            int ret = 0;
            try
            {
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
                frame.Nomenclatures = new Core.Nomenclature [] [] { measures.Values.ToArray (), sectors.Values.ToArray () };
                #endregion
                #region assets
                ProductionSite siteParis = new ProductionSite () { Code = "Site of Paris", Label = "Site of Paris, production of water", Location = "Paris", Sector = sectors [ "W" ] };
                ArraysHelper.AddElementToArray ( root, siteParis );
                ProductionSite siteLivry = new ProductionSite () { Code = "Site of Livry", Label = "Site of Livry-Gargan, quality of water", Location = "Livry-Gargan", Sector = sectors [ "S" ] };
                ArraysHelper.AddElementToArray ( root, siteLivry );
                GroupOfPumps grouppumps = new GroupOfPumps () { Code = "GP 001" };
                ArraysHelper.AddElementToArray ( siteParis, grouppumps );
                PressureSensor iot1 = new PressureSensor () { Code = "PS 001", TelephoneNumber = "0123456789" };
                ArraysHelper.AddElementToArray ( grouppumps, iot1 );
                WaterPump wp1 = new WaterPump () { Code = "WP 001" };
                ArraysHelper.AddElementToArray ( grouppumps, wp1 );
                WaterPump wp2 = new WaterPump () { Code = "WP 002" };
                ArraysHelper.AddElementToArray ( grouppumps, wp2 );
                ClorineInjector cl1 = new ClorineInjector () { Code = "CI 001" };
                ArraysHelper.AddElementToArray ( siteLivry, cl1 );
                MultiFunctionSensor iot2 = new MultiFunctionSensor () { Code = "MFS 001", TelephoneNumber = "1234567890" };
                ArraysHelper.AddElementToArray ( cl1, iot2 );
                #endregion
                #region measures
                AcquiredMeasure am1 = new AcquiredMeasure () { Code = "M 001", ScadaExternalReference = "EXTREF 001", TypeOfMeasure = measures [ "WT" ], ConsolidationType = ConsolidationType.None };
                ArraysHelper.AddElementToArray ( grouppumps, am1 );
                AcquiredMeasure am2 = new AcquiredMeasure () { Code = "M 002", ScadaExternalReference = "EXTREF 002", TypeOfMeasure = measures [ "ELEC" ], ConsolidationType = ConsolidationType.None };
                ArraysHelper.AddElementToArray ( wp1, am2 );
                AcquiredMeasure am3 = new AcquiredMeasure () { Code = "M 003", ScadaExternalReference = "EXTREF 003", TypeOfMeasure = measures [ "ELEC" ], ConsolidationType = ConsolidationType.None };
                ArraysHelper.AddElementToArray ( wp2, am3 );
                AcquiredMeasure am4 = new AcquiredMeasure () { Code = "M 004", ScadaExternalReference = "EXTREF 004", TypeOfMeasure = measures [ "PRESS" ], ConsolidationType = ConsolidationType.None };
                ArraysHelper.AddElementToArray ( iot1, am4 );
                ComputedMeasure am5 = new ComputedMeasure () { Code = "M 005", ScadaExternalReference = "EXTREF 005", TypeOfMeasure = measures [ "ELEC" ], ConsolidationType = ConsolidationType.Average };
                ArraysHelper.AddElementToArray ( siteParis, am5 );
                AcquiredMeasure am6 = new AcquiredMeasure () { Code = "M 006", ScadaExternalReference = "EXTREF 006", TypeOfMeasure = measures [ "CL" ], ConsolidationType = ConsolidationType.None };
                ArraysHelper.AddElementToArray ( iot2, am6 );
                AcquiredMeasure am7 = new AcquiredMeasure () { Code = "M 007", ScadaExternalReference = "EXTREF 007", TypeOfMeasure = measures [ "PRESS" ], ConsolidationType = ConsolidationType.None };
                ArraysHelper.AddElementToArray ( cl1, am7 );
                AcquiredMeasure am8 = new AcquiredMeasure () { Code = "M 008", ScadaExternalReference = "EXTREF 008", TypeOfMeasure = measures [ "ELEC" ], ConsolidationType = ConsolidationType.None };
                ArraysHelper.AddElementToArray ( cl1, am8 );
                AcquiredMeasure am9 = new AcquiredMeasure () { Code = "M 009", ScadaExternalReference = "EXTREF 010", TypeOfMeasure = measures [ "CL" ], ConsolidationType = ConsolidationType.None };
                ArraysHelper.AddElementToArray ( cl1, am9 );
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
                Console.WriteLine ( "Exception => " + e.GetType () + " = " + e.Message );
                ret = 1;
            }
            return ( ret );
        }
    }
}
