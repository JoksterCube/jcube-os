using jCubeOS_CMD;
using jCubeOS_CMD.Real;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jCubeOS_CMD.Virtual
{
    class VirtualMemory
    {
        private RealMemory Memory { get; set; }
        private Pager Pager { get; set; }

        public VirtualMemory(RealMemory memory, Pager pager)
        {
            Memory = memory;
            Pager = pager;
        }

        public char[] GetValue(int virtualAddress) => Memory.GetUserMemoryValue(Pager.GetCellRealAddress(virtualAddress));

        public char[][] GetBlockValues(int virtualBlockAddress) => Memory.GetUserMemoryBlockValues(Pager.GetCellRealAddress(virtualBlockAddress));

        public void SetValue(int virtualAddress, char[] value) => Memory.SetUserMemoryValue(Pager.GetCellRealAddress(virtualAddress), value);

        public void SetBlockValues(int virtualBlockAddress, char[][] values) => Memory.SetUserMemoryBlockValues(Pager.GetCellRealAddress(virtualBlockAddress), values);

        public Pager GetPager() => Pager;

        public void PrintVirtualMemory()
        {
            Console.WriteLine("--------------------------------VIRTUAL MEMORY---------------------------------------");
            Console.Write("    |");
            for (int i = 0; i < Utility.VIRTUAL_MEMORY_BLOCKS; i++) Console.Write(" " + new string(i.IntToHex(2)) + " |");
            Console.Write("\n-------------------------------------------------------------------------------------\n");
            for (int i = 0; i < Utility.VIRTUAL_MEMORY_BLOCKS; i++)
            {
                string block = " " + new string(i.IntToHex(2)) + " |";
                for (int ii = 0; ii < Utility.BLOCK_SIZE; ii++) block += new string(GetValue(i * Utility.BLOCK_SIZE + ii)) + "|";
                Console.WriteLine(block.Replace('\n', 'n'));
            }
            Console.WriteLine("--------------------------------VIRTUAL MEMORY---------------------------------------");
        }
    }
}
