using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FemDesignProgram.Containers
{
    public class Walls
    {
        public List<object> WallsInModel = new List<object>();

        public void AddWall(Wall wall)
        {
            WallsInModel.Add(wall);
        }
    }
}
