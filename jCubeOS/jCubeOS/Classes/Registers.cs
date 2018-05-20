using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jCubeOS.Classes
{
    /// <summary>
    /// Common usage register
    /// </summary>
    class Register
    {
        protected Cell Cell { get; set; }

        public Register()
        {
            Cell = new Cell();
        }

        public virtual byte[] GetValue()
        {
            return Cell.GetValue();
        }

        public virtual void SetValue(byte[] value)
        {
            Cell.SetValue(value);
        }
    }

    /// <summary>
    /// Positive hex register
    /// </summary>
    class HexRegister : Register
    {
        public HexRegister(int size = -1)
        {
            Cell = new Cell(size);
        }

        public override void SetValue(byte[] value)
        {
            base.SetValue(value);
        }

        public void SetValue(int value)
        {

        }

    }

    /// <summary>
    /// Register for logic value
    /// </summary>
    class ChoiceRegister : Register
    {
        private byte[][] Choices { get; set; }

        /// <param name="choices">Possible values that this register can get</param>
        public ChoiceRegister(params int[] choices)
        {
            Choices = new byte[choices.Length][];
            for (int i = 0; i < choices.Length; i++)
            {
                byte[] choiceBytes = BitConverter.GetBytes(choices[i]);
                Choices[i] = choiceBytes;
            }

            int size = Choices.Max(w => w.Length);

            Cell = new Cell(size);

            SetValue(Choices[0]);
        }

        /// <summary>
        /// Before setting value it checks if it is one of the choices
        /// </summary>
        /// <param name="value"></param>
        public override void SetValue(byte[] value)
        {
            if (Choices.Contains(value))
            {
                base.SetValue(value);
            }
            else
            {

            }
        }

        /// <summary>
        /// Before setting value it checks if it is one of the choices
        /// </summary>
        /// <param name="value"></param>
        public void SetValue(int value)
        {
            byte[] valueBytes = BitConverter.GetBytes(value);

            if (Choices.Contains(valueBytes))
            {
                base.SetValue(valueBytes);
            }
            else
            {

            }
        }
    }

    /// <summary>
    /// Status Flag Register. Value cannot be set for this register.
    /// </summary>
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
        }

        /// <returns>Status Flag register value of the flag</returns>
        public bool GetFlagValue(string flag)
        {
            if (Flags.ContainsKey(flag))
            {
                return Flags[flag];
            }
            else
            {
                throw new Exception("Unknown flag");
            }
        }

        /// <summary>
        /// Setting flag value
        /// </summary>
        public void SetFlagValue(string flag, bool value)
        {
            if (Flags.ContainsKey(flag))
            {
                Flags[flag] = value;
            }
            else
            {
                throw new Exception("Unknown flag");
            }
        }

        /// <summary>
        /// Returns SF string value
        /// CxZxSx, where x are flag values
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "C" + Flags["CF"] + "Z" + Flags["ZF"] + "S" + Flags["SF"];
        }
    }

}
