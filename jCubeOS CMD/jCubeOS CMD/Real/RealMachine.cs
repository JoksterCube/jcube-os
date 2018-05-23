using jCubeOS_CMD.Virtual;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jCubeOS_CMD.Real
{
    /// <summary>
    /// Real Machine simaling object
    /// </summary>
    class RealMachine
    {
        private RealMemory RealMemory { get; set; }
        private ExternalMemory ExternalMemory { get; set; }
        private ChannelTool ChannelTool { get; set; }
        private Processor Processor { get; set; }
        private Input InputHandler { get; set; }
        private Output OutputHandler { get; set; }

        private VirtualMemory VirtualMemory { get; set; }
        private Pager Pager { get; set; }

        public RealMachine(Input inputHandler = null, Output outputHandler = null)
        {
            RealMemory = new RealMemory();
            ExternalMemory = new ExternalMemory();
            InputHandler = inputHandler;
            OutputHandler = outputHandler;
            ChannelTool = new ChannelTool(RealMemory, ExternalMemory, InputHandler, OutputHandler);
            Processor = new Processor(RealMemory, ChannelTool);
            VirtualMemory = null;
            Pager = null;
        }
        
        public void LoadVirtualMachine(string filePath)
        {
            string errorMessage = string.Empty;

            int codeSize = 0;

            List<string> code = new List<string>();
            List<string> data = new List<string>();

            string[] lines;

            try
            {
                lines = File.ReadAllLines(filePath);
                ReadTaskFile(ref errorMessage, code, data, lines);
            }
            catch(Exception e)
            {
                errorMessage = e.Message;
                lines = new string[0];
            }

            if (errorMessage != string.Empty)
            {
                StopVirtualMachine(errorMessage);
                return;
            }

            

            var virtualMemory = RealMemory.CreateVirtualMemory(code, codeSize, data, 0, InputHandler, OutputHandler);
            VirtualMemory = virtualMemory.Item1;
            Pager = virtualMemory.Item2;

            Processor.SetHexRegisterValue("IC", 100);

        }

        private void StopVirtualMachine(string error) => Console.WriteLine("Virtual machine was stoped due to: " + error);

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
