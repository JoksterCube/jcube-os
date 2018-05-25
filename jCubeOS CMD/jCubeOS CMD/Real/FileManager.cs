using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace jCubeOS_CMD.Real
{
    class FileManager
    {
        private RealMemory RealMemory { get; set; }
        private Processor Processor { get; set; }

        private int[] RealFileMemoryAddresses { get; set; }
        private FileStream[] OpenFileStreams { get; set; }

        private Dictionary<int, StreamReader> FileReaders;
        private Dictionary<int, StreamWriter> FileWriters;

        public FileManager(RealMemory realMemory, Processor processor)
        {
            RealMemory = realMemory;
            Processor = processor;
            RealFileMemoryAddresses = new int[Utility.FILE_MANAGER_BLOCKS];
            OpenFileStreams = new FileStream[Utility.FILE_MANAGER_BLOCKS];
            GetBlocks();
            FileReaders = new Dictionary<int, StreamReader>();
            FileWriters = new Dictionary<int, StreamWriter>();
        }

        private void GetBlocks()
        {
            for (int i = 0; i < Utility.FILE_MANAGER_BLOCKS; i++)
            {
                int blockIndex = (Utility.BLOCKS - Utility.FILE_MANAGER_BLOCKS + i);
                RealFileMemoryAddresses[i] = blockIndex * Utility.BLOCK_SIZE;
                RealMemory.TakeMemoryBlock(blockIndex);
                CleanBlock(RealFileMemoryAddresses[i]);
            }
        }

        private void CleanBlock(int memoryAddress)
        {
            var address = Utility.GetAddressTuple(memoryAddress);
            int block = address.Item1;

            char[] emptyValue = string.Empty.ToCharArray().AddWhiteSpacesToSize(Utility.WORD_SIZE);

            for (int i = 0; i < Utility.BLOCK_SIZE; i++) RealMemory.GetMemoryCell(block, i).SetValue(emptyValue);
        }

        private char[][] GetBlockByIndex(int index)
        {
            char[][] block = new char[Utility.BLOCK_SIZE][];
            int blockAddress = Utility.GetAddressTuple(RealFileMemoryAddresses[index]).Item1;
            for (int i = 0; i < Utility.BLOCK_SIZE; i++) block[i] = RealMemory.GetMemoryCell(blockAddress, i).GetValue();
            return block;
        }

        private void SetBlockByIndex(int index, char[][] block)
        {
            int blockAddress = Utility.GetAddressTuple(RealFileMemoryAddresses[index]).Item1;
            for (int i = 0; i < Utility.BLOCK_SIZE; i++) RealMemory.GetMemoryCell(blockAddress, i).SetValue(block[i]);
        }

        private int GetEmptyBlockIndex()
        {
            for (int i = 0; i < RealFileMemoryAddresses.Length; i++)
            {
                char[] cellValue = GetBlockByIndex(i)[0];
                if (cellValue.IsHex() && cellValue.HexToInt() == 1) continue;
                return i;
            }
            return -1;
        }

        private string ConstructFileName(char[][] fileNameBlock) => fileNameBlock.CharsToString().RemoveWhiteSpaces();

        private bool IsAlreadyOpen(string fileName)
        {
            bool open = false;
            for (int i = 0; i < RealFileMemoryAddresses.Length; i++)
            {
                char[][] block = GetBlockByIndex(i);
                if (block[0].IsHex() && block[0].HexToInt() == 1)
                {
                    block = block.Skip(2).ToArray();
                    string blockString = block.CharsToString().RemoveWhiteSpaces();
                    open |= fileName == blockString;
                }
            }
            return open;
        }

        private void PutBlockToMemory(char[][] fileNameBlock, char openMode, int blockIndex)
        {
            char[][] newBlock = new char[Utility.BLOCK_SIZE][];
            newBlock[0] = Utility.IntToHex(1);
            newBlock[1] = (new char[] { openMode }).AddWhiteSpacesToSize(Utility.WORD_SIZE);

            for (int i = 2; i < Utility.BLOCK_SIZE; i++) newBlock[i] = fileNameBlock[i - 2];
            SetBlockByIndex(blockIndex, newBlock);
        }

        public int OpenFile(char[][] fileNameBlock, char fileOpenMode)
        {
            int emptyBlockIndex = GetEmptyBlockIndex();
            if (emptyBlockIndex == -1) { Processor.SetChoiceRegisterValue("PI", 11); return -1; }
            string fileName = ConstructFileName(fileNameBlock);
            if (fileName.Length >= (Utility.BLOCK_SIZE - 2) * Utility.WORD_SIZE) { Processor.SetChoiceRegisterValue("PI", 12); return -1; }
            if (IsAlreadyOpen(fileName)) { Processor.SetChoiceRegisterValue("PI", 6); return -1; }
            //if (!File.Exists(fileName) || fileOpenMode == 'W') { Processor.SetChoiceRegisterValue("PI", 6); return -1; }
            switch (fileOpenMode)
            {
                case 'R':
                    OpenFileStreams[emptyBlockIndex] = File.OpenRead(fileName);
                    FileReaders.Add(emptyBlockIndex, new StreamReader(OpenFileStreams[emptyBlockIndex]));
                    break;
                case 'W':
                    OpenFileStreams[emptyBlockIndex] = File.OpenWrite(fileName);
                    FileWriters.Add(emptyBlockIndex, new StreamWriter(OpenFileStreams[emptyBlockIndex]));
                    break;
            }
            PutBlockToMemory(fileNameBlock, fileOpenMode, emptyBlockIndex);
            return emptyBlockIndex;
        }

        public void CloseAll()
        {
            for (int i = 0; i < Utility.FILE_MANAGER_BLOCKS; i++) CloseFile(i);
        }

        public bool CloseFile(int index)
        {
            if (index < 0 || index >= Utility.FILE_MANAGER_BLOCKS)
            {
                Processor.SetChoiceRegisterValue("PI", 9);
                return false;
            }
            if (FileReaders.ContainsKey(index))
            {
                FileReaders[index].Close();
                FileReaders[index] = null;
                FileReaders.Remove(index);
                OpenFileStreams[index].Close();
                OpenFileStreams[index] = null;
                CleanBlock(RealFileMemoryAddresses[index]);
                return true;
            }
            else if (FileWriters.ContainsKey(index))
            {
                FileWriters[index].Close();
                FileWriters[index] = null;
                FileWriters.Remove(index);
                OpenFileStreams[index].Close();
                OpenFileStreams[index] = null;
                CleanBlock(RealFileMemoryAddresses[index]);
                return true;
            }
            else
            {
                Processor.SetChoiceRegisterValue("PI", 9);
                return false;
            }
        }

        public char[][] ReadFile(int index, int count = -1)
        {
            if (index < 0 || index >= Utility.FILE_MANAGER_BLOCKS || !FileReaders.ContainsKey(index))
            {
                Processor.SetChoiceRegisterValue("PI", 7);
                return null;
            }
            if (count == -1) count = Utility.BLOCK_SIZE;

            char[][] readBlock = new char[count][];

            for (int i = 0; i < count; i++)
            {
                readBlock[i] = new char[Utility.WORD_SIZE];
                FileReaders[index].Read(readBlock[i], 0, Utility.WORD_SIZE);
            }
            return readBlock;
        }

        public bool WriteFile(int index, char[][] values)
        {
            if (index < 0 || index >= Utility.FILE_MANAGER_BLOCKS || !FileWriters.ContainsKey(index))
            {
                Processor.SetChoiceRegisterValue("PI", 8);
                return false;
            }
            for (int i = 0; i < values.Length; i++) FileWriters[index].Write(values[i], 0, Utility.WORD_SIZE);
            return true;
        }

        public bool DeleteFile(char[][] fileNameBlock)
        {
            string fileName = ConstructFileName(fileNameBlock);
            if (!File.Exists(fileName)) { Processor.SetChoiceRegisterValue("PI", 10); return false; }

            File.Delete(fileName);

            if (File.Exists(fileName)) { Processor.SetChoiceRegisterValue("PI", 10); return false; }
            return true;
        }
    }
}
