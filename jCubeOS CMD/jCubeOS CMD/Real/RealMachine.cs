using jCubeOS_CMD.Virtual;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
            ChannelTool.SetProcessor(Processor);
            VirtualMemory = null;
            Pager = null;
        }

        public void LoadVirtualMachine(string filePath)
        {
            //Get all file to one string
            string uncutTask = ReadTaskFile(filePath);

            //Cut string to blocks words and bytes
            char[][][] taskCutToBlocks = CutToBlocks(uncutTask);
            int taskBlockLengthUnsorted = taskCutToBlocks.Length;

            //Store blocks to supervisor memory
            PutTaskToSupervisorMemory(taskCutToBlocks);

            //Create task program in supervisor memory
            Tuple<int, int> result = CreateTaskProgramInSupervisorMemory(0, taskBlockLengthUnsorted);
            int supervisorAddressSorted = result.Item1;
            int taskBlockLengthSorted = result.Item2;

            //Copy task program to external memory and clear supervisor memory
            CopyTaskToExternalMemory(supervisorAddressSorted, taskBlockLengthSorted);

            //Create virtual memory
            VirtualMemory = RealMemory.CreateVirtualMemory(Utility.VIRTUAL_MEMORY_BLOCKS);

            Pager = VirtualMemory.GetPager();

            //Move task program from external memory to user / virtual memory
            UploadTaskToVirtualMemory(0, Utility.VIRTUAL_MEMORY_BLOCKS);

            GetProcessor().SetVirtualMemory(VirtualMemory, Pager);

            GetProcessor().SetICRegisterValue(0);
        }

        private string ReadTaskFile(string filePath)
        {
            string singleString = string.Empty;
            if (File.Exists(filePath))
            {
                foreach (string line in File.ReadAllLines(filePath))
                {
                    if (line.Length == 0) continue;
                    else singleString += line.AddEndLine();
                }
                return singleString;
            }
            else throw new Exception("File path is incorrect or is being used by another process.");
        }

        private char[][][] CutToBlocks(string uncutTask)
        {
            int blocks = (int)Math.Ceiling(((float)uncutTask.Length) / (Utility.BLOCK_SIZE * Utility.WORD_SIZE));
            //block; word; byte;
            char[][][] taskInBlocks = new char[blocks][][];
            //If index does not exceed given string length cuts it to symbols (chars) and puts into block, else adds empty ' ' space
            for (int b = 0, i = 0; b < blocks; b++)
            {
                taskInBlocks[b] = new char[Utility.BLOCK_SIZE][];
                for (int c = 0; c < Utility.BLOCK_SIZE; c++)
                {
                    taskInBlocks[b][c] = new char[Utility.WORD_SIZE];
                    for (int bt = 0; bt < Utility.WORD_SIZE; bt++, i++)
                    {
                        taskInBlocks[b][c][bt] = ((i < uncutTask.Length) ? uncutTask[i] : ' ');
                    }
                }
            }
            return taskInBlocks;
        }

        private void PutTaskToSupervisorMemory(char[][][] blocks) => RealMemory.PutDataToSupervisorMemory(blocks, 0);

        private void PutTaskProgramToSupervisorMemory(char[][][] blocks, int supervisorAddress) => RealMemory.PutDataToSupervisorMemory(blocks, supervisorAddress);

        private void CleanTaskMemoryFromSupervisorMemory(int supervisorAddress, int blockCount) => RealMemory.CleanDataFromSupervisorMemory(supervisorAddress, blockCount);

        private Tuple<int, int> CreateTaskProgramInSupervisorMemory(int supervisorAddress, int blockSize)
        {

            string oneLine = string.Empty;

            for (int i = 0; i < blockSize; i++)
            {
                char[][] block = RealMemory.GetSupervisorMemoryBlockValues(supervisorAddress + i * Utility.BLOCK_SIZE);
                oneLine += block.CharsToString();
            }

            string[] lines = oneLine.Split('\n');

            var segments = SegmentTask(lines);
            List<string> code = segments.Item1;
            List<string> data = segments.Item2;

            char[][] cleanCode = CleanCode(code);
            int dataStartBlock = Utility.VIRTUAL_MEMORY_BLOCKS / 2;
            char[][] cleanData = CleanData(data, ref dataStartBlock);

            char[][][] taskMemory = CreateTaskMemory(cleanCode, cleanData, dataStartBlock);

            PutTaskProgramToSupervisorMemory(taskMemory, supervisorAddress + blockSize * Utility.BLOCK_SIZE);

            CleanTaskMemoryFromSupervisorMemory(supervisorAddress, blockSize);

            return Tuple.Create(supervisorAddress + blockSize * Utility.BLOCK_SIZE, Utility.VIRTUAL_MEMORY_BLOCKS);
        }

        private Tuple<List<string>, List<string>> SegmentTask(string[] lines)
        {
            bool codeSegment = false;
            bool dataSegment = false;

            bool codeDone = false;
            bool dataDone = false;

            List<string> code = new List<string>();
            List<string> data = new List<string>();

            for (int ii = 0; ii < lines.Length; ii++)
            {
                if (!codeSegment && !dataSegment && lines[ii] == "$CODE")
                {
                    if (codeDone) throw new Exception("Repetetive $CODE segments.");

                    codeSegment = true;
                    continue;
                }
                else if (codeSegment && !dataSegment && lines[ii] == ("$DATA"))
                {
                    if (dataDone) throw new Exception("Repetetive $DATA segments.");

                    codeSegment = false;
                    codeDone = true;

                    dataSegment = true;
                    continue;
                }
                else if (!codeSegment && dataSegment && lines[ii] == "$END")
                {
                    dataDone = true;
                    dataSegment = false;
                    break;
                }
                else if (!codeSegment && !dataSegment && lines[ii] == "") continue;

                if (codeSegment) code.Add(lines[ii]);
                else if (dataSegment) data.Add(lines[ii]);
                else throw new Exception("Undefined task line");
            }
            return Tuple.Create(code, data);
        }

        private char[][] CleanCode(List<string> code)
        {
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
                else command = code[i];
                cleanCode.Add(command.RemoveWhiteSpaces());
            }

            char[][] codeWords = new char[cleanCode.Count()][];

            for (int i = 0; i < cleanCode.Count(); i++)
            {
                string command = cleanCode[i];
                if (command.Contains('$'))
                {
                    string label = command.Split('$')[1].RemoveWhiteSpaces();
                    if (!labels.ContainsKey(label)) throw new Exception("There is no label " + label + " specified in the task file.");
                    string labelHexValue = new string((labels[label]).IntToHex(2));
                    string labelWithMark = String.Format("${0}", label);
                    command = command.Replace(labelWithMark, labelHexValue);
                }
                command = command.AddWhiteSpacesToSize(Utility.WORD_SIZE);
                codeWords[i] = command.StringToChars();
            }
            return codeWords;
        }

        private char[][] CleanData(List<string> data, ref int startBlock)
        {
            Dictionary<int, string> cleanData = new Dictionary<int, string>();

            string pattern = @"^\[(\w+)\]:";
            Regex regex = new Regex(pattern);

            int address = 0;

            for (int i = 0; i < data.Count(); i++)
            {
                string line = data[i];
                Match result = regex.Match(line);
                if (result.Success)
                {
                    string tag = result.Groups[0].Value;
                    string hexAddress = tag.Split('[')[1].Split(']')[0];
                    address = Utility.HexToInt(hexAddress.ToCharArray());
                    line = line.Replace(tag, "");
                }
                int roundToWord = line.Length + Utility.WORD_SIZE - line.Length % Utility.WORD_SIZE;
                line = line.AddWhiteSpacesToSize(roundToWord);
                cleanData.Add(address, line);
                address += line.Length;
            }

            bool first = true;
            int lastAddress = 0;
            int endBlock = (int)Math.Ceiling(((float)address) / Utility.BLOCK_SIZE);
            char[][] dataWords = null;
            int startBlockAddress = startBlock * Utility.BLOCK_SIZE;
            foreach (int k in cleanData.Keys)
            {
                if (first)
                {
                    startBlock = (int)Math.Floor(((float)k) / Utility.BLOCK_SIZE);
                    startBlockAddress = startBlock * Utility.BLOCK_SIZE;
                    first = false;
                    lastAddress = startBlockAddress;
                    dataWords = new char[(endBlock - startBlock) * Utility.BLOCK_SIZE][];
                }
                //Adding ' ' at the beginning of the block
                for (int i = lastAddress - startBlockAddress; i < k - startBlockAddress; i++)
                {
                    dataWords[i] = new char[Utility.WORD_SIZE];
                    for (int ii = 0; ii < Utility.WORD_SIZE; ii++) dataWords[i][ii] = ' ';
                }
                //Adding clean data to data words
                for (int i = k - startBlockAddress, d = 0; d < cleanData[k].Length; i++)
                {
                    dataWords[i] = new char[Utility.WORD_SIZE];
                    for (int ii = 0; ii < Utility.WORD_SIZE; d++, ii++) dataWords[i][ii] = cleanData[k][d];
                }
                lastAddress = k + cleanData[k].Length / Utility.WORD_SIZE;
            }
            for (int i = lastAddress - startBlockAddress; i < endBlock * Utility.BLOCK_SIZE - startBlockAddress; i++)
            {
                dataWords[i] = new char[Utility.WORD_SIZE];
                for (int ii = 0; ii < Utility.WORD_SIZE; ii++) dataWords[i][ii] = ' ';
            }

            return dataWords;
        }

        private char[][][] CreateTaskMemory(char[][] code, char[][] data, int dataStartBlock)
        {
            char[][][] taskMemory = new char[Utility.VIRTUAL_MEMORY_BLOCKS][][];
            for (int b = 0, cc = 0, dd = 0; b < Utility.VIRTUAL_MEMORY_BLOCKS; b++)
            {
                taskMemory[b] = new char[Utility.BLOCK_SIZE][];
                {
                    for (int c = 0; c < Utility.BLOCK_SIZE; c++)
                    {
                        if (b >= dataStartBlock && dd < data.Length)
                        {
                            taskMemory[b][c] = data[dd];
                            dd++;
                        }
                        else if (cc < code.Length)
                        {
                            taskMemory[b][c] = code[cc];
                            cc++;
                        }
                        else
                        {
                            taskMemory[b][c] = new char[Utility.WORD_SIZE];
                            for (int i = 0; i < Utility.WORD_SIZE; i++) taskMemory[b][c][i] = ' ';
                        }
                    }
                }
            }
            return taskMemory;
        }

        private void CopyTaskToExternalMemory(int supervisorAddress, int taskBlockLength)
        {
            for (int i = 0; i < taskBlockLength; i++)
            {
                Processor.UseChannelTool(supervisorAddress + i * Utility.BLOCK_SIZE, i * Utility.BLOCK_SIZE, 2, 3);
            }
        }

        private void UploadTaskToVirtualMemory(int externalMemoryAddress, int taskBlockLength)
        {
            for (int i = 0; i < taskBlockLength; i++)
            {
                Processor.UseChannelTool(externalMemoryAddress + i * Utility.BLOCK_SIZE, Pager.GetCellRealAddress(i * Utility.BLOCK_SIZE), 3, 1);
            }
        }

        public Processor GetProcessor() => Processor;

        public RealMemory GetRealMemory() => RealMemory;
    }
}