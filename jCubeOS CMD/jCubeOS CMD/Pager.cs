using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jCubeOS_CMD
{
    /// <summary>
    /// Helper object for handeling paging mechanism
    /// </summary>
    class Pager
    {


        /// <summary>
        /// 
        /// </summary>
        /// <param name="realMemory"></param>
        /// <param name="address"></param>
        /// <param name="C"></param>
        /// <param name="D"></param>
        /// <param name="inputHandler"></param>
        /// <param name="outputHandler"></param>
        public Pager(RealMemory realMemory, int address = -1, int C = -1, int D = -1, Input inputHandler = null, Output outputHandler = null)
        {

        }

        public int GetCodeCellAddress(int address)
        {
            return 0;
        }
    }
}
