using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using FemDesign;
using System.Globalization;
using FemDesignProgram.Containers;
using Newtonsoft.Json;
using FemDesignProgram.Helpers;
using FemDesign.Shells;
using FemDesign.Results;

namespace StructuralElementsExporter.StructuralAnalysis
{

    public class QuantityEstimation
    {
        public void Takeoff()
        {

            //string path = @"C:\Users\camil\OneDrive\OneDrive_Privat\OneDrive\Bygningsdesign kandidat\Speciale\femdesign-api\Quantities\FEM-design_quantities.struxml";
            //string bscPathtest = @"C:\Users\camil\OneDrive\OneDrive_Privat\OneDrive\Bygningsdesign kandidat\Speciale\femdesign-api\quantities_test.bsc";

            //string path = @"C:\Users\camil\OneDrive\OneDrive_Privat\OneDrive\Bygningsdesign kandidat\Speciale\femdesign-api\Fem-design-modeller-Rambøll\B6_5D_ver.struxml";
            //string bscPathtest = @"C:\Users\camil\OneDrive\OneDrive_Privat\OneDrive\Bygningsdesign kandidat\Speciale\femdesign-api\quantities_test.bsc";

            string path = @"C:\Users\camil\OneDrive\OneDrive_Privat\OneDrive\Bygningsdesign kandidat\Speciale\Testing\FEM-design_2\FEM-design_timber model.struxml";
            string bscPathtest = @"C:\Users\camil\OneDrive\OneDrive_Privat\OneDrive\Bygningsdesign kandidat\Speciale\femdesign-api\quantities_test.bsc";

            //string path = @"C:\Users\camil\OneDrive\OneDrive_Privat\OneDrive\Bygningsdesign kandidat\Speciale\femdesign-api\FEM-design_files\fem-climate-example.struxml";
            //string bscPathtest = @"C:\Users\camil\OneDrive\OneDrive_Privat\OneDrive\Bygningsdesign kandidat\Speciale\femdesign-api\FEM-design_files\fem-climate-example.bsc";
            //string outFolder = @"C:\femdesign-api\FEM-design_files\";
            //string tempPath = outFolder + "temp.struxml";

            //List<XmlElement> xmlElements = new List<XmlElement>();

            // Read model
            Model model = Model.DeserializeFromFilePath(path);
            //Quantities quantities = quantities.DeserializeFromFilePath(bscPathtest);

            
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

                                    Beam beam = new Beam(typeID, material, quality, volume, weight);
                                    beams.AddBeam(beam);

                                    if (values[11] != "" & values[11] != null)
                                    {
                                        material = "Reinforcement";
                                        weight = Double.Parse(values[11].Replace('.', '.'), CultureInfo.InvariantCulture) * length;
                                        quality = "";

                                        Beam beamReinforcement = new Beam(typeID, material, quality, volume, weight);
                                        beams.AddBeam(beamReinforcement);

                                    }

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

                                    Column column = new Column(typeID, material, quality, volume, weight);
                                    columns.AddColumn(column);

                                    if (values[11] != "" & values[11] != null)
                                    {
                                        material = "Reinforcement";
                                        weight = Double.Parse(values[11].Replace('.', '.'), CultureInfo.InvariantCulture) * length;
                                        quality = "";

                                        Column columnReinforcement = new Column(typeID, material, quality, volume, weight);
                                        columns.AddColumn(columnReinforcement);

                                    }

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

                                    Column column = new Column(typeID, material, quality, volume, weight);
                                    columns.AddColumn(column);

                                    if (values[11] != "" & values[11] != null)
                                    {
                                        material = "Reinforcement";
                                        weight = Double.Parse(values[11].Replace('.', '.'), CultureInfo.InvariantCulture) * length;
                                        quality = "";

                                        Column columnReinforcement = new Column(typeID, material, quality, volume, weight);
                                        columns.AddColumn(columnReinforcement);

                                    }

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

                                    if (values[11] != "" & values[11] != null)
                                    {
                                        material = "Reinforcement";
                                        weight = Double.Parse(values[11].Replace('.', '.'), CultureInfo.InvariantCulture) * area;
                                        quality = "";
                                        Deck deckReinforcement = new Deck(typeID, material, quality, area, thickness, weight);
                                        decks.AddDeck(deckReinforcement);

                                    }
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

                                    if (values[11] != "" & values[11] != null)
                                    {
                                        material = "Reinforcement";
                                        weight = Double.Parse(values[11].Replace('.', '.'), CultureInfo.InvariantCulture) * area;
                                        quality = "";
                                        Wall wallReinforcement = new Wall(typeID, material, quality, area, thickness, weight);
                                        walls.AddWall(wallReinforcement);

                                    }

                                }
                                else if (values[1] == "Foundation slab" || values[1] == "Wall foundation" || values[1] == "Isolated foundation" & line != "")
                                {
                                    string typeID = values[2];
                                    string material = "Concrete";
                                    string quality = values[3];
                                    string volumeString = values[8];
                                    double volume = Double.Parse(volumeString.Replace('.', '.'), CultureInfo.InvariantCulture);
                                    string weightString = values[9];
                                    double weight = Double.Parse(weightString.Replace('.', '.'), CultureInfo.InvariantCulture);

                                    Foundation foundation = new Foundation(typeID, material, quality, volume, weight);
                                    foundations.AddFoundation(foundation);

                                    if (values[11] != "" & values[11] != null)
                                    {
                                        material = "Reinforcement";
                                        weight = Double.Parse(values[11].Replace('.', '.'), CultureInfo.InvariantCulture) * Double.Parse(values[7].Replace('.', '.'), CultureInfo.InvariantCulture);
                                        quality = "";

                                        Foundation foundationReinforcement = new Foundation(typeID, material, quality, volume, weight);
                                        foundations.AddFoundation(foundationReinforcement);

                                    }

                                }
                                counter++;
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
                            if (values[0] != "Storey" & values[0] != "" & values[0] != "TOTAL" & line != "")
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

