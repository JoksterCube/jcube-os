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
        private Input Input { get; set; }
        private Output Output { get; set; }

        private Dictionary<string, Register> registers;

        public ChannelTool(RealMemory realMemmory, ExternalMemory externalMemory, Input inputHandler, Output outputHandler)
        {
            RealMemory = realMemmory;
            ExternalMemory = externalMemory;
            Input = inputHandler;
            Output = outputHandler;
            registers = new Dictionary<string, Register> {
                { "SB", new HexRegister() },				// Copy source block address
                { "DB", new HexRegister() },				// Copy destination block address
                { "ST", new ChoiceRegister(1,2,3,4) },		// Source object number: 1-user memory, 2-supervisor, 3-external, 4-input
                { "DT", new ChoiceRegister(1,2,3,4) },		// Destination object number: 1-user memory, 2-supervisor, 3-external, 4-output
            };
        }

        public void SetRegisters(int SB, int DB, int ST, int DT)
        {
            ((HexRegister)registers["SB"]).SetValue(SB);
            ((HexRegister)registers["DB"]).SetValue(DB);
            ((ChoiceRegister)registers["ST"]).SetValue(ST);
            ((ChoiceRegister)registers["DT"]).SetValue(DT);
        }

        public void XCHG()
        {

        }
    }
}
