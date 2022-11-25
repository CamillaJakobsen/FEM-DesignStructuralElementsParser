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
using FemDesign.Materials;
using Newtonsoft.Json;
using FemDesignProgram.Helpers;

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

            //string path = @"C:\femdesign-api\FEM-design_files\fem-climate-example.struxml";
            //string bscPathtest = @"C:\femdesign-api\FEM-design_files\fem-climate-example.bsc";
            //string outFolder = @"C:\femdesign-api\FEM-design_files\";
            //string tempPath = outFolder + "temp.struxml";

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

            // Creating the bsc paths in C:\femdesign-api\quantities_test\scripts
            var bscPathsFromResultTypes = FemDesign.Calculate.Bsc.BscPathFromResultTypes(resultTypes, bscPathtest);


            #region ANALYSIS
            // Running the analysis
            var analysisSettings = FemDesign.Calculate.Analysis.StaticAnalysis();

            // creates csv files
            var fdScript = FemDesign.Calculate.FdScript.Analysis(path, analysisSettings, bscPathsFromResultTypes, null, true);
            

            var app = new FemDesign.Calculate.Application();
            // creates the csv files at the location: C:\femdesign-api\Quantities\FEM-design_quantities\results
            app.RunFdScript(fdScript, false, true);
            model.SerializeModel(path);

            // Read model and results
            model = Model.DeserializeFromFilePath(fdScript.StruxmlPath);
            #endregion
            
            Beams beams = new Beams();
            Decks decks = new Decks();
            Walls walls = new Walls();
            Columns columns = new Columns();
            Foundations foundations = new Foundations();

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
                    var valuesMaterial = line.Split("\t");
                    string currentMaterialString = valuesMaterial[0];
                    if (currentMaterialString == "Quantity estimation, Concrete")
                    {
                            var nextLine = reader.ReadLine(); 
                        while (!nextLine.Contains("TOTAL") && !reader.EndOfStream)
                        {
                            nextLine = reader.ReadLine();
                            var values = nextLine.Split("\t");
                            if (values[0] != "Storey" & values[0] != "" & line != "")
                            {
                                if (values[1] == "Beam" & line != "")
                                {
                                    string typeID = values[2];
                                    string quality = values[3];
                                    string material = "Concrete";
                                    string volumeString = values[8];
                                    double volume = Double.Parse(volumeString.Replace('.', '.'), CultureInfo.InvariantCulture);
                                    string lengthString = values[7];
                                    double length = Double.Parse(lengthString.Replace('.', '.'), CultureInfo.InvariantCulture);
                                    string weightString = values[9];
                                    double weight = Double.Parse(weightString.Replace('.', '.'), CultureInfo.InvariantCulture);

                                    Beam beam = new Beam(typeID, material, quality, length, volume, weight);
                                    beams.AddBeam(beam);

                                }
                                else if (values[1] == "Column" & line != "")
                                {
                                    string typeID = values[2];
                                    string material = "Concrete";
                                    string quality = values[3];
                                    string volumeString = values[8];
                                    double volume = Double.Parse(volumeString.Replace('.', '.'), CultureInfo.InvariantCulture);
                                    string lengthString = values[7];
                                    double length = Double.Parse(lengthString.Replace('.', '.'), CultureInfo.InvariantCulture);
                                    string weightString = values[9];
                                    double weight = Double.Parse(weightString.Replace('.', '.'), CultureInfo.InvariantCulture);

                                    Column column = new Column(typeID, material, quality, length, volume, weight);
                                    columns.AddColumn(column);

                                }
                                else if (values[1] == "Truss" & line != "")
                                {
                                    string typeID = values[2];
                                    string material = "Concrete";
                                    string quality = values[3];
                                    string volumeString = values[8];
                                    double volume = Double.Parse(volumeString.Replace('.', '.'), CultureInfo.InvariantCulture);
                                    string lengthString = values[7];
                                    double length = Double.Parse(lengthString.Replace('.', '.'), CultureInfo.InvariantCulture);
                                    string weightString = values[9];
                                    double weight = Double.Parse(weightString.Replace('.', '.'), CultureInfo.InvariantCulture);

                                    Column column = new Column(typeID, material, quality, length, volume, weight);
                                    columns.AddColumn(column);

                                }
                                else if (values[1] == "Plate" & line != "")
                                {
                                    string typeID = values[2];
                                    string material = "Concrete";
                                    string quality = values[3];
                                    string areaString = values[7];
                                    double area = Double.Parse(areaString.Replace('.', '.'), CultureInfo.InvariantCulture);
                                    string thicknessString = values[4];
                                    double thickness = Double.Parse(thicknessString.Replace('.', '.'), CultureInfo.InvariantCulture);
                                    string weightString = values[9];
                                    double weight = Double.Parse(weightString.Replace('.', '.'), CultureInfo.InvariantCulture);

                                    Deck deck = new Deck(typeID, material, quality, area, thickness, weight);
                                    decks.AddDeck(deck);
                                }
                                else if (values[1] == "Wall" & line != "")
                                {
                                    string typeID = values[2];
                                    string material = "Concrete";
                                    string quality = values[3];
                                    string areaString = values[7];
                                    double area = Double.Parse(areaString.Replace('.', '.'), CultureInfo.InvariantCulture);
                                    string thicknessString = values[4];
                                    double thickness = Double.Parse(thicknessString.Replace('.', '.'), CultureInfo.InvariantCulture);
                                    string weightString = values[9];
                                    double weight = Double.Parse(weightString.Replace('.', '.'), CultureInfo.InvariantCulture);

                                    Wall wall = new Wall(typeID, material, quality, area, thickness, weight);
                                    walls.AddWall(wall);
                                }
                                else if (values[1] == "Foundation slab" || values[1] == "Wall foundation" || values[1] == "Isolated foundation" & line != "")
                                {
                                    string typeID = values[2];
                                    string material = "Concrete";
                                    string quality = values[3];
                                    string volumeString = values[8];
                                    double volume = Double.Parse(volumeString.Replace('.', '.'), CultureInfo.InvariantCulture);
                                    string weightString = values[9];
                                    double weight = Double.Parse(volumeString.Replace('.', '.'), CultureInfo.InvariantCulture);

                                    Foundation foundation = new Foundation(typeID, material, quality, volume, weight);
                                    foundations.AddFoundation(foundation);
                                }
                                counter++;
                            }
                        }
                          
                        counter++;
                        }
                    else if (currentMaterialString == "Quantity estimation, Reinforcement")
                    {
                        var nextLine = reader.ReadLine();
                        while (!nextLine.Contains("TOTAL") && !reader.EndOfStream)
                        {
                            nextLine = reader.ReadLine();
                            var values = nextLine.Split("\t");
                            if (values[0] == "-" & line != "")
                            {
                                if (values[1] == "Beam" & line != "")
                                {
                                    string typeID = values[2];
                                    string quality = values[3];
                                    string material = "Reinforcement";
                                    string volumeString = "0";
                                    double volume = Double.Parse(volumeString.Replace('.', '.'), CultureInfo.InvariantCulture);
                                    string lengthString = "0";
                                    double length = Double.Parse(lengthString.Replace('.', '.'), CultureInfo.InvariantCulture);
                                    string weightString = values[5];
                                    double weight = Double.Parse(weightString.Replace('.', '.'), CultureInfo.InvariantCulture);

                                    Beam beam = new Beam(typeID, material, quality, length, volume, weight);
                                    beams.AddBeam(beam);

                                }
                                else if (values[1] == "Column" & line != "")
                                {
                                    string typeID = values[2];
                                    string quality = values[3];
                                    string material = "Reinforcement";
                                    string volumeString = "0";
                                    double volume = Double.Parse(volumeString.Replace('.', '.'), CultureInfo.InvariantCulture);
                                    string lengthString = "0";
                                    double length = Double.Parse(lengthString.Replace('.', '.'), CultureInfo.InvariantCulture);
                                    string weightString = values[5];
                                    double weight = Double.Parse(weightString.Replace('.', '.'), CultureInfo.InvariantCulture);

                                    Column column = new Column(typeID, material, quality, length, volume, weight);
                                    columns.AddColumn(column);

                                }
                                else if (values[1] == "Truss" & line != "")
                                {
                                    string typeID = values[2];
                                    string quality = values[3];
                                    string material = "Reinforcement";
                                    string volumeString = "0";
                                    double volume = Double.Parse(volumeString.Replace('.', '.'), CultureInfo.InvariantCulture);
                                    string lengthString = "0";
                                    double length = Double.Parse(lengthString.Replace('.', '.'), CultureInfo.InvariantCulture);
                                    string weightString = values[5];
                                    double weight = Double.Parse(weightString.Replace('.', '.'), CultureInfo.InvariantCulture);

                                    Column column = new Column(typeID, material, quality, length, volume, weight);
                                    columns.AddColumn(column);

                                }
                                else if (values[1] == "Plate" & line != "")
                                {
                                        if (values[2].Substring(0, 1) == "P")
                                        {
                                            string typeID = values[2];
                                            string quality = values[3];
                                            string material = "Reinforcement";
                                            string areaString = "0";
                                            double area = Double.Parse(areaString.Replace('.', '.'), CultureInfo.InvariantCulture);
                                            string thicknessString = "0";
                                            double thickness = Double.Parse(thicknessString.Replace('.', '.'), CultureInfo.InvariantCulture);
                                            string weightString = values[5];
                                            double weight = Double.Parse(weightString.Replace('.', '.'), CultureInfo.InvariantCulture);

                                            Deck deck = new Deck(typeID, material, quality, area, thickness, weight);
                                            decks.AddDeck(deck);
                                        }
                                        if (values[2].Substring(0, 1) == "F")
                                        {
                                            string typeID = values[2];
                                            string quality = values[3];
                                            string material = "Reinforcement";
                                            double volume = 0;
                                            string weightString = values[5];
                                            double weight = Double.Parse(weightString.Replace('.', '.'), CultureInfo.InvariantCulture);

                                            Foundation foundation = new Foundation(typeID, material, quality, volume, weight);
                                            foundations.AddFoundation(foundation);
                                        }
                                    }
                                else if (values[1] == "Wall" & line != "")
                                {
                                    string typeID = values[2];
                                    string quality = values[3];
                                    string material = "Reinforcement";
                                    string areaString = "0";
                                    double area = Double.Parse(areaString.Replace('.', '.'), CultureInfo.InvariantCulture);
                                    string thicknessString = "0";
                                    double thickness = Double.Parse(thicknessString.Replace('.', '.'), CultureInfo.InvariantCulture);
                                    string weightString = values[5];
                                    double weight = Double.Parse(weightString.Replace('.', '.'), CultureInfo.InvariantCulture);

                                    Wall wall = new Wall(typeID, material, quality, area, thickness, weight);
                                    walls.AddWall(wall);
                                }
                            }
                             
                        }
                        counter++;
                    }
                    else if (currentMaterialString == "Quantity estimation, Steel")
                    {
                        var nextLine = reader.ReadLine();
                        while (!nextLine.Contains("TOTAL") && !reader.EndOfStream)
                        {
                            nextLine = reader.ReadLine();
                            var values = nextLine.Split("\t");
                            if (values[0] == "-" & line != "")
                            {
                                if (values[1] == "Beam" & line != "")
                                {
                                    string typeID = values[2];
                                    string quality = values[3];
                                    string material = "Steel";
                                    string volumeString = "0";
                                    double volume = Double.Parse(volumeString.Replace('.', '.'), CultureInfo.InvariantCulture);
                                    string lengthString = values[6];
                                    double length = Double.Parse(lengthString.Replace('.', '.'), CultureInfo.InvariantCulture);
                                    string weightString = values[7];
                                    double weight = Double.Parse(weightString.Replace('.', '.'), CultureInfo.InvariantCulture);

                                    Beam beam = new Beam(typeID, material, quality, length, volume, weight);
                                    beams.AddBeam(beam);

                                }
                                else if (values[1] == "Column" & line != "")
                                {
                                    string typeID = values[2];
                                    string material = "Steel";
                                    string quality = values[3];
                                    string volumeString = "0";
                                    double volume = Double.Parse(volumeString.Replace('.', '.'), CultureInfo.InvariantCulture);
                                    string lengthString = values[6];
                                    double length = Double.Parse(lengthString.Replace('.', '.'), CultureInfo.InvariantCulture);
                                    string weightString = values[7];
                                    double weight = Double.Parse(weightString.Replace('.', '.'), CultureInfo.InvariantCulture);

                                    Column column = new Column(typeID, material, quality, length, volume, weight);
                                    columns.AddColumn(column);

                                }
                                else if (values[1] == "Truss" & line != "")
                                {
                                    string typeID = values[2];
                                    string quality = values[3];
                                    string material = "Steel";
                                    string volumeString = "0";
                                    double volume = Double.Parse(volumeString.Replace('.', '.'), CultureInfo.InvariantCulture);
                                    string lengthString = values[6];
                                    double length = Double.Parse(lengthString.Replace('.', '.'), CultureInfo.InvariantCulture);
                                    string weightString = values[7];
                                    double weight = Double.Parse(weightString.Replace('.', '.'), CultureInfo.InvariantCulture);

                                    Column column = new Column(typeID, material, quality, length, volume, weight);
                                    columns.AddColumn(column);

                                }
                                else if (values[1] == "Plate" & line != "")
                                {
                                    string typeID = values[2];
                                    string quality = values[3];
                                    string material = "Steel";
                                    string areaString = values[7];
                                    double area = Double.Parse(areaString.Replace('.', '.'), CultureInfo.InvariantCulture);
                                    string thicknessString = values[4];
                                    double thickness = Double.Parse(thicknessString.Replace('.', '.'), CultureInfo.InvariantCulture);
                                    string weightString = values[9];
                                    double weight = Double.Parse(weightString.Replace('.', '.'), CultureInfo.InvariantCulture);

                                    Deck deck = new Deck(typeID, material, quality, area, thickness, weight);
                                    decks.AddDeck(deck);
                                }
                                else if (values[1] == "Wall" & line != "")
                                {
                                    string typeID = values[2];
                                    string quality = values[3];
                                    string material = "Steel";
                                    string areaString = values[7];
                                    double area = Double.Parse(areaString.Replace('.', '.'), CultureInfo.InvariantCulture);
                                    string thicknessString = values[4];
                                    double thickness = Double.Parse(thicknessString.Replace('.', '.'), CultureInfo.InvariantCulture);
                                    string weightString = values[9];
                                    double weight = Double.Parse(weightString.Replace('.', '.'), CultureInfo.InvariantCulture);

                                    Wall wall = new Wall(typeID, material, quality, area, thickness, weight);
                                    walls.AddWall(wall);
                                }
                            }
                                
                        }
                        counter++;
                    }
                    else if (currentMaterialString == "Quantity estimation, Timber")
                    {
                        var nextLine = reader.ReadLine();
                        while (!nextLine.Contains("TOTAL") && !reader.EndOfStream)
                        {
                            nextLine = reader.ReadLine();
                            var values = nextLine.Split("\t");
                            if (values[0] == "-" & line != "")
                            {
                                if (values[1] == "Beam" & line != "")
                                {
                                    string typeID = values[2];
                                    string quality = values[3];
                                    string material = "Timber";
                                    string volumeString = "0";
                                    double volume = Double.Parse(volumeString.Replace('.', '.'), CultureInfo.InvariantCulture);
                                    string lengthString = values[6];
                                    double length = Double.Parse(lengthString.Replace('.', '.'), CultureInfo.InvariantCulture);
                                    string weightString = values[7];
                                    double weight = Double.Parse(weightString.Replace('.', '.'), CultureInfo.InvariantCulture);

                                    Beam beam = new Beam(typeID, material, quality, length, volume, weight);
                                    beams.AddBeam(beam);

                                }
                                else if (values[1] == "Column" & line != "")
                                {
                                    string typeID = values[2];
                                    string material = "Timber";
                                    string quality = values[3];
                                    string volumeString = "0";
                                    double volume = Double.Parse(volumeString.Replace('.', '.'), CultureInfo.InvariantCulture);
                                    string lengthString = values[6];
                                    double length = Double.Parse(lengthString.Replace('.', '.'), CultureInfo.InvariantCulture);
                                    string weightString = values[7];
                                    double weight = Double.Parse(weightString.Replace('.', '.'), CultureInfo.InvariantCulture);

                                    Column column = new Column(typeID, material, quality, length, volume, weight);
                                    columns.AddColumn(column);

                                }
                                else if (values[1] == "Truss" & line != "")
                                {
                                    string typeID = values[2];
                                    string quality = values[3];
                                    string material = "Timber";
                                    string volumeString = "0";
                                    double volume = Double.Parse(volumeString.Replace('.', '.'), CultureInfo.InvariantCulture);
                                    string lengthString = values[6];
                                    double length = Double.Parse(lengthString.Replace('.', '.'), CultureInfo.InvariantCulture);
                                    string weightString = values[7];
                                    double weight = Double.Parse(weightString.Replace('.', '.'), CultureInfo.InvariantCulture);

                                    Column column = new Column(typeID, material, quality, length, volume, weight);
                                    columns.AddColumn(column);

                                }
                                else if (values[1] == "Plate" & line != "")
                                {
                                    string typeID = values[2];
                                    string quality = values[3];
                                    string material = "Timber";
                                    string areaString = values[6];
                                    double area = Double.Parse(areaString.Replace('.', '.'), CultureInfo.InvariantCulture);
                                    string thicknessString = values[4];
                                    double thickness = Double.Parse(thicknessString.Replace('.', '.'), CultureInfo.InvariantCulture);
                                    string weightString = values[7];
                                    double weight = Double.Parse(weightString.Replace('.', '.'), CultureInfo.InvariantCulture);

                                    Deck deck = new Deck(typeID, material, quality, area, thickness, weight);
                                    decks.AddDeck(deck);
                                }
                                else if (values[1] == "Wall" & line != "")
                                {
                                    string typeID = values[2];
                                    string quality = values[3];
                                    string material = "Timber";
                                    string areaString = values[6];
                                    double area = Double.Parse(areaString.Replace('.', '.'), CultureInfo.InvariantCulture);
                                    string thicknessString = values[4];
                                    double thickness = Double.Parse(thicknessString.Replace('.', '.'), CultureInfo.InvariantCulture);
                                    string weightString = values[7];
                                    double weight = Double.Parse(weightString.Replace('.', '.'), CultureInfo.InvariantCulture);

                                    Wall wall = new Wall(typeID, material, quality, area, thickness, weight);
                                    walls.AddWall(wall);
                                }
                            }

                        }
                        counter++;
                    }
                    else if (currentMaterialString == "Quantity estimation, Profiled panel")
                    {
                        var nextLine = reader.ReadLine();
                        while (!nextLine.Contains("TOTAL") && !reader.EndOfStream)
                        {
                            nextLine = reader.ReadLine();
                            var values = nextLine.Split("\t");
                            if (values[0] != "Storey" & values[0] != "" & line != "")
                            {
                                if (values[1] == "Plate" & line != "")
                                {
                                    string typeID = values[2];
                                    string material = "Prefabricated Concrete";
                                    string quality = values[3];
                                    string areaString = values[7];
                                    double area = Double.Parse(areaString.Replace('.', '.'), CultureInfo.InvariantCulture);
                                    string thicknessString = values[5];
                                    double thickness = mmTomConverter.Convert(Double.Parse(thicknessString.Replace('.', '.'), CultureInfo.InvariantCulture));
                                    string weightString = values[9];
                                    double weight = Double.Parse(weightString.Replace('.', '.'), CultureInfo.InvariantCulture);

                                    Deck deck = new Deck(typeID, material, quality, area, thickness, weight);
                                    decks.AddDeck(deck);
                                }

                                counter++;
                            }
                        }

                        counter++;
                    }
                    else if (currentMaterialString == "Quantity estimation, Timber panel")
                    {
                        var nextLine = reader.ReadLine();
                        while (!nextLine.Contains("TOTAL") && !reader.EndOfStream)
                        {
                            nextLine = reader.ReadLine();
                            var values = nextLine.Split("\t");
                            if (values[0] != "Storey" & values[0] != "" & line != "")
                            {
                                if (values[1] == "Plate" & line != "")
                                {
                                    string typeID = values[2];
                                    string material = "CLT";
                                    string quality = values[3];
                                    string areaString = values[8];
                                    double area = Double.Parse(areaString.Replace('.', '.'), CultureInfo.InvariantCulture);
                                    string thicknessString = values[4];
                                    double thickness = Double.Parse(thicknessString.Replace('.', '.'), CultureInfo.InvariantCulture);
                                    string weightString = values[9];
                                    double weight = Double.Parse(weightString.Replace('.', '.'), CultureInfo.InvariantCulture);

                                    Deck deck = new Deck(typeID, material, quality, area, thickness, weight);
                                    decks.AddDeck(deck);
                                }

                            counter++;
                            }
                        }

                            counter++;
                    }

                    }
                             
                }

            }

            // Add all structural elements to a Dictionary of Structuralelements
            Dictionary<string, List<object>> structuralElements = new Dictionary<string, List<object>>();

            structuralElements.Add("Beam", beams.BeamsInModel);
            structuralElements.Add("Column", columns.ColumnsInModel);
            structuralElements.Add("Deck", decks.DecksInModel);
            structuralElements.Add("Wall", walls.WallsInModel);
            //structuralElements.Add("Foundation", foundations.FoundationsInModel);

            // Lav breakpoint og kopier JSON filen.
            JsonConvert.SerializeObject(structuralElements);

            File.WriteAllText(@"C:\femdesign-api\Quantities\Structuralelements_Json", JsonConvert.SerializeObject(structuralElements));
            
        }

    }
}
    