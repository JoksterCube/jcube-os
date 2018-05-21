using System;
using System.Collections.Generic;
using System.Text;

namespace jCubeOS_CMD
{
    class ConsoleInput : Input
    {
        public override byte[][] ReadBlock()
        {
            Console.Write("INPUT: ");
            string block = Console.ReadLine();
            byte[][] blockBytes = new byte[Utility.BLOCK_SIZE][];
            if (block.Length > Utility.WORD_SIZE * Utility.BLOCK_SIZE)
            {
                block = block.Substring(0, Utility.WORD_SIZE * Utility.BLOCK_SIZE);
            }
            else
            {
                block = block.AddWhiteSpacesToSize(Utility.WORD_SIZE * Utility.BLOCK_SIZE);
            }
            for (int i = 0; i < Utility.BLOCK_SIZE; i++)
            {
                blockBytes[i] = new byte[Utility.WORD_SIZE];
                blockBytes[i] = Utility.StringToBytes(block.Substring(i * Utility.WORD_SIZE, Utility.WORD_SIZE));
            }
            return blockBytes;
        }

        public override byte[] ReadWord()
        {
            Console.Write("INPUT: ");
            string word = Console.ReadLine();
            byte[] wordBytes = new byte[Utility.WORD_SIZE];
            if (word.Length > Utility.WORD_SIZE)
            {
                word = word.Substring(0, Utility.WORD_SIZE);
            }
            else
            {
                word = word.AddWhiteSpacesToSize(Utility.WORD_SIZE);
            }
            wordBytes = Utility.StringToBytes(word);
            return wordBytes;
        }
    }
}
