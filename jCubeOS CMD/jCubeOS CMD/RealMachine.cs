using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jCubeOS_CMD
{
    /// <summary>
    /// Real Machine simaling object
    /// </summary>
    class RealMachine
    {
        private RealMemory RealMemory { get; set; }
        private Processor Processor { get; set; }
        private VirtualMemory VirtualMemory { get; set; }
        private Pager Pager { get; set; }

        public RealMachine()
        {
            RealMemory = new RealMemory();
            Processor = new Processor(RealMemory);
            VirtualMemory = null;
        }

        /// <summary>
        /// Loads virtual machine
        /// </summary>
        public void LoadVirtualMachine(string filePath, Input inputHandler = null, Output outputHandler = null)
        {
            string errorMessage = string.Empty;

            int codeSize = 0;

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

            ReadTaskFile(ref errorMessage, code, data, lines);
            //codeSize = (int)(Math.Ceiling((double)(code.Count()) / Utility.BLOCK_SIZE));

            if (errorMessage != string.Empty)
            {
                StopVirtualMachine(errorMessage);
                return;
            }

            var virtualMemory = RealMemory.CreateVirtualMemory(code, codeSize, data, 0, inputHandler, outputHandler);
            VirtualMemory = virtualMemory.Item1;
            Pager = virtualMemory.Item2;

            Processor.SetRegisterValue("IC", 100);

        }

        private void StopVirtualMachine(string error)
        {
            Console.WriteLine("Virtual machine was stoped due to: " + error);
        }

        private static void ReadTaskFile(ref string errorMessage, List<string> code, List<string> data, string[] lines)
        {
            bool codeSegment = false;
            bool dataSegment = false;

            bool codeDone = false;
            bool dataDone = false;

            for (int i = 0; i < lines.Length; i++)
            {
                if (!codeSegment && !dataSegment && lines[i] == "$CODE\n")
                {
                    if (codeDone)
                    {
                        errorMessage = "Repetetive $CODE segments.";
                        break;
                    }

                    codeSegment = true;
                    continue;
                }
                else if (codeSegment && !dataSegment && lines[i] == ("$DATA\n"))
                {
                    if (dataDone)
                    {
                        errorMessage = "Repetetive DATA segments.";
                        break;
                    }

                    codeSegment = false;
                    codeDone = true;

                    dataSegment = true;
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
