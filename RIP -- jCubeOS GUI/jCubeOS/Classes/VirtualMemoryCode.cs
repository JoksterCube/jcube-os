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

        public string GetCodeValue(int address)
        {
            return Memory.GetStringValue(Pager.GetCodeCellAddress(address));
        }

        public void SetCodeValue(int address, string value)
        {
            Memory.SetValue(Pager.GetCodeCellAddress(address), value);
        }
    }
}
