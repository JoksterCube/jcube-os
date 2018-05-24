using jCubeOS_CMD.Registers;
using System;
using System.Collections.Generic;
using System.Text;

namespace jCubeOS_CMD.Real
{
    class ChannelTool
    {
        private RealMemory RealMemory { get; set; }
        private ExternalMemory ExternalMemory { get; set; }
        private Input InputHandler { get; set; }
        private Output OutputHandler { get; set; }
        private Processor Processor { get; set; }

        private Dictionary<string, Register> registers;

        public ChannelTool(RealMemory realMemmory, ExternalMemory externalMemory, Input inputHandler, Output outputHandler, Processor processor = null)
        {
            RealMemory = realMemmory;
            ExternalMemory = externalMemory;
            InputHandler = inputHandler;
            OutputHandler = outputHandler;
            Processor = processor;
            registers = new Dictionary<string, Register> {
                { "SB", new HexRegister() },				            // Copy source block address
                { "DB", new HexRegister() },				            // Copy destination block address
                { "ST", new ChoiceRegister(1, 2, 3, 4, 5, 6, 7) },		// Source object number: 1-user memory, 2-supervisor, 3-external, 4-input
                { "DT", new ChoiceRegister(1, 2, 3, 4, 5, 6, 7) },		// Destination object number: 1-user memory, 2-supervisor, 3-external, 4-output
            };
        }

        public void SetProcessor(Processor processor) => Processor = processor;

        public void SetRegisters(int SB, int DB, int ST, int DT)
        {
            ((HexRegister)registers["SB"]).SetValue(SB);
            ((HexRegister)registers["DB"]).SetValue(DB);
            ((ChoiceRegister)registers["ST"]).SetValue(ST);
            ((ChoiceRegister)registers["DT"]).SetValue(DT);
        }

        public void XCHG()
        {
            char[][] sourceBlock = null;
            char[] sourceWord = null;

            int sourceAddress = ((HexRegister)registers["SB"]).GetIntValue();
            int destinationAddress = ((HexRegister)registers["DB"]).GetIntValue();

            int STvalue = ((ChoiceRegister)registers["ST"]).GetIntValue();
            int DTvalue = ((ChoiceRegister)registers["DT"]).GetIntValue();

            if ((STvalue == '5' || STvalue == '6' || STvalue == '7') ^ (DTvalue == '5' || DTvalue == '6' || DTvalue == '7'))
            {
                throw new Exception("Channel tool cannot work with word and block at the same time.");
            }

            switch (STvalue)
            {
                case 1:
                    sourceBlock = RealMemory.GetUserMemoryBlockValues(sourceAddress);
                    break;
                case 2:
                    sourceBlock = RealMemory.GetSupervisorMemoryBlockValues(sourceAddress);
                    break;
                case 3:
                    sourceBlock = ExternalMemory.GetBlockValues(sourceAddress);
                    break;
                case 4:
                    sourceBlock = InputHandler.ReadBlock();
                    break;
                case 5:
                    sourceWord = InputHandler.ReadWord();
                    break;
                case 6:
                    sourceWord = Processor.GetRegisterValue("R1");
                    break;
                case 7:
                    sourceWord = Processor.GetRegisterValue("R2");
                    break;
                default:
                    throw new Exception("Channel tool ST register value is incorrect.");
            }

            switch (DTvalue)
            {
                case 1:
                    RealMemory.SetUserMemoryBlockValues(destinationAddress, sourceBlock);
                    break;
                case 2:
                    RealMemory.SetSupervisorMemoryBlockValues(destinationAddress, sourceBlock);
                    break;
                case 3:
                    ExternalMemory.SetBlockValues(destinationAddress, sourceBlock);
                    break;
                case 4:
                    OutputHandler.WriteBlock(sourceBlock);
                    break;
                case 5:
                    OutputHandler.WriteWord(sourceWord);
                    break;
                case 6:
                    Processor.SetRegisterValue("R1", sourceWord);
                    break;
                case 7:
                    Processor.SetRegisterValue("R2", sourceWord);
                    break;
                default:
                    throw new Exception("Channel tool ST register value is incorrect.");
            }
        }
    }
}
