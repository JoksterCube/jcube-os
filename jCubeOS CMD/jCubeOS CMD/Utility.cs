using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jCubeOS_CMD
{
    static class Utility
    {
        public static readonly int BLOCKS = 128;
        public static readonly int BLOCK_SIZE = 16;
        public static readonly int WORD_SIZE = 4;
        public static readonly int VIRTUAL_MEMORY_BLOCKS = 16;
        public static readonly int SUPERVISOR_MEMORY_BLOCKS = 32;
        public static readonly int EXTERNAL_MEMORY_BLOCKS = 128;

        public static char[] IntToHex(int number, int size = -1)
        {
            string hex = Math.Abs(number).ToString("X");
            if (size != -1)
            {
                char[] placeholder = new char[size];
                for (int i = size - 1, ii = hex.Length - 1; i >= 0; i--, ii--)
                {
                    placeholder[i] = (ii < 0 ? '0' : hex[ii]);
                }
                return placeholder;
            }
            return hex.ToCharArray();
        }

        public static int HexToInt(char[] hex)
        {
            return int.Parse(hex.ToString(), NumberStyles.HexNumber);
        }

        public static string RemoveWhiteSpaces(this string str)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < str.Length; i++)
            {
                char c = str[i];
                if (!Char.IsWhiteSpace(c))
                    sb.Append(c);
            }

            return sb.ToString();
        }

        public static string AddWhiteSpacesToSize(this string str, int size)
        {
            if (size < str.Length) return str;
            StringBuilder sb = new StringBuilder();
            sb.Append(str);
            for (int i = str.Length; i < size; i++)
            {
                sb.Append(' ');
            }
            return sb.ToString();
        }

        public static char[] AddWhiteSpacesToSize(this char[] arrayAddTo, int size)
        {
            if (size < arrayAddTo.Length) return arrayAddTo;
            char[] newArray = new char[size];
            for (int i = 0; i < size; i++)
            {
                newArray[i] = ((i < arrayAddTo.Length) ? arrayAddTo[i] : ' ');
            }
            return newArray;
        }

        public static byte[] StringToBytes(string str)
        {
            byte[] strBytes = new byte[str.Length];
            for (int i = 0; i < str.Length; i++)
            {
                char c = (char)str[i];
                strBytes[i] = Convert.ToByte(c);
            }
            return strBytes;
        }

        public static string BytesToString(byte[] bytes)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                char c = Convert.ToChar(bytes[i]);
                sb.Append(c);
            }
            return sb.ToString();
        }

        public static byte[] IntToBytes(int integer, int sizeIfPossible = -1)
        {
            byte[] intBytes = BitConverter.GetBytes(integer);
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(intBytes);
            }
            if (sizeIfPossible < 0)
            {
                return intBytes;
            }
            else
            {
                List<byte> sizedBytes = new List<byte>();
                if (sizeIfPossible > intBytes.Length)
                {
                    for (int i = sizeIfPossible - 1, b = intBytes.Length - 1; i >= 0; i--, b--)
                    {
                        sizedBytes.Add((b > 0) ? intBytes[b] : (byte)0);
                    }
                }
                else
                {
                    bool added = false;
                    for (int i = 0, s = sizeIfPossible - intBytes.Length; i < intBytes.Length; i++, s++)
                    {
                        if (intBytes[i] != 0 || added || s >= 0)
                        {
                            sizedBytes.Add(intBytes[i]);
                            added = true;
                        }
                        else if (!added && intBytes[i] == 0 && s < 0)
                        {
                            continue;
                        }
                    }
                }
                return sizedBytes.ToArray();
            }
        }

        public static Tuple<int, int> GetAdressTuple(int address)
        {
            int block = address / BLOCK_SIZE;
            int cell = address % BLOCK_SIZE;
            return Tuple.Create(block, cell);
        }

        public static char[] IntToChars(int value)
        {
            char[] charValue = value.ToString().ToCharArray();
            for (int i = 0; i < charValue.Length; i++)
            {
                charValue[i] -= '0';
            }
            return charValue;
        }
    }
}
