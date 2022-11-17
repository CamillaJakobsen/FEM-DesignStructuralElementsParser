using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FemDesignProgram.Containers
{
    public class Columns
    {
        public List<object> ColumnsInModel = new List<object>();

        public void AddColumn(Column column)
        {
            ColumnsInModel.Add(column);
        }

    }
}
