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
        public string MaterialID { get; set; }
        public double Volume { get; set; }

        // Key value pairs indgår i Dictionary
        public Dictionary<string, string> MaterialProperties { get; set; } = new Dictionary<string, string>();

        // Constructor: Starts with lower case letter
        public Foundation(int typeID, string materialID, double volume)
        {
            TypeID = typeID;
            MaterialID = materialID;
            Volume = volume;


        }

    }
}
