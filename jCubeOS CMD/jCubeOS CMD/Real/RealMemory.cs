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
        private Block[] Memory { get; set; }

        public RealMemory()
        {
            //Creating All memory blocks
            Memory = new Block[Utility.BLOCKS];
            for (int i = 0; i < Utility.BLOCKS; i++)
            {
                //Creating all words/cells for individual memory block
                Memory[i] = new Block(Utility.BLOCK_SIZE, Utility.WORD_SIZE, false);
            }
        }

        private Cell GetMemoryCell(int block, int cell) => Memory[block].GetCell(cell);

        //USER MEMORY METHODS
        public Cell GetUserMemoryCell(int address)
        {
            var addressTuple = Utility.GetAddressTuple(address);
            int block = addressTuple.Item1;
            int cell = addressTuple.Item2;
            if (block < Utility.USER_MEMORY_BLOCKS) return GetMemoryCell(block, cell);
            else throw new Exception("Block exceeds user memory.");
        }

        public char[] GetUserMemoryValue(int address) => GetUserMemoryCell(address).GetValue();

        public char[][] GetUserMemoryBlockValues(int blockAddress)
        {
            blockAddress -= blockAddress % Utility.BLOCK_SIZE;

            char[][] block = new char[Utility.BLOCK_SIZE][];
            for (int i = 0; i < Utility.BLOCK_SIZE; i++) block[i] = GetUserMemoryValue(blockAddress + i);
            return block;
        }

        public void SetUserMemoryValue(int address, char[] value) => GetUserMemoryCell(address).SetValue(value);

        public void SetUserMemoryBlockValues(int blockAddress, char[][] values)
        {
            blockAddress -= blockAddress % Utility.BLOCK_SIZE;
            for (int i = 0; i < Utility.BLOCK_SIZE; i++) SetUserMemoryValue(blockAddress + i, values[i]);
        }

        public int GetFreeMemoryBlock()
        {
            for (int i = 0; i < Memory.Length; i++)
            {
                if (Memory[i].IsTaken()) continue;
                else return i;
            }
            throw new Exception("No available user memory blocks were found.");
        }

        public void TakeMemoryBlock(int blockAddress) => Memory[blockAddress].SetTaken(true);
        public void FreeMemoryBlock(int blockAddress) => Memory[blockAddress].SetTaken(false);

        //SUPERVISOR MEMORY METHODS
        public Cell GetSupervisorMemoryCell(int supervisorAddress)
        {
            var addressTuple = Utility.GetAddressTuple(supervisorAddress);
            int block = addressTuple.Item1;
            int cell = addressTuple.Item2;
            if (block < Utility.BLOCKS - Utility.USER_MEMORY_BLOCKS) return GetMemoryCell(block + Utility.USER_MEMORY_BLOCKS, cell);
            else throw new Exception("Block exceeds supervisor memory.");
        }

        public char[] GetSupervisorMemoryValue(int supervisorAddress) => GetSupervisorMemoryCell(supervisorAddress).GetValue();

        public void SetSupervisorMemoryValue(int supervisorAddress, char[] value) => GetSupervisorMemoryCell(supervisorAddress).SetValue(value);

        public char[][] GetSupervisorMemoryBlockValues(int supervisorBlockAddress)
        {
            supervisorBlockAddress -= supervisorBlockAddress % Utility.BLOCK_SIZE;

            char[][] block = new char[Utility.BLOCK_SIZE][];
            for (int i = 0; i < Utility.BLOCK_SIZE; i++) block[i] = GetSupervisorMemoryValue(supervisorBlockAddress + i);
            return block;
        }

        public void SetSupervisorMemoryBlockValues(int supervisorBlockAddress, char[][] values)
        {
            supervisorBlockAddress -= supervisorBlockAddress % Utility.BLOCK_SIZE;
            for (int i = 0; i < Utility.BLOCK_SIZE; i++) SetSupervisorMemoryValue(supervisorBlockAddress + i, values[i]);
        }

        public void PutDataToSupervisorMemory(char[][][] blocks, int supervisorStartBlockAddress)
        {
            supervisorStartBlockAddress -= supervisorStartBlockAddress % Utility.BLOCK_SIZE;
            if (blocks.Length <= Utility.BLOCKS - Utility.USER_MEMORY_BLOCKS)
            {
                for (int i = 0; i < blocks.Length; i++)
                {
                    if (blocks[i].Length <= Utility.BLOCK_SIZE)
                    {
                        for (int ii = 0; ii < blocks[i].Length; ii++)
                        {
                            if (blocks[i][ii] == null)
                            {
                                Console.WriteLine("FOUND");
                            }
                            SetSupervisorMemoryValue(supervisorStartBlockAddress + i * Utility.BLOCK_SIZE + ii, blocks[i][ii]);
                        }
                    }
                    else throw new Exception("Cannot put data to supervisor memory. Data block is too long.");
                }
            }
            else throw new Exception("Supervisor memory does not have enought memmory.");
        }

        public void CleanDataFromSupervisorMemory(int supervisorAddress, int blockCount)
        {
            supervisorAddress -= supervisorAddress % Utility.BLOCK_SIZE;
            char[] emptyWord = new char[Utility.WORD_SIZE];
            for (int i = 0; i < Utility.WORD_SIZE; i++) emptyWord[i] = ' ';
            for (int i = supervisorAddress; i < supervisorAddress + blockCount * Utility.BLOCK_SIZE; i++) SetSupervisorMemoryValue(i, emptyWord);
        }



        public VirtualMemory CreateVirtualMemory(int blockSize = -1)
        {
            int PTR = GetFreeMemoryBlock();
            TakeMemoryBlock(PTR);
            Pager pager = new Pager(this);
            pager.SetPTR(PTR);
            VirtualMemory virtualMemory = new VirtualMemory(this, pager);
            for (int i = 0; i < Utility.VIRTUAL_MEMORY_BLOCKS; i++)
            {
                int blockRealAddress = GetFreeMemoryBlock();
                TakeMemoryBlock(blockRealAddress);
                pager.AddBlock(i, blockRealAddress);
            }

            return virtualMemory;
        }

        public void PrintAllMemory()
        {
            Console.WriteLine("--------------------------------USER MEMORY-------------------------------------");
            for (int i = 0; i < Utility.USER_MEMORY_BLOCKS; i++)
            {
                string block = string.Empty;
                for (int ii = 0; ii < Utility.BLOCK_SIZE; ii++) block += new string(GetUserMemoryValue(i * Utility.BLOCK_SIZE + ii)) + new string(new char[] { '|' });
                Console.WriteLine(block.Replace('\n', '&'));
            }

            Console.WriteLine("------------------------------SUPERVISOR MEMORY---------------------------------");
            for (int i = 0; i < Utility.BLOCKS - Utility.USER_MEMORY_BLOCKS; i++)
            {
                string block = string.Empty;
                for (int ii = 0; ii < Utility.BLOCK_SIZE; ii++) block += new string(GetSupervisorMemoryValue(i * Utility.BLOCK_SIZE + ii)) + new string(new char[] { '|' });
                Console.WriteLine(block.Replace('\n', '&'));
            }
        }
    }
}
