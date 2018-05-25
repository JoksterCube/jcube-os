using System;
using System.Collections.Generic;
using System.Text;

namespace jCubeOS_CMD
{
    class ConsoleInput : Input
    {
        public override char[][] ReadBlock()
        {
            Console.Write("INPUT: ");
            string block = Console.ReadLine();
            char[][] blockChars = new char[Utility.BLOCK_SIZE][];
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
                blockChars[i] = new char[Utility.WORD_SIZE];
                blockChars[i] = block.Substring(i * Utility.WORD_SIZE, Utility.WORD_SIZE).ToCharArray();
            }
            return blockChars;
        }

        public override char[] ReadWord()
        {
            Console.Write("INPUT: ");
            string word = Console.ReadLine();
            char[] wordChars = new char[Utility.WORD_SIZE];
            if (word.Length > Utility.WORD_SIZE) word = word.Substring(0, Utility.WORD_SIZE);
            else if (word.Length < Utility.WORD_SIZE) word = word.AddWhiteSpacesToSize(Utility.WORD_SIZE);
            wordChars = word.ToCharArray();
            return wordChars;
        }
    }
}
