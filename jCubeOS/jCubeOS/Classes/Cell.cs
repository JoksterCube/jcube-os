using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jCubeOS.Classes
{
    /// <summary>
    /// Memory cell. Has size out of bytes
    /// </summary>
    class Cell
    {
        private int Size { get; set; }
        private byte[] Value { get; set; }

        /// <param name="size">Cell size in bytes</param>
        public Cell(int size = -1)
        {
            Size = (size == -1) ? Utility.WORD_SIZE : size;
            Value = new byte[Size];
            for (int i = 0; i < Size; i++)
            {
                Value[i] = 0;
            }
        }

        /// <summary>
        /// Checks if the new value fits in the cell. If it does it sets Value to the new value. If it doesn't ----- Error
        /// </summary>
        /// <param name="value"></param>
        public void SetValue(byte[] value)
        {
            if (value.Length <= Size)
            {
                Value = value;
            }
            else
            {
                throw new Exception(message: "Value does not fit in the cell");
            }
        }

        /// <returns>Returns cells value</returns>
        public byte[] GetValue()
        {
            return Value;
        }

        //public string GetStringValue()
        //{
        //    string value = string.Empty;
        //    for (int i = 0; i < Size; i++)
        //    {
        //        value += Value[i];
        //    }
        //    return value;
        //}
    }
}
