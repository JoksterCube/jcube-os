﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jCubeOS_CMD
{
    /// <summary>
    /// Real machine processor
    /// </summary>
    class Processor
    {
        private RealMemory RealMemory { get; set; }
        private VirtualMemory VirtualMemory { get; set; }
        private Pager Pager { get; set; }

        private Dictionary<string, Register> registers;

        public Processor(RealMemory realMemory, VirtualMemory virtualMemory = null, Pager pager = null)
        {
            RealMemory = realMemory;
            VirtualMemory = virtualMemory;
            Pager = pager;

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

        public Register GetRegisterValue(string registerName)
        {
            return registers[registerName];
        }

        public void SetRegisterValue(string registerName, byte[] value)
        {
            registers[registerName].SetValue(value);
        }

        public void SetRegisterValue(string registerName, string value)
        {
            registers[registerName].SetValue(value);
        }

        public void SetRegisterValue(string registerName, int value)
        {
            SetRegisterValue(registerName, Utility.IntToBytes(value, registers[registerName].GetSize()));
        }

        public void SetICRegisterValue(int value)
        {

        }

        public void SetVirtualMemory(VirtualMemory virtualMemory, Pager pager)
        {
            VirtualMemory = virtualMemory;
            Pager = pager;
        }

        /// <summary>
        /// Executes one command
        /// </summary>
        /// <returns>true if successful and false if failed</returns>
        public bool Step()
        {
            //string value = VirtualMemoryCode[registers["IC"].GetValue()];
            return false;
        }

        /// <summary>
        /// Executes as long as it does.
        /// </summary>
        /// <returns>true if successful and false if failed</returns>
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
