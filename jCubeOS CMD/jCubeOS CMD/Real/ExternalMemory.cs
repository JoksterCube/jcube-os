using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace jCubeOS_CMD.Real
{
    class ExternalMemory
    {
        private static readonly string externalMemoryFilePath = "external.memory";

        private Block[] Blocks { get; set; }

        public ExternalMemory()
        {
            //Creating all external memory blocks
            Blocks = new Block[Utility.BLOCKS];
            for (int i = 0; i < Utility.BLOCKS; i++)
            {
                //creating all words/cells for individual memory block 
                Blocks[i] = new Block(Utility.BLOCK_SIZE, Utility.WORD_SIZE, false);
            }

            UpdateExternalMemory();
        }

        public void PutData(int address, char[][] data)
        {
            var addressTuple = Utility.GetAddressTuple(address);
            int block = addressTuple.Item1;
            int cell = addressTuple.Item2;

            for (int i = 0, c = cell; i < data.Length; i++, c++)
            {
                if (c < Utility.BLOCK_SIZE) Blocks[block].SetValue(c, data[i]);
                else throw new Exception("Data does not fit to the external memory block.");
            }

            UpdateExternalMemory();
        }

        private Cell GetCell(int address)
        {
            var addressTuple = Utility.GetAddressTuple(address);
            return Blocks[addressTuple.Item1].GetCell(addressTuple.Item2);
        }

        public char[] GetValue(int address) => GetCell(address).GetValue();

        public char[][] GetBlockValues(int blockAddress)
        {
            blockAddress -= blockAddress % Utility.BLOCK_SIZE;

            char[][] block = new char[Utility.BLOCK_SIZE][];
            for (int i = 0; i < Utility.BLOCK_SIZE; i++) block[i] = GetValue(blockAddress + i);
            return block;
        }

        public void SetValue(int address, char[] value)
        {
            GetCell(address).SetValue(value);
            UpdateExternalMemory();
        }

        public void SetBlockValues(int blockAddress, char[][] values)
        {
            blockAddress -= blockAddress % Utility.BLOCK_SIZE;
            for (int i = 0; i < Utility.BLOCK_SIZE; i++) SetValue(blockAddress + i, values[i]);
        }

        private void UpdateExternalMemory()
        {
            string[] memoryLines = new string[Blocks.Length];
            for (int i = 0; i < Blocks.Length; i++)
            {
                StringBuilder sb = new StringBuilder();
                for (int ii = 0; ii < Blocks[i].GetBlockSize(); ii++)
                {
                    for (int iii = 0; iii < Blocks[i].GetValue(ii).Length; iii++) sb.Append(Blocks[i].GetValue(ii)[iii]);
                }
                memoryLines[i] = sb.ToString();
            }
            File.WriteAllLines(externalMemoryFilePath, memoryLines);
        }
    }
}
