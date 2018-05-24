using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jCubeOS_CMD.Real
{
    class Pager
    {
        RealMemory RealMemory { get; set; }
        private int PTR { get; set; }

        public Pager(RealMemory realMemory, int pagerAddress = 0)
        {
            RealMemory = realMemory;
            PTR = 0;
        }

        public int GetCellRealAddress(int virtualAddress)
        {
            var addressTuple = Utility.GetAddressTuple(virtualAddress);
            int virtualBlock = addressTuple.Item1;
            int cell = addressTuple.Item2;
            int realBlock = RealMemory.GetUserMemoryValue(PTR * Utility.BLOCK_SIZE + virtualBlock).HexToInt();
            return realBlock * Utility.BLOCK_SIZE + cell;
        }

        public void SetPTR(int value) => PTR = value;

        public int GetPTR() => PTR;

        public void AddBlock(int index, int blockRealAddress) => RealMemory.SetUserMemoryValue(PTR * Utility.BLOCK_SIZE + index, blockRealAddress.IntToHex());
    }
}
