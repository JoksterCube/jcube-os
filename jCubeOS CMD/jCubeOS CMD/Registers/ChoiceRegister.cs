using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace jCubeOS_CMD.Registers
{
    class ChoiceRegister : Register
    {
        private char[][] Choices { get; set; }

        public ChoiceRegister(params char[][] choices) => Init(choices);

        public ChoiceRegister(params int[] choices)
        {
            char[][] charChoices = new char[choices.Length][];
            for (int i = 0; i < choices.Length; i++)
            {
                if (choices[i] >= 0) charChoices[i] = Utility.IntToChars(choices[i]);
                else throw new Exception("Choice register cannot get negative integer value.");
            }
            Init(charChoices);
        }

        private void Init(char[][] choices)
        {
            Choices = choices;

            int max = 0;

            for (int i = 0; i < choices.Length; i++) if (choices[i].Length > max) max = choices[i].Length;

            int size = max;

            Cell = new Cell(size);

            SetValue(Choices[0]);
        }

        public override void SetValue(char[] value)
        {
            if (ContainsChoice(value))
            {
                base.SetValue(value);
            }
            else
            {

            }
        }

        public void SetValue(int value)
        {
            if (value >= 0)
            {
                SetValue(Utility.IntToChars(value));
            }
            else
            {

            }
        }

        public int GetIntValue() => Utility.CharsToInt(GetValue());

        private bool ContainsChoice(char[] value)
        {
            bool contains = false;
            for (int i = 0; i < Choices.Length; i++)
            {
                if (value.Length != Choices[i].Length) continue;
                bool equals = true;
                for (int ii = 0; ii < Choices[i].Length; ii++)
                {
                    if (Choices[i][ii] != value[ii])
                    {
                        equals = false;
                        break;
                    }

                }
                if (equals)
                {
                    contains = true;
                    break;
                }
            }
            return contains;
        }
    }
}
