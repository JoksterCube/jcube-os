using System;
using System.Collections.Generic;
using System.Text;

namespace jCubeOS_CMD.Registers
{
    class HexRegister : Register
    {
        public HexRegister(int size = -1) => Cell = new Cell(size);

        public HexRegister(int value, int size = -1)
        {
            Cell = new Cell(size);
            SetValue(value);
        }

        public void SetValue(int value) => base.SetValue(Utility.IntToHex(Math.Abs(value), Cell.GetSize()));

        public int GetIntValue() => Utility.HexToInt(base.GetValue());

        public void AddValue(int value) => SetValue(GetIntValue() + value);
    }
}
