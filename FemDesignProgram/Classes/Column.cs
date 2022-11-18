using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FemDesignProgram.Containers
{
    public class Column
    {
        // Declaring variables
        public string TypeID { get; set; }
        public string Material { get; set; }
        public string Quality { get; set; }
        public double Length { get; set; }

        public double Volume { get; set; }

        public double Weight { get; set; }

        //public Dictionary<string, string> MaterialProperties { get; set; } = new Dictionary<string, string>();

        // Constructor: Starts with lower case letter
        public Column(string typeID, string material,string quality, double length, double volume, double weight)
        {
            TypeID = typeID;
            Material = material;
            Quality = quality;
            Length = length;
            Volume = volume;
            Weight = weight;

        }


    }
}
