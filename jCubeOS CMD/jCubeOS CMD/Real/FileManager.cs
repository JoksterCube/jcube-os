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

        public FileManager(RealMemory realMemory, Processor processor)
        {
            RealMemory = realMemory;
            Processor = processor;
            RealFileMemoryAddresses = new int[Utility.FILE_MANAGER_BLOCKS];
            OpenFileStreams = new FileStream[Utility.FILE_MANAGER_BLOCKS];
            GetBlocks();
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
            if (!File.Exists(fileName)) { Processor.SetChoiceRegisterValue("PI", 6); return -1; }
            switch (fileOpenMode)
            {
                case 'R': OpenFileStreams[emptyBlockIndex] = File.OpenRead(fileName); break;
                case 'W': OpenFileStreams[emptyBlockIndex] = File.OpenWrite(fileName); break;
            }
            PutBlockToMemory(fileNameBlock, fileOpenMode, emptyBlockIndex);
            return emptyBlockIndex;
        }

        public void CloseFile()
        {

        }

        public void ReadFile()
        {

        }

        public void WriteFile()
        {

        }

        public void DeleteFile()
        {

        }
    }
}
