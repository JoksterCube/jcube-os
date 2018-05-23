using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace jCubeOS_CMD.Real
{
    class ExternalMemory
    {
        private static readonly string externalMemoryFilePath = "external.memory";

        private Cell[][] Cells { get; set; }

        public ExternalMemory()
        {
            Cells = new Cell[Utility.EXTERNAL_MEMORY_BLOCKS][];
            for (int i = 0; i < Utility.EXTERNAL_MEMORY_BLOCKS; i++)
            {
                Cells[i] = new Cell[Utility.BLOCK_SIZE];
                for (int ii = 0; ii < Utility.BLOCK_SIZE; ii++)
                {
                    Cells[i][ii] = new Cell();
                }
            }

            UpdateExternalMemory();
        }

        public void PutData(int address, char[][] data)
        {
            var addressTuple = Utility.GetAdressTuple(address);
            int block = addressTuple.Item1;
            int cell = addressTuple.Item2;

            for (int i = 0, c = cell; i < data.Length; i++, c++)
            {
                if (c >= Utility.BLOCK_SIZE)
                {
                    Console.WriteLine("Data does not fit to the block");
                }
                Cells[block][c].SetValue(data[i]);
            }

            UpdateExternalMemory();
        }

        private Cell GetCell(int address)
        {
            var addressTuple = Utility.GetAdressTuple(address);
            return Cells[addressTuple.Item1][addressTuple.Item2];
        }

        public char[] GetValue(int address)
        {
            return GetCell(address).GetValue();
        }
        
        public void SetValue(int address, char[] value)
        {
            GetCell(address).SetValue(value);
            UpdateExternalMemory();
        }

        private void UpdateExternalMemory()
        {
            string[] memoryLines = new string[Cells.Length];
            for (int i = 0; i < Cells.Length; i++)
            {
                StringBuilder sb = new StringBuilder();
                for (int ii = 0; ii < Cells[i].Length; ii++)
                {
                    for(int iii = 0; iii < Cells[i][ii].GetValue().Length; iii++)
                    {
                        sb.Append(Cells[i][ii].GetValue()[iii]);
                    }
                    sb.Append('\t');
                }
                memoryLines[i] = sb.ToString();
            }
            File.WriteAllLines(externalMemoryFilePath, memoryLines);
        }
    }
}
