using jCubeOS_CMD.Registers;
using jCubeOS_CMD.Virtual;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jCubeOS_CMD.Real
{
    class Processor
    {
        private RealMemory RealMemory { get; set; }
        private ChannelTool ChannelTool { get; set; }
        private VirtualMemory VirtualMemory { get; set; }
        private Pager Pager { get; set; }
        private CommandInterpretator CommandInterpretator { get; set; }

        private Dictionary<string, Register> registers;

        public Processor(RealMemory realMemory, ChannelTool channelTool, VirtualMemory virtualMemory = null, Pager pager = null)
        {
            RealMemory = realMemory;
            VirtualMemory = virtualMemory;
            ChannelTool = channelTool;
            Pager = pager;
            CommandInterpretator = new CommandInterpretator(this, virtualMemory);

            registers = new Dictionary<string, Register>
            {
                { "R1", new Register() },										// Word length general register
                { "R2", new Register() },										// Word length general register
                { "IC", new HexRegister(2) },									// Current command adress in memory register
                { "PTR", new HexRegister(4) },									// Page table adress register
                { "SF", new StatusFlagRegister() },								// Aritmetic operation logic values
                { "MODE", new ChoiceRegister('N', 'S') },						// Processor mode "N" - user, "S" - supervisor
                { "PI", new ChoiceRegister(0, 1, 2, 3, 4, 5, 6, 7, 8, 9) },		// Program interuptor
                { "SI", new ChoiceRegister(0, 1, 2, 3, 4) },					// Supervisor interuptor
                { "TI", new ChoiceRegister(0, 1) }								// Timer interuptor
            };
        }

        public void UseChannelTool(int SB, int DB, int ST, int DT)
        {
            ChannelTool.SetRegisters(SB, DB, ST, DT);
            ChannelTool.XCHG();
        }

        public Register GetRegister(string registerName)
        {
            if (HasRegister(registerName)) return registers[registerName];
            else throw new Exception("Processor does not have register named " + registerName);
        }

        public char[] GetRegisterValue(string registerName) => GetRegister(registerName).GetValue();

        public void SetRegisterValue(string registerName, char[] value)
        {
            if (HasRegister(registerName)) GetRegister(registerName).SetValue(value);
            else throw new Exception("Processor does not have register named " + registerName);
        }

        public void SetHexRegisterValue(string registerName, int value)
        {
            if (!HasRegister(registerName)) throw new Exception("Processor does not have register named " + registerName);
            else if (GetRegister(registerName) is HexRegister) ((HexRegister)registers[registerName]).SetValue(value);
            else throw new Exception("Register " + registerName + " is not HexRegister type.");
        }

        private bool HasRegister(string registerName) => registers.ContainsKey(registerName);

        public void SetICRegisterValue(int value) => ((HexRegister)registers["IC"]).SetValue(value);

        public int GetICRegisterValue() => ((HexRegister)registers["IC"]).GetIntValue();

        public void SetPTRRegisterValue(int ptr)
        {
            ptr += (Utility.VIRTUAL_MEMORY_BLOCKS - 1) * 16 * 16 * 16 + (Utility.BLOCK_SIZE - 1) * 16 * 16;
            ((HexRegister)registers["PTR"]).SetValue(ptr);
        }

        public void SetVirtualMemory(VirtualMemory virtualMemory, Pager pager)
        {
            VirtualMemory = virtualMemory;
            Pager = pager;
            SetPTRRegisterValue(pager.GetPTR());
            CommandInterpretator.SetVirtualMemory(virtualMemory);
        }

        public bool Step()
        {
            char[] stepCommand = VirtualMemory.GetValue(GetICRegisterValue());
            string stringCommand = new String(stepCommand);
            Console.WriteLine("Step: " + stringCommand);

            CommandInterpretator.ParseCommand(stepCommand);

            return false;
        }

        public bool Execute()
        {
            while (Step())
            {
                continue;
            }
            return false;
        }
    }
}
