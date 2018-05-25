using jCubeOS_CMD.Real;
using jCubeOS_CMD.Virtual;
using System;
using System.Collections.Generic;
using System.Text;

namespace jCubeOS_CMD
{
    class Interruptor
    {
        private Processor Processor { get; set; }
        private VirtualMemory VirtualMemory { get; set; }
        private FileManager FileManager { get; set; }

        private enum ReadWriteSize { Word, Block }

        public Interruptor(Processor processor, VirtualMemory virtualMemory, FileManager fileManager)
        {
            Processor = processor;
            VirtualMemory = virtualMemory;
            FileManager = fileManager;
        }

        public bool Interrupt()
        {
            int PI = Processor.GetChoiceRegisterIntValue("PI");
            int SI = Processor.GetChoiceRegisterIntValue("SI");
            int TI = Processor.GetHexRegisterIntValue("TI");

            bool programCanContinue = true;

            bool showMessage = false;
            showMessage = true;

            if (PI != 0) programCanContinue = CheckPI(PI, programCanContinue, showMessage);

            if (SI != 0)
            {
                //Console.Write("SUPERVISOR INTERRUPT: ");
                switch (SI)
                {
                    case 1: programCanContinue &= InputBlockInterrupt(); break;
                    case 2: programCanContinue &= InputWordInterrupt(); break;
                    case 3: programCanContinue &= OutputBlockInterrupt(); break;
                    case 4: programCanContinue &= OutputWordInterrupt(); break;
                    case 5: programCanContinue &= FileOpenInterrupt(); break;
                    case 6: programCanContinue &= FileCloseInterrupt(); break;
                    //case 7: programCanContinue &= FileDeleteInterrupt(); break;
                    //case 8: programCanContinue &= FileWriteInterrupt(); break;
                    //case 9: programCanContinue &= FileReadInterrupt(); break;
                    case 10: programCanContinue &= ProgramEndInterrupt(); break;

                }
            }
            PI = Processor.GetChoiceRegisterIntValue("PI");
            if (PI != 0) programCanContinue = CheckPI(PI, programCanContinue, showMessage);

            if (TI == 0)
            {
                Console.WriteLine("TIMER INTERRUPT");
                programCanContinue &= TimerInterrupt();
            }

            return programCanContinue;
        }

        private bool CheckPI(int PI, bool programCanContinue, bool showMessage)
        {
            if (showMessage) Console.Write("PROGRAM INTERRUPT: ");
            switch (PI)
            {
                case 1: programCanContinue &= IncorrectAddressInterrupt(showMessage); break;
                case 2: programCanContinue &= IncorrectCommandCodeInterrupt(showMessage); break;
                case 3: programCanContinue &= IncorrectArgumentTypeInterrupt(showMessage); break;
                case 4: programCanContinue &= IncorrectAssignInterrupt(showMessage); break;
                case 5: programCanContinue &= OverflowInterrupt(showMessage); break;
                case 6: programCanContinue &= FailedFileOpenInterrupt(showMessage); break;
                case 7: programCanContinue &= FailedFileReadInterrupt(showMessage); break;
                case 8: programCanContinue &= FailedFileWriteInterrupt(showMessage); break;
                case 9: programCanContinue &= FailedFileCloseInterrupt(showMessage); break;
                case 10: programCanContinue &= FailedFileDeleteInterrupt(showMessage); break;
                case 11: programCanContinue &= OpenFileLimitReachedInterrupt(showMessage); break;
                case 12: programCanContinue &= BadFileNameInterrupt(showMessage); break;
            }

            return programCanContinue;
        }

        private void ResetSIRegister() => Processor.SetChoiceRegisterValue("SI", 0);

