using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FemDesignProgram.Containers
{
    public class Beam
    {
        // Declaring variables
        public string TypeID { get; set; }
        public string MaterialID { get; set; }
        public double Length { get; set; }

        public double Volume { get; set; }

        public double Weight { get; set; }
        //public Dictionary<string, string> MaterialProperties { get; set; } = new Dictionary<string, string>();

        // Constructor: Starts with lower case letter
        public Beam(string typeID, string materialID, double length, double volume, double weight)
        {
            TypeID = typeID;
            MaterialID = materialID;
            Length = length;
            Volume = volume;
            Weight = weight;

        }


    }
}
