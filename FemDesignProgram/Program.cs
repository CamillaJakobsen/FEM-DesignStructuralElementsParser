using StructuralElementsExporter.StructuralAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;




namespace FemDesignProgram
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //new QuantityEstimation().Takeoff();
            new Optimisation().SlabOptimisation();
        }

    }
}
