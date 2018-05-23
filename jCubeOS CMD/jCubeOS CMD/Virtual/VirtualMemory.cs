using jCubeOS_CMD;
using jCubeOS_CMD.Real;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jCubeOS_CMD.Virtual
{
    /// <summary>
    /// Virtual memory code segment
    /// </summary>
    class VirtualMemory
    {
        private RealMemory Memory { get; set; }
        private Pager Pager { get; set; }

        public VirtualMemory(RealMemory memory, Pager pager)
        {
            Memory = memory;
            Pager = pager;
        }

        public char[] GetValue(int address)
        {
            return Memory.GetUserMemoryValue(Pager.GetCodeCellAddress(address));
        }

        public void SetValue(int address, char[] value)
        {
            Memory.SetUserMemoryValue(Pager.GetCodeCellAddress(address), value);
        }
    }
}
