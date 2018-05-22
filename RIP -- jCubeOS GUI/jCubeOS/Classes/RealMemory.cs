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
            return Tuple.Create(block, cell);
        }

        /// <returns>Returns memory cell at this address</returns>
        public Cell GetCell(int address)
        {
            var addressTuple = GetAdressTuple(address);
            return Cells[addressTuple.Item1][addressTuple.Item2];
        }

        /// <returns>Returns memory cell value of this address</returns>
        public byte[] GetValue(int address)
        {
            return GetCell(address).GetValue();
        }

        public string GetStringValue(int address)
        {
            return GetCell(address).GetStringValue();
        }

        public void SetValue(int address, string value)
        {
            GetCell(address).SetValue(value);
        }

        /// <summary>
        /// Allocating virtual memory
        /// </summary>
        public Tuple<VirtualMemoryCode, VirtualMemoryData> CreateVirtualMemory(List<string> code, int codeSize, List<string> data, int dataSize, Input inputHandler = null, Output outputHandler = null)
        {
            Pager pager = new Pager(this, C: codeSize, D: dataSize, inputHandler: inputHandler, outputHandler: outputHandler);

            VirtualMemoryCode virtualMemoryCode = new VirtualMemoryCode(this, pager);
            Dictionary<string, int> labels = new Dictionary<string, int>();
            List<string> cleanCode = new List<string>();

            for (int i = 0; i < code.Count(); i++)
            {
                string command = string.Empty;
                if (code[i].Contains(':'))
                {
                    string[] line = code[i].Split(':');
                    labels.Add(line[0].RemoveWhiteSpaces(), i);
                    command = line[1];
                }
                else
                {
                    command = code[i];
                }
                cleanCode.Add(command.RemoveWhiteSpaces());
            }

            for (int i = 0; i < cleanCode.Count(); i++)
            {
                string command = cleanCode[i];
                if (command.Contains('$')){
                    string label = command.Split('$')[1].RemoveWhiteSpaces();
                    string labelHexValue = Utility.IntToHex(labels[label], 2);
                    string labelWithMark = String.Format("${0}", label);
                    command = command.Replace(labelWithMark, labelHexValue);
                }
                command += ' ' * (Utility.WORD_SIZE - command.Length);
                virtualMemoryCode.SetCodeValue(i, command);
            }

            VirtualMemoryData virtualMemoryData = new VirtualMemoryData(this, pager);


            return Tuple.Create((VirtualMemoryCode)null, (VirtualMemoryData)null);
        }
    }
}
