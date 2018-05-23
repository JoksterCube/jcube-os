using jCubeOS_CMD.Virtual;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jCubeOS_CMD.Real
{
    class RealMemory
    {
        private Cell[][] UserMemoryCells { get; set; }
        private Cell[][] SupervisorMemoryCells { get; set; }
        
        public RealMemory()
        {
            UserMemoryCells = new Cell[Utility.BLOCKS - Utility.SUPERVISOR_MEMORY_BLOCKS][];
            for (int i = 0; i < Utility.BLOCKS - Utility.SUPERVISOR_MEMORY_BLOCKS; i++)
            {
                UserMemoryCells[i] = new Cell[Utility.BLOCK_SIZE];
                for (int ii = 0; ii < Utility.BLOCK_SIZE; ii++)
                {
                    UserMemoryCells[i][ii] = new Cell();
                }
            }

            SupervisorMemoryCells = new Cell[Utility.SUPERVISOR_MEMORY_BLOCKS][];
            for (int i = 0; i < Utility.SUPERVISOR_MEMORY_BLOCKS; i++)
            {
                SupervisorMemoryCells[i] = new Cell[Utility.BLOCK_SIZE];
                for (int ii = 0; ii < Utility.BLOCK_SIZE; ii++)
                {
                    SupervisorMemoryCells[i][ii] = new Cell();
                }
            }
        }

        public Cell GetUserMemoryCell(int address)
        {
            var addressTuple = Utility.GetAdressTuple(address);
            return UserMemoryCells[addressTuple.Item1][addressTuple.Item2];
        }

        public char[] GetUserMemoryValue(int address)
        {
            return GetUserMemoryCell(address).GetValue();
        }

        public void SetUserMemoryValue(int address, char[] value)
        {
            GetUserMemoryCell(address).SetValue(value);
        }

        public Tuple<VirtualMemory, Pager> CreateVirtualMemory(List<string> code, int codeSize, List<string> data, int dataSize, Input inputHandler = null, Output outputHandler = null)
        {
            Pager pager = new Pager(this, C: codeSize, D: dataSize, inputHandler: inputHandler, outputHandler: outputHandler);

            VirtualMemory VirtualMemory = new VirtualMemory(this, pager);
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
                if (command.Contains('$'))
                {
                    string label = command.Split('$')[1].RemoveWhiteSpaces();
                    //string labelHexValue = Utility.IntToHex(labels[label], 2);
                    string labelWithMark = String.Format("${0}", label);
                    //command = command.Replace(labelWithMark, labelHexValue);
                }
                command = command.AddWhiteSpacesToSize(Utility.WORD_SIZE);
                //VirtualMemory.SetValue(i, command);
            }


            return Tuple.Create((VirtualMemory)null, (Pager)null);
        }
    }
}