        private bool IncorrectAddressInterrupt(bool show = false)
        {
            if (show) Console.WriteLine("Incorrect address.");
            return false;
        }
        private bool IncorrectCommandCodeInterrupt(bool show = false)
        {
            if (show) Console.WriteLine("Incorrect command code.");
            return false;
        }
        private bool IncorrectArgumentTypeInterrupt(bool show = false)
        {
            if (show) Console.WriteLine("Incorrect arguments for aritmetic command.");
            return false;
        }
        private bool IncorrectAssignInterrupt(bool show = false)
        {
            if (show) Console.WriteLine("Incorrect assign.");
            return false;
        }
        private bool OverflowInterrupt(bool show = false)
        {
            if (show) Console.WriteLine("Overflow.");
            return false;

        }
        private bool FailedFileOpenInterrupt(bool show = false)
        {
            if (show) Console.WriteLine("File open failed.");
            return false;

        }
        private bool FailedFileReadInterrupt(bool show = false)
        {
            if (show) Console.WriteLine("File read failed.");
            return false;

        }
        private bool FailedFileWriteInterrupt(bool show = false)
        {
            if (show) Console.WriteLine("File write failed.");
            return false;

        }
        private bool FailedFileCloseInterrupt(bool show = false)
        {
            if (show) Console.WriteLine("File close failed.");
            return false;

        }
        private bool FailedFileDeleteInterrupt(bool show = false)
        {
            if (show) Console.WriteLine("File delete failed.");
            return false;

        }
        private bool OpenFileLimitReachedInterrupt(bool show = false)
        {
            if (show) Console.WriteLine("Reached open file limit.");
            return false;

        }
        private bool BadFileNameInterrupt(bool show = false)
        {
            if (show) Console.WriteLine("Bad file name. Can be too long.");
            return false;

        }

        private int[] GetCurrentCommandLastTwoArguments()
        {
            char[] command = VirtualMemory.GetValue(Processor.GetICRegisterValue());
            return new int[] { command[2].ToString().HexToInt(), command[3].ToString().HexToInt() };
        }
        private int GetCurrentCommandLastArgument()
        {
            char[] command = VirtualMemory.GetValue(Processor.GetICRegisterValue());
            return command[3].ToString().HexToInt();
        }
        private int GetCurrentCommandSecondLastArgument()
        {
            char[] command = VirtualMemory.GetValue(Processor.GetICRegisterValue());
            return command[2].ToString().HexToInt();
        }
        private char GetCurrentCommandSecondLastArgumentChar()
        {
            char[] command = VirtualMemory.GetValue(Processor.GetICRegisterValue());
            return command[2];
        }

        private bool InputBlockInterrupt()
        {
            int x = GetCurrentCommandLastArgument();
            Processor.UseChannelTool(0, VirtualMemory.GetPager().GetCellRealAddress(x * Utility.BLOCK_SIZE), 4, 1);
            ResetSIRegister();
            return true;
        }
        private bool InputWordInterrupt()
        {
            int r = GetCurrentCommandSecondLastArgument();
            switch (r)
            {
                case 1:
                    Processor.UseChannelTool(0, 0, 5, 6);
                    break;
                case 2:
                    Processor.UseChannelTool(0, 0, 5, 7);
                    break;
                default: return false;
            }
            ResetSIRegister();
            return true;
        }
        private bool OutputBlockInterrupt()
        {
            int x = GetCurrentCommandLastArgument();
            Processor.UseChannelTool(VirtualMemory.GetPager().GetCellRealAddress(x * Utility.BLOCK_SIZE), 0, 1, 4);
            ResetSIRegister();
            return true;
        }
        private bool OutputWordInterrupt()
        {
            int r = GetCurrentCommandSecondLastArgument();
            switch (r)
            {
                case 1:
                    Processor.UseChannelTool(0, 0, 6, 5);
                    break;
                case 2:
                    Processor.UseChannelTool(0, 0, 7, 5);
                    break;
                default: return false;
            }
            ResetSIRegister();
            return true;
        }
        private bool FileOpenInterrupt()
        {
            int x = GetCurrentCommandLastArgument();
            char r = GetCurrentCommandSecondLastArgumentChar();

            char[][] block = VirtualMemory.GetBlockValues(x * Utility.BLOCK_SIZE);

            int fileIndex = FileManager.OpenFile(block, r);
            if (fileIndex == -1) return false;

            Processor.SetRegisterValue("R1", fileIndex.IntToHex());
            ResetSIRegister();
            return true;
        }private bool FileCloseInterrupt()
        {
            int x = GetCurrentCommandLastArgument();
            char r = GetCurrentCommandSecondLastArgumentChar();

            char[][] block = VirtualMemory.GetBlockValues(x * Utility.BLOCK_SIZE);

            int fileIndex = FileManager.OpenFile(block, r);
            if (fileIndex == -1) return false;

            Processor.SetRegisterValue("R1", fileIndex.IntToHex());
            ResetSIRegister();
            return true;
        }


        private bool ProgramEndInterrupt() => false;

        private bool TimerInterrupt()
        {
            Processor.SetHexRegisterValue("TI", Utility.TIMER_VALUE);
            return true;
        }


        public void SetVirtualMemory(VirtualMemory virtualMemory) => VirtualMemory = virtualMemory;
    }
}
