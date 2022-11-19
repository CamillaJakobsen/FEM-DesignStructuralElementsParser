using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FemDesignProgram.Containers
{
    public class Deck
    {
        // Declaring variables
        public string TypeID { get; set; }
        public string Material { get; set; }
        public string Quality { get; set; }
        public double Area { get; set; }
        public double Thickness { get; set; }
        public double Weight { get; set; }
        //public Dictionary<string, string> MaterialProperties { get; set; } = new Dictionary<string, string>();
        // In MaterialProperties is Density, strength, reinforcement ratio etc.


        // Constructor: Starts with lower case letter
        public Deck(string typeID, string material, string quallity, double area, double thickness, double weight)
        {
            TypeID = typeID;
            Material = material;
            Quality = quallity;
            Area = area;
            Thickness = thickness;
            Weight = weight;
        }


    }
}
