using jCubeOS_CMD.Real;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace jCubeOS_CMD.Virtual
{
    class CommandInterpretator
    {
        private Processor Processor { get; set; }
        private VirtualMemory VirtualMemory { get; set; }

        public CommandInterpretator(Processor processor, VirtualMemory virtualMemory)
        {
            Processor = processor;
            VirtualMemory = virtualMemory;
        }

        public void SetVirtualMemory(VirtualMemory virtualMemory) => VirtualMemory = virtualMemory;

        public bool ParseCommand(char[] command)
        {
            string strCommand = new string(command);
            if (strCommand.StartsWith("L1") || strCommand.StartsWith("L2")) return L(r: command[1], x: command[2], y: command[3]);
            else if (strCommand.StartsWith("S1") || strCommand.StartsWith("S2")) return S(r: command[1], x: command[2], y: command[3]);
            else if (strCommand.StartsWith("ADD")) return ADD();
            else if (strCommand.StartsWith("AD")) return AD(x: command[2], y: command[3]);
            else if (strCommand.StartsWith("SUB")) return SUB();
            else if (strCommand.StartsWith("SB")) return SB(x: command[2], y: command[3]);
            else if (strCommand.StartsWith("MUL")) return MUL();
            else if (strCommand.StartsWith("ML")) return ML(x: command[2], y: command[3]);
            else if (strCommand.StartsWith("DIV")) return DIV();
            else if (strCommand.StartsWith("CMP")) return CMP();
            else if (strCommand.StartsWith("C1") || strCommand.StartsWith("C2")) return C(r: command[1], x: command[2], y: command[3]);
            else if (strCommand.StartsWith("XOR")) return XOR();
            else if (strCommand.StartsWith("AND")) return AND();
            else if (strCommand.StartsWith("OR")) return OR();
            else if (strCommand.StartsWith("NOT")) return NOT();
            else if (strCommand.StartsWith("GO")) return GO(x: command[2], y: command[3]);
            else if (strCommand.StartsWith("JG")) return JG(x: command[2], y: command[3]);
            else if (strCommand.StartsWith("JL")) return JL(x: command[2], y: command[3]);
            else if (strCommand.StartsWith("JC")) return JC(x: command[2], y: command[3]);
            else if (strCommand.StartsWith("JZ")) return JZ(x: command[2], y: command[3]);
            else if (strCommand.StartsWith("JN")) return JN(x: command[2], y: command[3]);
            else if (strCommand.StartsWith("GDB")) return GDB(x: command[3]);
            else if (strCommand.StartsWith("GD1") || strCommand.StartsWith("GD2")) return GD(r: command[2]);
            else if (strCommand.StartsWith("PDB")) return PDB(x: command[3]);
            else if (strCommand.StartsWith("PD1") || strCommand.StartsWith("PD2")) return PD(r: command[2]);
            else if (strCommand.StartsWith("FOW") || strCommand.StartsWith("FOR")) return FO(w: command[2], x: command[3]);
            else if (strCommand.StartsWith("FG")) return FG(x: command[2], y: command[3]);
            else if (strCommand.StartsWith("FP")) return FP(x: command[2], y: command[3]);
            else if (strCommand.StartsWith("FW")) return FW(x: command[2], y: command[3]);
            else if (strCommand.StartsWith("FC")) return FC(x: command[2]);
            else if (strCommand.StartsWith("FCR")) return FCR();
            else if (strCommand.StartsWith("FD")) return FD(x: command[2]);
            else if (strCommand.StartsWith("HALT")) return HALT();
            else return IncorrectCommand();
        }

        private bool IncorrectCommand()
        {
            Processor.SetChoiceRegisterValue("PI", 1);
            return false;
        }

        private void UpdateStatusFlag(int aritmeticOperationResult)
        {
            bool CF = ((aritmeticOperationResult > 0) ? (aritmeticOperationResult.IntToHex().Length > Utility.WORD_SIZE) : false);
            bool ZF = aritmeticOperationResult == 0;
            bool SF = aritmeticOperationResult < 0;
            Processor.SetSFRegisterFlag("CF", CF);
            Processor.SetSFRegisterFlag("ZF", ZF);
            Processor.SetSFRegisterFlag("SF", SF);
        }

        private bool L(char r, char x, char y)
        {
            int numX = x.ToString().HexToInt();
            int numY = y.ToString().HexToInt();
            char[] memoryWord = VirtualMemory.GetValue(numX * Utility.BLOCK_SIZE + numY);
            switch (r)
            {
                case '1':
                    Processor.SetRegisterValue("R1", memoryWord);
                    break;
                case '2':
                    Processor.SetRegisterValue("R2", memoryWord);
                    break;
                default: return false;
            }
            Processor.DecTIRegisterValue();
            return true;
        }

        private bool S(char r, char x, char y)
        {
            int numX = x.ToString().HexToInt();
            int numY = y.ToString().HexToInt();
            char[] registerContent;
            switch (r)
            {
                case '1':
                    registerContent = Processor.GetRegisterValue("R1");
                    break;
                case '2':
                    registerContent = Processor.GetRegisterValue("R2");
                    break;
                default: return false;
            }
            VirtualMemory.SetValue(numX * Utility.BLOCK_SIZE + numY, registerContent);
            Processor.DecTIRegisterValue();
            return true;
        }

        private bool ADD()
        {
            char[] R1content = Processor.GetRegisterValue("R1");
            char[] R2content = Processor.GetRegisterValue("R2");
            if (R1content.IsHex() && R2content.IsHex())
            {
                int R1intValue = R1content.HexToInt();
                int R2intValue = R2content.HexToInt();

                int result = R1intValue + R2intValue;
                char[] hexResult = result.IntToHex();
                if (hexResult.Length > Utility.WORD_SIZE) hexResult = hexResult.Skip(hexResult.Length - Utility.WORD_SIZE).ToArray();

                Processor.SetRegisterValue("R1", hexResult);
                UpdateStatusFlag(result);
                Processor.DecTIRegisterValue();
                return true;
            }
            else
            {
                Processor.SetChoiceRegisterValue("PI", 3);
                return false;
            }
        }

        private bool AD(char x, char y)
        {
            char[] R1content = Processor.GetRegisterValue("R1");
            char[] R2content = Processor.GetRegisterValue("R2");
            if (R1content.IsHex() && R2content.IsHex())
            {
                int R1intValue = R1content.HexToInt();
                int R2intValue = R2content.HexToInt();

                int result = R1intValue + R2intValue;
                char[] hexResult = result.IntToHex();
                if (hexResult.Length > Utility.WORD_SIZE) hexResult = hexResult.Skip(hexResult.Length - Utility.WORD_SIZE).ToArray();

                int numX = x.ToString().HexToInt();
                int numY = y.ToString().HexToInt();
                VirtualMemory.SetValue(numX * Utility.BLOCK_SIZE + numY, hexResult);

                UpdateStatusFlag(result);
                Processor.DecTIRegisterValue();
                return true;
            }
            else
            {
                Processor.SetChoiceRegisterValue("PI", 3);
                return false;
            }
        }

        private bool SUB()
        {
            char[] R1content = Processor.GetRegisterValue("R1");
            char[] R2content = Processor.GetRegisterValue("R2");
            if (R1content.IsHex() && R2content.IsHex())
            {
                int R1intValue = R1content.HexToInt();
                int R2intValue = R2content.HexToInt();

                int result = R1intValue - R2intValue;
                char[] hexResult = result.IntToHex();
                if (hexResult.Length > Utility.WORD_SIZE) hexResult = hexResult.Skip(hexResult.Length - Utility.WORD_SIZE).ToArray();

                Processor.SetRegisterValue("R1", hexResult);

                UpdateStatusFlag(result);
                Processor.DecTIRegisterValue();
                return true;
            }
            else
            {
                Processor.SetChoiceRegisterValue("PI", 3);
                return false;
            }
        }

        private bool SB(char x, char y)
        {
            char[] R1content = Processor.GetRegisterValue("R1");
            char[] R2content = Processor.GetRegisterValue("R2");
            if (R1content.IsHex() && R2content.IsHex())
            {
                int R1intValue = R1content.HexToInt();
                int R2intValue = R2content.HexToInt();

                int result = R1intValue - R2intValue;
                char[] hexResult = result.IntToHex();
                if (hexResult.Length > Utility.WORD_SIZE) hexResult = hexResult.Skip(hexResult.Length - Utility.WORD_SIZE).ToArray();

                int numX = x.ToString().HexToInt();
                int numY = y.ToString().HexToInt();
                VirtualMemory.SetValue(numX * Utility.BLOCK_SIZE + numY, hexResult);

                UpdateStatusFlag(result);
                Processor.DecTIRegisterValue();
                return true;
            }
            else
            {
                Processor.SetChoiceRegisterValue("PI", 3);
                return false;
            }
        }

        private bool MUL()
        {
            char[] R1content = Processor.GetRegisterValue("R1");
            char[] R2content = Processor.GetRegisterValue("R2");
            if (R1content.IsHex() && R2content.IsHex())
            {
                int R1intValue = R1content.HexToInt();
                int R2intValue = R2content.HexToInt();

                int result = R1intValue * R2intValue;
                char[] hexResult = result.IntToHex();
                if (hexResult.Length > Utility.WORD_SIZE) hexResult = hexResult.Skip(hexResult.Length - Utility.WORD_SIZE).ToArray();
                Processor.SetRegisterValue("R1", hexResult);

                UpdateStatusFlag(result);
                Processor.DecTIRegisterValue();
                return true;
            }
            else
            {
                Processor.SetChoiceRegisterValue("PI", 3);
                return false;
            }
        }

        private bool ML(char x, char y)
        {
            char[] R1content = Processor.GetRegisterValue("R1");
            char[] R2content = Processor.GetRegisterValue("R2");
            if (R1content.IsHex() && R2content.IsHex())
            {
                int R1intValue = R1content.HexToInt();
                int R2intValue = R2content.HexToInt();

                int result = R1intValue * R2intValue;
                char[] hexResult = result.IntToHex();
                if (hexResult.Length > Utility.WORD_SIZE) hexResult = hexResult.Skip(hexResult.Length - Utility.WORD_SIZE).ToArray();

                int numX = x.ToString().HexToInt();
                int numY = y.ToString().HexToInt();
                VirtualMemory.SetValue(numX * Utility.BLOCK_SIZE + numY, hexResult);

                UpdateStatusFlag(result);
                Processor.DecTIRegisterValue();
                return true;
            }
            else
            {
                Processor.SetChoiceRegisterValue("PI", 3);
                return false;
            }
        }

        private bool DIV()
        {
            char[] R1content = Processor.GetRegisterValue("R1");
            char[] R2content = Processor.GetRegisterValue("R2");
            if (R1content.IsHex() && R2content.IsHex())
            {
                int R1intValue = R1content.HexToInt();
                int R2intValue = R2content.HexToInt();

                int result = R1intValue / R2intValue;
                int resultRemainder = R1intValue % R2intValue;

                char[] hexResult = result.IntToHex();
                char[] hexResultRemainder = resultRemainder.IntToHex();

                Processor.SetRegisterValue("R1", hexResult);
                Processor.SetRegisterValue("R2", hexResult);

                UpdateStatusFlag(result);
                Processor.DecTIRegisterValue();
                return true;
            }
            else
            {
                Processor.SetChoiceRegisterValue("PI", 3);
                return false;
            }
        }

        private bool CMP()
        {
            char[] R1content = Processor.GetRegisterValue("R1");
            char[] R2content = Processor.GetRegisterValue("R2");
            if (R1content.IsHex() && R2content.IsHex())
            {
                int R1intValue = R1content.HexToInt();
                int R2intValue = R2content.HexToInt();

                int result = R1intValue - R2intValue;
                UpdateStatusFlag(result);
                Processor.DecTIRegisterValue();
                return true;
            }
            else
            {
                Processor.SetChoiceRegisterValue("PI", 3);
                return false;
            }
        }

        private bool C(char r, char x, char y)
        {
            int numX = x.ToString().HexToInt();
            int numY = y.ToString().HexToInt();
            char[] memoryWord = VirtualMemory.GetValue(numX * Utility.BLOCK_SIZE + numY);

            char[] registerContent;
            switch (r)
            {
                case '1':
                    registerContent = Processor.GetRegisterValue("R1");
                    break;
                case '2':
                    registerContent = Processor.GetRegisterValue("R2");
                    break;
                default: return false;
            }

            if (registerContent.IsHex())
            {
                int registerInt = registerContent.HexToInt();
                int memoryInt = memoryWord.HexToInt();

                int result = registerInt - memoryInt;
                UpdateStatusFlag(result);
                Processor.DecTIRegisterValue();
                return true;
            }
            else
            {
                Processor.SetChoiceRegisterValue("PI", 3);
                return false;
            }
        }

        private bool XOR()
        {
            char[] R1content = Processor.GetRegisterValue("R1");
            char[] R2content = Processor.GetRegisterValue("R2");
            if (R1content.IsHex() && R2content.IsHex())
            {
                int R1intValue = R1content.HexToInt();
                int R2intValue = R2content.HexToInt();

                int result = R1intValue ^ R2intValue;
                char[] hexResult = result.IntToHex();
                if (hexResult.Length > Utility.WORD_SIZE) hexResult = hexResult.Skip(hexResult.Length - Utility.WORD_SIZE).ToArray();
                Processor.SetRegisterValue("R1", hexResult);

                UpdateStatusFlag(result);
                Processor.DecTIRegisterValue();
                return true;
            }
            else
            {
                Processor.SetChoiceRegisterValue("PI", 3);
                return false;
            }
        }

        private bool AND()
        {
            char[] R1content = Processor.GetRegisterValue("R1");
            char[] R2content = Processor.GetRegisterValue("R2");
            if (R1content.IsHex() && R2content.IsHex())
            {
                int R1intValue = R1content.HexToInt();
                int R2intValue = R2content.HexToInt();

                int result = R1intValue & R2intValue;
                char[] hexResult = result.IntToHex();
                if (hexResult.Length > Utility.WORD_SIZE) hexResult = hexResult.Skip(hexResult.Length - Utility.WORD_SIZE).ToArray();
                Processor.SetRegisterValue("R1", hexResult);

                UpdateStatusFlag(result);
                Processor.DecTIRegisterValue();
                return true;
            }
            else
            {
                Processor.SetChoiceRegisterValue("PI", 3);
                return false;
            }
        }

        private bool OR()
        {
            char[] R1content = Processor.GetRegisterValue("R1");
            char[] R2content = Processor.GetRegisterValue("R2");
            if (R1content.IsHex() && R2content.IsHex())
            {
                int R1intValue = R1content.HexToInt();
                int R2intValue = R2content.HexToInt();

                int result = R1intValue | R2intValue;
                char[] hexResult = result.IntToHex();
                if (hexResult.Length > Utility.WORD_SIZE) hexResult = hexResult.Skip(hexResult.Length - Utility.WORD_SIZE).ToArray();
                Processor.SetRegisterValue("R1", hexResult);

                UpdateStatusFlag(result);
                Processor.DecTIRegisterValue();
                return true;
            }
            else
            {
                Processor.SetChoiceRegisterValue("PI", 3);
                return false;
            }
        }

        private bool NOT()
        {
            char[] R1content = Processor.GetRegisterValue("R1");
            if (R1content.IsHex())
            {
                int R1intValue = R1content.HexToInt();

                int result = ~R1intValue;
                char[] hexResult = result.IntToHex();
                if (hexResult.Length > Utility.WORD_SIZE) hexResult = hexResult.Skip(hexResult.Length - Utility.WORD_SIZE).ToArray();
                Processor.SetRegisterValue("R1", hexResult);

                UpdateStatusFlag(result);
                Processor.DecTIRegisterValue();
                return true;
            }
            else
            {
                Processor.SetChoiceRegisterValue("PI", 3);
                return false;
            }
        }

        private bool GO(char x, char y)
        {
            int numX = x.ToString().HexToInt();
            int numY = y.ToString().HexToInt();
            Processor.SetICRegisterValue(numX * Utility.BLOCK_SIZE + numY);
            Processor.ChangedIC = true;
            Processor.DecTIRegisterValue();
            return true;
        }

        private bool JG(char x, char y)
        {
            if (Processor.GetSFRegisterFlag("ZF") && Processor.GetSFRegisterFlag("SF")) return GO(x, y);
            else Processor.DecTIRegisterValue();
            return false;
        }

        private bool JL(char x, char y)
        {
            if (Processor.GetSFRegisterFlag("SF")) return GO(x, y);
            else Processor.DecTIRegisterValue();
            return true;
        }

        private bool JC(char x, char y)
        {
            if (Processor.GetSFRegisterFlag("CF")) return GO(x, y);
            else Processor.DecTIRegisterValue();
            return true;
        }

        private bool JZ(char x, char y)
        {
            if (Processor.GetSFRegisterFlag("ZF")) return GO(x, y);
            else Processor.DecTIRegisterValue();
            return true;
        }

        private bool JN(char x, char y)
        {
            if (!Processor.GetSFRegisterFlag("ZF")) return GO(x, y);
            else Processor.DecTIRegisterValue();
            return true;
        }

        private bool GDB(char x)
        {
            if (x.ToString().IsHex())
            {
                Processor.SetChoiceRegisterValue("SI", 1);
                Processor.DecTIRegisterValue();
                return true;
            }
            else
            {
                Processor.SetChoiceRegisterValue("PI", 1);
                return true;
            }
        }

        private bool GD(char r)
        {
            if (r == '1' || r == '2')
            {
                Processor.SetChoiceRegisterValue("SI", 2);
                Processor.DecTIRegisterValue();
            }
            return true;
        }

        private bool PDB(char x)
        {
            if (x.ToString().IsHex())
            {
                int numX = x.ToString().HexToInt();
                Processor.SetChoiceRegisterValue("SI", 3);
                Processor.DecTIRegisterValue();
                return true;
            }
            else
            {
                Processor.SetChoiceRegisterValue("PI", 1);
                return true;
            }
        }

        private bool PD(char r)
        {
            Processor.SetChoiceRegisterValue("SI", 4);
            Processor.DecTIRegisterValue();
            return true;
        }

        private bool FO(char w, char x)
        {
            if (x.ToString().IsHex())
            {
                Processor.SetChoiceRegisterValue("SI", 5);
                Processor.DecTIRegisterValue();
                return true;
            }
            else
            {
                Processor.SetChoiceRegisterValue("PI", 1);
                return true;
            }
        }

        private bool FG(char x, char y)
        {

            return false;
        }

        private bool FP(char x, char y)
        {

            return false;
        }

        private bool FW(char x, char y)
        {

            return false;
        }

        private bool FC(char x)
        {

            return false;
        }

        private bool FCR()
        {

            return false;
        }

        private bool FD(char x)
        {

            return false;
        }

        private bool HALT()
        {
            Processor.SetChoiceRegisterValue("SI", 10);
            Processor.DecTIRegisterValue();
            return true;
        }
    }
}
