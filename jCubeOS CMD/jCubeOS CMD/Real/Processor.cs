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
        private Interruptor Interruptor { get; set; }
        private FileManager FileManager { get; set; }

        private Dictionary<string, Register> registers;

        private bool UseMaxStep { get; set; }
        private int CurrentStep { get; set; }

        public bool ChangedIC { get; set; }

        public Processor(RealMemory realMemory, ChannelTool channelTool, VirtualMemory virtualMemory = null, Pager pager = null)
        {
            RealMemory = realMemory;
            VirtualMemory = virtualMemory;
            ChannelTool = channelTool;
            Pager = pager;
            FileManager = new FileManager(RealMemory, this);
            CommandInterpretator = new CommandInterpretator(this, virtualMemory);
            Interruptor = new Interruptor(this, virtualMemory, FileManager, RealMemory);

            // STOPS PROGRAM AFTER MAX STEP COUNT
            UseMaxStep = true;
            // CURRENT STEP
            CurrentStep = 0;

            registers = new Dictionary<string, Register>
            {
                { "R1", new Register(value: 0) },								            // Word length general register
                { "R2", new Register(value: 0) },								            // Word length general register
                { "IC", new HexRegister(2) },									            // Current command adress in memory register
                { "PTR", new HexRegister(4) },									            // Page table adress register
                { "SF", new StatusFlagRegister() },								            // Aritmetic operation logic values
                { "MODE", new ChoiceRegister('N', 'S') },						            // Processor mode "N" - user, "S" - supervisor
                { "PI", new ChoiceRegister(0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12) },		// Program interuptor
                { "SI", new ChoiceRegister(0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12) },	    // Supervisor interuptor
                { "TI", new HexRegister(Utility.TIMER_VALUE, 1) }				            // Timer interuptor
            };
        }

        public void SetChannelToolRegisterValues(int SB, int DB, int ST, int DT) => ChannelTool.SetRegisters(SB, DB, ST, DT);

        public void ExecuteChannelToolXCHGCommand() => ChannelTool.XCHG();

        public void UseChannelTool(int SB, int DB, int ST, int DT)
        {
            SetChannelToolRegisterValues(SB, DB, ST, DT);
            ExecuteChannelToolXCHGCommand();
        }

        private bool HasRegister(string registerName) => registers.ContainsKey(registerName);

        public Register GetRegister(string registerName)
        {
            if (HasRegister(registerName)) return registers[registerName];
            else throw new Exception("Processor does not have register named " + registerName);
        }

        public char[] GetRegisterValue(string registerName) => GetRegister(registerName).GetValue();

        public void SetRegisterValue(string registerName, char[] value) => GetRegister(registerName).SetValue(value);

        public void SetChoiceRegisterValue(string registerName, char[] value)
        {
            if (GetRegister(registerName) is ChoiceRegister) ((ChoiceRegister)GetRegister(registerName)).SetValue(value);
            else throw new Exception("Register " + registerName + " is not ChoiceRegister type.");
        }

        public void SetChoiceRegisterValue(string registerName, int value)
        {
            if (GetRegister(registerName) is ChoiceRegister) ((ChoiceRegister)GetRegister(registerName)).SetValue(value);
            else throw new Exception("Register " + registerName + " is not ChoiceRegister type.");
        }

        public int GetChoiceRegisterIntValue(string registerName)
        {
            if (GetRegister(registerName) is ChoiceRegister) return ((ChoiceRegister)GetRegister(registerName)).GetIntValue();
            else throw new Exception("Register " + registerName + " is not ChoiceRegister type.");
        }

        public void SetHexRegisterValue(string registerName, int value)
        {
            if (GetRegister(registerName) is HexRegister) ((HexRegister)GetRegister(registerName)).SetValue(value);
            else throw new Exception("Register " + registerName + " is not HexRegister type.");
        }

        public int GetHexRegisterIntValue(string registerName)
        {
            if (GetRegister(registerName) is HexRegister) return ((HexRegister)GetRegister(registerName)).GetIntValue();
            else throw new Exception("Register " + registerName + " is not HexRegister type.");
        }

        public void SetSFRegisterFlag(string flag, bool value) => ((StatusFlagRegister)GetRegister("SF")).SetFlagValue(flag, value);

        public bool GetSFRegisterFlag(string flag) => ((StatusFlagRegister)GetRegister("SF")).GetFlagValue(flag);

        public void SetICRegisterValue(int value) => ((HexRegister)GetRegister("IC")).SetValue(value);

        public int GetICRegisterValue() => ((HexRegister)GetRegister("IC")).GetIntValue();

        public void IncICRegisterValue(int offset = 1) => ((HexRegister)GetRegister("IC")).AddValue(offset);

        public void DecTIRegisterValue(int offset = 1) => ((HexRegister)GetRegister("TI")).AddValue(-offset);

        public void SetPTRRegisterValue(int ptr)
        {
            ptr += (Utility.VIRTUAL_MEMORY_BLOCKS - 1) * 16 * 16 * 16 + (Utility.BLOCK_SIZE - 1) * 16 * 16;
            ((HexRegister)GetRegister("PTR")).SetValue(ptr);
        }

        public void SetVirtualMemory(VirtualMemory virtualMemory, Pager pager)
        {
            VirtualMemory = virtualMemory;
            Pager = pager;
            SetPTRRegisterValue(pager.GetPTR());
            CommandInterpretator.SetVirtualMemory(virtualMemory);
            Interruptor.SetVirtualMemory(virtualMemory);
        }

        public void SetSupervisorMode() => ((ChoiceRegister)GetRegister("MODE")).SetValue('S');

        public void SetUserMode() => ((ChoiceRegister)GetRegister("MODE")).SetValue('N');

        public bool Step()
        {
            char[] stepCommand = VirtualMemory.GetValue(GetICRegisterValue());
            string stringCommand = new String(stepCommand);
            Console.WriteLine("STEP: " + stringCommand);
            bool commandResult = CommandInterpretator.ParseCommand(stepCommand);

            if (Test()) if (!Interrupt()) { FileManager.CloseAll(); return false; }

            if (ChangedIC) ChangedIC = false;
            else IncICRegisterValue(1);

            if (UseMaxStep && CurrentStep++ >= Utility.MAX_STEPS) return StopProgram();

            return commandResult;
        }

        public bool Execute()
        {
            while (Step()) continue;
            return false;
        }

        public void PrintAllRegisters()
        {
            Console.WriteLine("-----------Registers-----------");
            Console.WriteLine("R1: " + new String(GetRegisterValue("R1")));
            Console.WriteLine("R2: " + new String(GetRegisterValue("R2")));
            Console.WriteLine("IC: " + new String(GetRegisterValue("IC")));
            Console.WriteLine("PTR: " + new String(GetRegisterValue("PTR")));
            Console.WriteLine("SF: " + ((StatusFlagRegister)GetRegister("SF")).ToString());
            Console.WriteLine("MODE: " + ((char)(((ChoiceRegister)GetRegister("MODE")).GetIntValue())));
            Console.WriteLine("PI: " + new String(GetRegisterValue("PI")));
            Console.WriteLine("SI: " + new String(GetRegisterValue("SI")));
            Console.WriteLine("TI: " + new String(GetRegisterValue("TI")));
            Console.WriteLine("-----------Registers-----------");
        }

        private bool Interrupt()
        {
            SetSupervisorMode();
            if (Interruptor.Interrupt())
            {
                SetUserMode();
                return true;
            }
            else return false;
        }

        private bool Test()
        {
            int PI = GetChoiceRegisterIntValue("PI");
            int SI = GetChoiceRegisterIntValue("SI");
            int TI = GetHexRegisterIntValue("TI");
            return PI + SI > 0 || TI == 0;
        }

        private bool StopProgram()
        {
            Console.WriteLine("Max step count of " + Utility.MAX_STEPS + " was reached. Program was stoped.");
            return false;
        }
    }
}
