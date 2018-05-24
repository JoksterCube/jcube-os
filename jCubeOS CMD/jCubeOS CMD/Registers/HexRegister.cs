using System;
using System.Collections.Generic;
using System.Text;

namespace jCubeOS_CMD.Registers
{
    class HexRegister : Register
    {
        public HexRegister(int size = -1) => Cell = new Cell(size);

        public void SetValue(int value)
        {
            if (value >= 0)
            {
                base.SetValue(Utility.IntToHex(value, Cell.GetSize()));
            }
            else
            {

            }
        }

        public int GetIntValue() => Utility.HexToInt(base.GetValue());

        public void AddValue(int value) => SetValue(GetIntValue() + value);
    }
}
