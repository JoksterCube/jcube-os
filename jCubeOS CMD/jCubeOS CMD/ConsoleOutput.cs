using System;
using System.Collections.Generic;
using System.Text;

namespace jCubeOS_CMD
{
    class ConsoleOutput : Output
    {
        public override void WriteBlock(byte[][] block)
        {
            if (block != null)
            {
                for (int i = 0, b = block.Length - 1; i < Utility.BLOCK_SIZE; i++, b--)
                {
                    Console.Write(((b > 0) ? Utility.BytesToString(block[i]) : string.Empty).AddWhiteSpacesToSize(Utility.WORD_SIZE));
                }
                Console.Write('\n');
            }
        }

        public override void WriteWord(byte[] word)
        {
            Console.WriteLine(Utility.BytesToString(word).AddWhiteSpacesToSize(Utility.WORD_SIZE));
        }
    }
}
