using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jCubeOS.Classes
{
    static class Utility
    {
        public static readonly int BLOCKS = 128;
        public static readonly int BLOCK_SIZE = 16;
        public static readonly int WORD_SIZE = 4;
        public static readonly int VIRTUAL_MEMORY_BLOCKS = 16;


        /// <summary>
        /// Converts positive integer number to positive hex value of string type. If size is not -1, result will be at least that size.
        /// </summary>
        /// <param name="number">Integer number to convert</param>
        /// <param name="size">Minimum size of result</param>
        /// <returns></returns>
        public static string IntToHex(int number, int size = -1)
        {
            string hex = Math.Abs(number).ToString("X");
            if (size != -1)
            {
                string placeholder = string.Empty;
                for (int i = size - 1, ii = hex.Length - 1; i >= 0; i--, ii--)
                {
                    placeholder = (ii < 0 ? '0' : hex[ii]) + placeholder;
                }
                return placeholder;
            }
            return hex;
        }

        /// <summary>
        /// Converts positive hex string to positive integer number
        /// </summary>
        /// <param name="hex">Hex number to convert</param>
        /// <returns></returns>
        public static int HexToInt(string hex)
        {
            return int.Parse(hex, NumberStyles.HexNumber);
        }
    }
}
