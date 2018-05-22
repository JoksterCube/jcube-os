using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jCubeOS.Classes
{
    /// <summary>
    /// Real Machine simaling object
    /// </summary>
    class RealMachine
    {
        private RealMemory RealMemory { get; set; }
        private Processor Processor { get; set; }
        private VirtualMemoryCode VirtualMemoryCode { get; set; }
        private VirtualMemoryData VirtualMemoryData { get; set; }

        public RealMachine()
        {
            RealMemory = new RealMemory();
            Processor = new Processor(RealMemory);
            VirtualMemoryCode = null;
            VirtualMemoryData = null;
        }

        /// <summary>
        /// Loads virtual machine
        /// </summary>
        public void LoadVirtualMachine(string filePath, Input inputHandler = null, Output outputHandler = null)
        {
            string errorMessage = string.Empty;

            int codeSize = 0;
            int dataSize = 0;

            List<string> code = new List<string>();
            List<string> data = new List<string>();

            string[] lines;

            try
            {
                lines = File.ReadAllLines(filePath);
            }
            catch
            {
                errorMessage = "File was not read correctly. Path might be incorrect.";
                lines = new string[0];
            }

            ReadTaskFile(ref errorMessage, ref dataSize, code, data, lines);
            codeSize = (int)(Math.Ceiling((double)(code.Count()) / Utility.BLOCK_SIZE));

            var virtualMemory = RealMemory.CreateVirtualMemory(code, codeSize, data, dataSize, inputHandler, outputHandler);
            VirtualMemoryCode = virtualMemory.Item1;
            VirtualMemoryData = virtualMemory.Item2;
            
            Processor.

        }

        private static void ReadTaskFile(ref string errorMessage, ref int dataSize, List<string> code, List<string> data, string[] lines)
        {
            bool codeSegment = false;
            bool dataSegment = false;

            bool codeDone = false;
            bool dataDone = false;

            for (int i = 0; i < lines.Length; i++)
            {
                if (!codeSegment && !dataSegment && lines[i] == "CODE\n")
                {
                    if (codeDone)
                    {
                        errorMessage = "Repetetive CODE segments.";
                        break;
                    }

                    codeSegment = true;
                    continue;
                }
                else if (codeSegment && !dataSegment && lines[i] == "ENDCODE\n")
                {
                    codeSegment = false;
                    codeDone = true;
                    continue;
                }
                else if (!codeSegment && dataSegment && lines[i].StartsWith("DATA "))
                {
                    if (dataDone)
                    {
                        errorMessage = "Repetetive DATA segments.";
                        break;
                    }

                    dataSegment = true;
                    if (!Int32.TryParse(lines[i].Substring(5), out dataSize))
                    {
                        errorMessage = "DATA segment size is defined incorrectly.";
                        break;
                    }
                    continue;
                }
                else if (!codeSegment && dataSegment && lines[i] == "$END\n")
                {
                    dataDone = true;
                    dataSegment = false;
                    continue;
                }

                if (codeSegment)
                {
                    code.Add(lines[i]);
                }
                else if (dataSegment)
                {
                    data.Add(lines[i]);
                }
            }
        }

        public Processor GetProcessor()
        {
            return Processor;
        }
    }
}
