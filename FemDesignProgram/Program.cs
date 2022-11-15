using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using FemDesign;
using System.Globalization;


namespace StructuralElementsExporter.StructuralAnalysis
{



    public class FemDesignProgram
    {
        static void Main(string[] args)
        {

            string path = @"C:\Users\camil\FEM-design_API_test.struxml";
            string bscPathtest = @"C:\femdesign-api\quantities_test.bsc";
            string outFolder = @"C:\Users\camil\FemDesign_API_test";
            string tempPath = outFolder + "temp.struxml";

            //List<XmlElement> xmlElements = new List<XmlElement>();


            Model model = Model.DeserializeFromFilePath(path);
            //Quantities quantities = quantities.DeserializeFromFilePath(bscPathtest);

            //double concreteCost = 20;
            //double reinforcementCost = 70;
            //double concreteWeight = 0;
            //double reinforcementWeight = 0;

            //public Dictionary<string, string> MaterialProperties { get; set; } = new Dictionary<string, string>();

            List<object> bars = new List<object>();

            bars.Add(model.Entities.Bars);


            FemDesign.Shells.Slab slab = model.Entities.Slabs[0];
            FemDesign.Materials.Material material = model.Entities.Slabs[0].Material;
            //FemDesign.Materials.Material material = model.Entities.Slabs[1];

            FemDesign.Bars.Bar bar = model.Entities.Bars[0];

            FemDesign.Shells.Panel panel = model.Entities.Panels[0];

        }


        public string materialID =


        #region Analysis Setup
        bool NLE = true;
            bool PL = false;
        bool NLS = false;
        bool Cr = false;
        bool _2nd = false;

        // SETTING UP LOAD COMBINATIONS
        // In this example, we use the same settings (CombItem)
        // for all load combinations, applied with a simple loop.
        var combItem = new FemDesign.Calculate.CombItem(0, 0, NLE, PL, NLS, Cr, _2nd);

        int numLoadCombs = model.Entities.Loads.LoadCombinations.Count;
        var combItems = new List<FemDesign.Calculate.CombItem>();
            for (int i = 0; i<numLoadCombs; i++)
            {
                combItems.Add(combItem);
            }

    FemDesign.Calculate.Comb comb = new FemDesign.Calculate.Comb();
    comb.CombItem = combItems.ToList();

            #endregion

            double low = 0.2;
    double high = 0.7;

    List<double> costs = new List<double>();

            for (double i = low; i<high; i = i + 0.5)
            {
                double thickness = i;
    slab.SlabPart.Thickness[0].Value = Math.Round(thickness, 3);

                //Save temporary model
                model.SerializeModel(tempPath);


                //Calculate cost, write to console app and write to list
                double totalCost = concreteCost * concreteWeight + reinforcementCost * reinforcementWeight;
    Console.WriteLine(string.Format("{0} {1} {2}", "Cost: ", totalCost, Math.Round(thickness, 3) + "m"));
                costs.Add(totalCost);

            }


        }

        //public static void RunAnalysis(string modelPath, List<string> bscFilePaths)
        //{
        //    FemDesign.Calculate.Analysis analysis = new FemDesign.Calculate.Analysis(null, null, null, null, false, false, false, false, false, false, false, false, false, false, false, false);
        //    FemDesign.Calculate.Design design = new FemDesign.Calculate.Design(true, false);
        //    FemDesign.Calculate.FdScript fdScript = FemDesign.Calculate.FdScript.Design("rc", modelPath, analysis, design, bscFilePaths, "", true);
        //    FemDesign.Calculate.Application app = new FemDesign.Calculate.Application();
        //    //app.RunRfScript(fdScript, false, true, true);
        //}

    }