                                    Beam beam = new Beam(typeID, material, quality, volume, weight);
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

                                    Column column = new Column(typeID, material, quality, volume, weight);
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

                                    Column column = new Column(typeID, material, quality, volume, weight);
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
                            if (values[0] != "Storey" & values[0] != "" & values[0] != "TOTAL" & line != "")
                            {
                                if (values[1] == "Beam" & line != "")
                                {
                                    string typeID = values[2];
                                    string quality = values[3];
                                    string material = "Timber";
                                    string lengthString = values[6];
                                    double length = Double.Parse(lengthString.Replace('.', '.'), CultureInfo.InvariantCulture);
                                    string volumeString = values[4];
                                    string[] volumeStringSplitted = volumeString.Split(' ', 'x');
                                    double volume = mmTomConverter.Convert(Convert.ToDouble(volumeStringSplitted[1])) * mmTomConverter.Convert(Convert.ToDouble(volumeStringSplitted[2])) * length;
                                    string weightString = values[7];
                                    double weight = Double.Parse(weightString.Replace('.', '.'), CultureInfo.InvariantCulture);

                                    Beam beam = new Beam(typeID, material, quality, volume, weight);
                                    beams.AddBeam(beam);

                                }
                                else if (values[1] == "Column" & line != "")
                                {
                                    string typeID = values[2];
                                    string material = "Timber";
                                    string quality = values[3];
                                    string lengthString = values[6];
                                    double length = Double.Parse(lengthString.Replace('.', '.'), CultureInfo.InvariantCulture);
                                    string volumeString = values[4];
                                    string[] volumeStringSplitted = volumeString.Split(' ', 'x');
                                    double volume = mmTomConverter.Convert(Convert.ToDouble(volumeStringSplitted[1])) * mmTomConverter.Convert(Convert.ToDouble(volumeStringSplitted[2])) * length;

                                    string weightString = values[7];
                                    double weight = Double.Parse(weightString.Replace('.', '.'), CultureInfo.InvariantCulture);

                                    Column column = new Column(typeID, material, quality, volume, weight);
                                    columns.AddColumn(column);

                                }
                                else if (values[1] == "Truss" & line != "")
                                {
                                    string typeID = values[2];
                                    string quality = values[3];
                                    string material = "Timber";
                                    string lengthString = values[6];
                                    double length = Double.Parse(lengthString.Replace('.', '.'), CultureInfo.InvariantCulture);
                                    string volumeString = values[4];
                                    string[] volumeStringSplitted = volumeString.Split(' ', 'x');
                                    double volume = mmTomConverter.Convert(Convert.ToDouble(volumeStringSplitted[1])) * mmTomConverter.Convert(Convert.ToDouble(volumeStringSplitted[2])) * length;

                                    string weightString = values[7];
                                    double weight = Double.Parse(weightString.Replace('.', '.'), CultureInfo.InvariantCulture);

                                    Column column = new Column(typeID, material, quality, volume, weight);
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
                            if (values[0] != "Storey" & values[0] != "" & values[0] != "TOTAL" & line != "")
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
                            if (values[0] != "Storey" & values[0] != "" & values[0] != "TOTAL" & line != "")
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
            structuralElements.Add("Foundation", foundations.FoundationsInModel);

            // Lav breakpoint og kopier JSON filen.
            JsonConvert.SerializeObject(structuralElements, (Formatting)1);

            File.WriteAllText(@"C:\Users\camil\OneDrive\OneDrive_Privat\OneDrive\Bygningsdesign kandidat\Speciale\femdesign-api\Quantities\Structuralelements_Json", JsonConvert.SerializeObject(structuralElements));
            
        }

    }
}
    