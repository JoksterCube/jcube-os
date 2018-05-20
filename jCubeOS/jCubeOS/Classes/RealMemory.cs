using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jCubeOS.Classes
{
    /// <summary>
    /// Real machine memory
    /// </summary>
    class RealMemory
    {
        private Cell[][] Cells { get; set; }

        /// <summary>
        /// Initializing Empty memory
        /// </summary>
        public RealMemory()
        {
            Cells = new Cell[Utility.BLOCKS][];
            for (int i = 0; i < Utility.BLOCKS; i++)
            {
                Cells[i] = new Cell[Utility.BLOCK_SIZE];
                for (int ii = 0; ii < Utility.BLOCK_SIZE; ii++)
                {
                    Cells[i][ii] = new Cell();
                }
            }
        }
        
        /// <returns>Returns block and block element addresses</returns>
        public Tuple<int, int> GetAdressTuple(int address)
        {
            int block = address / Utility.BLOCK_SIZE;
            int cell = address % Utility.BLOCK_SIZE;
            return Tuple.Create(block,cell);
        }
        
        /// <returns>Returns memory cell at this address</returns>
        public Cell GetCell(int address)
        {
            var addressTuple = GetAdressTuple(address);
            return Cells[addressTuple.Item1][addressTuple.Item2];
        }
        
        /// <returns>Returns memory cell address of this address</returns>
        public byte[] GetValue(int address)
        {
            return GetCell(address).GetValue();
        }

        /// <summary>
        /// Allocating virtual memory
        /// </summary>
        public Tuple<VirtualMemoryCode, VirtualMemoryData> CreateVirtualMemory(List<string> code, int codeSize, List<string> data, int dataSize, Input inputHandler = null, Output outputHandler = null)
        {
            Pager pager = new Pager();



            return Tuple.Create((VirtualMemoryCode)null, (VirtualMemoryData)null);
        }
    }
}
