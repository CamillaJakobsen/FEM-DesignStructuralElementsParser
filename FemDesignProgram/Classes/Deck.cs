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
        public string MaterialID { get; set; }
        public double Area { get; set; }
        public double Thickness { get; set; }
        //public Dictionary<string, string> MaterialProperties { get; set; } = new Dictionary<string, string>();
        // In MaterialProperties is Density, strength, reinforcement ratio etc.


        // Constructor: Starts with lower case letter
        public Deck(string typeID, string materialID, double area, double thickness)
        {
            TypeID = typeID;
            MaterialID = materialID;
            Area = area;
            Thickness = thickness;

        }


    }
}
