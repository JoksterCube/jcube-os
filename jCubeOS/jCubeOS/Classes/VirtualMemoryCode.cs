using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jCubeOS.Classes
{
    /// <summary>
    /// Virtual memory code segment
    /// </summary>
    class VirtualMemoryCode
    {
        private RealMemory Memory { get; set; }
        private Pager Pager { get; set; }

        public VirtualMemoryCode(RealMemory memory, Pager pager)
        {
            Memory = memory;
            Pager = pager;
        }

        public int GetCodeValue(int adress)
        {
            return Memory.GetValue(Pager.GetCodeCellAddress(adress))
        }
    }
}
