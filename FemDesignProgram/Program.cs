using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using FemDesign;
using System.Globalization;
using System.Security.Cryptography.X509Certificates;
using System.Reflection;
using System.Reflection.PortableExecutable;
using FemDesign.Results;
using FemDesignProgram.Containers;

namespace StructuralElementsExporter.StructuralAnalysis
{



    public class FemDesignProgram
    {
        static void Main(string[] args)
        {

            string path = @"C:\femdesign-api\Quantities\FEM-design_quantities.struxml";
            string bscPathtest = @"C:\femdesign-api\quantities_test.bsc";
            string outFolder = @"C:\femdesign-api\";
            string tempPath = outFolder + "temp.struxml";

            //List<XmlElement> xmlElements = new List<XmlElement>();

            // Read model
            Model model = Model.DeserializeFromFilePath(path);
            //Quantities quantities = quantities.DeserializeFromFilePath(bscPathtest);

            var units = new FemDesign.Results.UnitResults(FemDesign.Results.Length.m, FemDesign.Results.Angle.deg, FemDesign.Results.SectionalData.mm, FemDesign.Results.Force.kN, FemDesign.Results.Mass.kg, FemDesign.Results.Displacement.cm, FemDesign.Results.Stress.MPa);

            var resultTypes = new List<Type>
            {
                typeof(FemDesign.Results.QuantityEstimationConcrete),
                typeof(FemDesign.Results.QuantityEstimationReinforcement),
                typeof(FemDesign.Results.QuantityEstimationSteel),
                typeof(FemDesign.Results.QuantityEstimationTimber),
                typeof(FemDesign.Results.QuantityEstimationProfiledPlate),
                typeof(FemDesign.Results.QuantityEstimationTimberPanel)
            };

            var bscPathsFromResultTypes = FemDesign.Calculate.Bsc.BscPathFromResultTypes(resultTypes, bscPathtest);


            #region ANALYSIS
            // Running the analysis
            var analysisSettings = FemDesign.Calculate.Analysis.StaticAnalysis();

            // creates csv files
            var fdScript = FemDesign.Calculate.FdScript.Analysis(path, analysisSettings, bscPathsFromResultTypes, null, true);
            //FemDesign.Calculate.CmdListGen(outFolder)

            var app = new FemDesign.Calculate.Application();
            app.RunFdScript(fdScript, false, true);
            model.SerializeModel(path);

            // Read model and results
            model = Model.DeserializeFromFilePath(fdScript.StruxmlPath);
            #endregion

            #region EXTRACT RESULTS

            //IEnumerable<FemDesign.Results.IResult> results = Enumerable.Empty<FemDesign.Results.IResult>();

            //foreach (var cmd in fdScript.CmdListGen)
            //{
            //    string path2 = cmd.OutFile;
            //    var _results = FemDesign.Results.ResultsReader.Parse(path2);
            //    results = results.Concat(_results);
            //}
            #endregion


            //        //Read results from csv file
            //        double concreteWeight = 0;
            //        int counter = 0;
            
            IEnumerable<FemDesign.Results.IQuantityEstimationResult> quantityEstimations = Enumerable.Empty<FemDesign.Results.IQuantityEstimationResult>();
            Beams beams = new Beams();
            Decks decks = new Decks();
            Walls walls = new Walls();
            Columns columns = new Columns();

            foreach (var cmd in fdScript.CmdListGen)
            {
                string csvfiles = cmd.OutFile;
                var _results = FemDesign.Results.ResultsReader.Parse(csvfiles);
                int counter = 0;
                using (var reader = new StreamReader(csvfiles))
                {
                    while (!reader.EndOfStream)
                    {
                    var line = reader.ReadLine();
                    var values = line.Split("\t");
                    if (values[0] == "-" & line != "")
                    {
                        if (values[1] == "Beam" & line != "")
                            {
                                string v = values[2];
                                string typeID = v;
                                string materialID = values[3];
                                string volumeString = values[8];
                                double volume = Double.Parse(volumeString.Replace('.', '.'), CultureInfo.InvariantCulture);
                                string lengthString = values[7];
                                double length = Double.Parse(lengthString.Replace('.', '.'), CultureInfo.InvariantCulture);
                                string weightString = values[9];
                                double weight = Double.Parse(weightString.Replace('.', '.'), CultureInfo.InvariantCulture);

                                Beam beam = new Beam(typeID, materialID, length, volume, weight);
                                beams.AddBeam(beam);
                              
                            }
                        else if (values[1] == "Column" & line != "")
                            {
                                string v = values[2];
                                string typeID = v;
                                string materialID = values[3];
                                string volumeString = values[8];
                                double volume = Double.Parse(volumeString.Replace('.', '.'), CultureInfo.InvariantCulture);
                                string lengthString = values[7];
                                double length = Double.Parse(lengthString.Replace('.', '.'), CultureInfo.InvariantCulture);
                                string weightString = values[9];
                                double weight = Double.Parse(weightString.Replace('.', '.'), CultureInfo.InvariantCulture);

                                Column column = new Column(typeID, materialID, length, volume, weight);
                                columns.AddColumn(column);

                            }
                            else if (values[1] == "Truss" & line != "")
                            {
                                string v = values[2];
                                string typeID = v;
                                string materialID = values[3];
                                string volumeString = values[8];
                                double volume = Double.Parse(volumeString.Replace('.', '.'), CultureInfo.InvariantCulture);
                                string lengthString = values[7];
                                double length = Double.Parse(lengthString.Replace('.', '.'), CultureInfo.InvariantCulture);
                                string weightString = values[9];
                                double weight = Double.Parse(weightString.Replace('.', '.'), CultureInfo.InvariantCulture);

                                Column column = new Column(typeID, materialID, length, volume, weight);
                                columns.AddColumn(column);

                            }
                            else if (values[1] == "Plate" & line != "")
                            {
                                string typeID = values[2];
                                string materialID = values[3];
                                string areaString = values[7];
                                double area = Double.Parse(areaString.Replace('.', '.'), CultureInfo.InvariantCulture);
                                string thicknessString = values[4];
                                double thickness = Double.Parse(thicknessString.Replace('.', '.'), CultureInfo.InvariantCulture);

                                Deck deck = new Deck(typeID, materialID, area, thickness);
                                decks.AddDeck(deck);
                            }
                        else if (values[1] == "Wall" & line != "")
                            {
                                string typeID = values[2];
                                string materialID = values[3];
                                string areaString = values[7];
                                double area = Double.Parse(areaString.Replace('.', '.'), CultureInfo.InvariantCulture);
                                string thicknessString = values[4];
                                double thickness = Double.Parse(thicknessString.Replace('.', '.'), CultureInfo.InvariantCulture);

                                Wall wall = new Wall(typeID, materialID, area, thickness);
                                walls.AddWall(wall);
                            }

                            //double length = X509SubjectKeyIdentifierHashAlgorithm nok komme fra struxml
                            //double volume = values[10];
                            //ouble weight = values[11];
                            //Console.WriteLine(string.Format("{0} {1} {2}", values[0], "concrete", values[10]));
                            //concreteWeight = double.Parse(values[9], CultureInfo.InvariantCulture);
                        }
                    counter++;

                    }
                             
                }





                // Display Results on Screen
                // The results are grouped by their type
                //var resultGroups = results.GroupBy(t => t.GetType()).ToList();
                //foreach (var resultGroup in resultGroups)
                //{
                //    Console.WriteLine(resultGroup.Key.Name);
                //    Console.WriteLine();
                //    foreach (var result in resultGroup)
                //    {
                //        Console.WriteLine(result);
                //    }
                //    Console.WriteLine();



                //}



            }
        }

    }
}
    