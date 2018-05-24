using System;
using System.Collections.Generic;
using System.Text;

namespace jCubeOS_CMD.Registers
{
    class StatusFlagRegister : Register
    {
        private Dictionary<string, bool> Flags { get; set; }

        public StatusFlagRegister()
        {
            Flags = new Dictionary<string, bool>
            {
                { "CF", false },
                { "ZF", false },
                { "SF", false }
            };
            Cell = new Cell(1);
            UpdateCellValue();
        }

        public bool GetFlagValue(string flag)
        {
            if (Flags.ContainsKey(flag)) return Flags[flag];
            else throw new Exception("Unknown flag: " + flag);
        }

        public void SetFlagValue(string flag, bool value)
        {
            if (Flags.ContainsKey(flag))
            {
                Flags[flag] = value;
                UpdateCellValue();
            }
            else throw new Exception("Unknown flag: " + flag);
        }

        public override void SetValue(char[] value) => throw new Exception("StatusFlag value cannot be set.");

        private void UpdateCellValue()
        {

            byte maskCF = (byte)((Flags["CF"]) ? (1 << 0) : 0);
            byte maskZF = (byte)((Flags["ZF"]) ? (1 << 1) : 0);
            byte maskSF = (byte)((Flags["SF"]) ? (1 << 2) : 0);
            
            byte byteSF = 0;
            byteSF |= maskCF;
            byteSF |= maskZF;
            byteSF |= maskSF;

            char[] newValue = new char[] { Convert.ToChar(byteSF) };

            base.SetValue(newValue);
        }

        public override string ToString() => "CF:" + Flags["CF"] + " ZF:" + Flags["ZF"] + " SF:" + Flags["SF"];
    }
}
