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
        public static readonly int USER_MEMORY_BLOCKS = 96;
        public static readonly int EXTERNAL_MEMORY_BLOCKS = 128;
        public static readonly int TIMER_VALUE = 15;
        public static readonly int MAX_STEPS = 256;
        public static readonly int FILE_MANAGER_BLOCKS = 8;

        public enum CharMode { Number, Character }

        public static char[] IntToHex(this int number, int size = -1)
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

        public static bool IsHex(this char[] hex) => IsHex(new string(hex));

        public static bool IsHex(this string hex) => int.TryParse(hex, NumberStyles.HexNumber, null, out int value);

        public static int HexToInt(this char[] hex) => HexToInt(new string(hex));

        public static int HexToInt(this string hex) => int.Parse(hex, NumberStyles.HexNumber);

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

        public static char[] RemoveWhiteSpaces(this char[] charArray) => RemoveWhiteSpaces(new string(charArray)).ToCharArray();

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

        public static string AddEndLine(this string str)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(str);
            sb.Append('\n');
            return sb.ToString();
        }

        public static char[] AddEndLine(this char[] charArray)
        {
            char[] newCharArray = new char[charArray.Length + 1];
            for (int i = 0; i < charArray.Length; i++) newCharArray[i] = charArray[i];
            newCharArray[charArray.Length] = '\n';
            return newCharArray;
        }

        public static Tuple<int, int> GetAddressTuple(int address)
        {
            int block = address / BLOCK_SIZE;
            int cell = address % BLOCK_SIZE;
            return Tuple.Create(block, cell);
        }

        public static int GetAddressInt(int block, int cell) => block * BLOCK_SIZE + cell;

        public static char[] IntToChars(this int value, CharMode charMode = CharMode.Character)
        {
            char[] charValue = value.ToString().ToCharArray();
            switch (charMode)
            {
                case CharMode.Number:
                    for (int i = 0; i < charValue.Length; i++) charValue[i] -= '0';
                    return charValue;
                case CharMode.Character:
                default:
                    return charValue;
            }
        }

        public static int CharsToInt(this char[] value, CharMode charMode = CharMode.Character)
        {
            int intValue = 0;
            value = value.RemoveWhiteSpaces();
            for (int i = 0; i < value.Length; i++)
            {
                intValue *= 10;
                intValue += value[i];
                switch (charMode)
                {
                    case CharMode.Number:
                        break;
                    case CharMode.Character:
                        intValue -= '0';
                        break;
                }
            }
            return intValue;
        }

        public static string CharsToString(this char[][] chars)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < chars.Length; i++) sb.Append(new string(chars[i]));
            return sb.ToString();
        }

        public static char[] StringToChars(this string str) => str.ToCharArray();
    }
}