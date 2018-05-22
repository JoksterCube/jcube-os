using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jCubeOS.Classes
{
    class VirtualMemoryData
    {
        private RealMemory Memory { get; set; }
        private Pager Pager { get; set; }

        public VirtualMemoryData(RealMemory memory, Pager pager)
        {
            Memory = memory;
            Pager = pager;
        }

    }
}
