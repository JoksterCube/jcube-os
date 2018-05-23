using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jCubeOS_CMD
{
    class Cell
    {
        private int Size { get; set; }
        private char[] Value { get; set; }
        
        public Cell(int size = -1)
        {
            Size = ((size == -1) ? Utility.WORD_SIZE : size);
            Value = new char[Size];
            for (int i = 0; i < Size; i++)
            {
                Value[i] = ' ';
            }
        }

        public void SetValue(char[] value)
        {
            if (value.Length == Size)
            {
                Value = value;
            }
            else if( value.Length < Size)
            {
                Value = value.AddWhiteSpacesToSize(Size);
            }
            else
            {
                throw new Exception(message: "Value does not fit in the cell");
            }
        }
        
        public char[] GetValue()
        {
            return Value;
        }

        public int GetSize()
        {
            return Size;
        }
    }
}
