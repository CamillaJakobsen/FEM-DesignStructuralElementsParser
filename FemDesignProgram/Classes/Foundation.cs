using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FemDesignProgram.Containers
{
    public class Foundation
    {
        // Declaring variables
        public int TypeID { get; set; }
        public string Material { get; set; }
        public string Quality { get; set; }
        public double Volume { get; set; }

       

        // Constructor: Starts with lower case letter
        public Foundation(int typeID, string material, string quality, double volume)
        {
            TypeID = typeID;
            Material = material;
            Quality = quality;
            Volume = volume;


        }

    }
}
