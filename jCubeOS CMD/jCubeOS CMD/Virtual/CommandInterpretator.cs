using jCubeOS_CMD.Real;
using System;
using System.Collections.Generic;
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

        public void ParseCommand(char[] command)
        {
            string strCommand = new string(command);
            if (strCommand.StartsWith("L1") || strCommand.StartsWith("L2")) L(r: command[1], x: command[2], y: command[3]);
            else if (strCommand.StartsWith("S1") || strCommand.StartsWith("S2")) S(r: command[1], x: command[2], y: command[3]);
            else if (strCommand.StartsWith("ADD")) ADD();
            else if (strCommand.StartsWith("AD")) AD(x: command[2], y: command[3]);
            else if (strCommand.StartsWith("SUB")) SUB();
            else if (strCommand.StartsWith("SB")) SB(x: command[2], y: command[3]);
            else if (strCommand.StartsWith("MUL")) MUL();
            else if (strCommand.StartsWith("ML")) ML(x: command[2], y: command[3]);
            else if (strCommand.StartsWith("DIV")) DIV();
            else if (strCommand.StartsWith("CMP")) CMP();
            else if (strCommand.StartsWith("XOR")) XOR();
            else if (strCommand.StartsWith("AND")) AND();
            else if (strCommand.StartsWith("OR")) OR();
            else if (strCommand.StartsWith("NOT")) NOT();
            else if (strCommand.StartsWith("GO")) GO(x: command[2], y: command[3]);
            else if (strCommand.StartsWith("JG")) JG(x: command[2], y: command[3]);
            else if (strCommand.StartsWith("JL")) JL(x: command[2], y: command[3]);
            else if (strCommand.StartsWith("JC")) JC(x: command[2], y: command[3]);
            else if (strCommand.StartsWith("JZ")) JC(x: command[2], y: command[3]);
            else if (strCommand.StartsWith("JN")) JC(x: command[2], y: command[3]);
            else if (strCommand.StartsWith("GDB")) GDB(x: command[3]);
            else if (strCommand.StartsWith("GD")) GD(r: command[2]);
            else if (strCommand.StartsWith("PDB")) PDB(x: command[3]);
            else if (strCommand.StartsWith("PD")) PD(r: command[2]);
            else if (strCommand.StartsWith("FO")) FO(w: command[2], x: command[3]);
            else if (strCommand.StartsWith("FG")) FG(x: command[2], y: command[3]);
            else if (strCommand.StartsWith("FP")) FP(x: command[2], y: command[3]);
            else if (strCommand.StartsWith("FW")) FW(x: command[2], y: command[3]);
            else if (strCommand.StartsWith("FC")) FC(x: command[2]);
            else if (strCommand.StartsWith("FCR")) FCR();
            else if (strCommand.StartsWith("FD")) FD(x: command[2]);
            else if (strCommand.StartsWith("HALT")) HALT();
            else throw new Exception("Command " + strCommand + " does not exists.");
        }

        private void L(char r, char x, char y)
        {
            char[] memoryWord = VirtualMemory.GetValue(x * Utility.BLOCK_SIZE + y);
            switch (r)
            {
                case '1':
                    Processor.SetRegisterValue("R1", memoryWord);
                    break;
                case '2':
                    Processor.SetRegisterValue("R2", memoryWord);
                    break;
            }
        }

        private void S(char r, char x, char y)
        {

        }

        private void ADD()
        {

        }

        private void AD(char x, char y)
        {

        }

        private void SUB()
        {

        }

        private void SB(char x, char y)
        {

        }

        private void MUL()
        {

        }

        private void ML(char x, char y)
        {

        }

        private void DIV()
        {

        }

        private void CMP()
        {

        }

        private void XOR()
        {

        }

        private void AND()
        {

        }

        private void OR()
        {

        }

        private void NOT()
        {

        }

        private void GO(char x, char y)
        {

        }

        private void JG(char x, char y)
        {

        }

        private void JL(char x, char y)
        {

        }

        private void JC(char x, char y)
        {

        }

        private void JZ(char x, char y)
        {

        }

        private void JN(char x, char y)
        {

        }

        private void GDB(char x)
        {

        }

        private void GD(char r)
        {

        }

        private void PDB(char x)
        {
            int numX = x - '0';
            Processor.UseChannelTool(VirtualMemory.GetPager().GetCellRealAddress(numX * Utility.BLOCK_SIZE), 0, 1, 4);
        }

        private void PD(char r)
        {

        }

        private void FO(char w, char x)
        {

        }

        private void FG(char x, char y)
        {

        }

        private void FP(char x, char y)
        {

        }

        private void FW(char x, char y)
        {

        }

        private void FC(char x)
        {

        }

        private void FCR()
        {

        }

        private void FD(char x)
        {

        }

        private void HALT()
        {

        }
    }
}
