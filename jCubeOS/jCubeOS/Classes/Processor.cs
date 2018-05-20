using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jCubeOS.Classes
{
    /// <summary>
    /// Real machine processor
    /// </summary>
    class Processor
    {
        private RealMemory RealMemory { get; set; }
        private VirtualMemoryCode VirtualMemoryCode { get; set; }
        private VirtualMemoryData VirtualMemoryData { get; set; }
        private Pager Pager { get; set; }

        private Dictionary<string, Register> registers;

        public Processor(RealMemory realMemory, VirtualMemoryCode virtualMemoryCode = null, VirtualMemoryData virtualMemoryData = null, Pager pager = null)
        {
            RealMemory = realMemory;
            VirtualMemoryCode = virtualMemoryCode;
            VirtualMemoryData = virtualMemoryData;
            Pager = pager;

            registers = new Dictionary<string, Register>
            {
                { "R1", new Register() },										// Word length register
                { "R2", new Register() },										// Word length register
                { "IC", new HexRegister(2) },									// Current command adress in memory register
                { "PTR", new HexRegister(4) },									// Page table adress register
                { "SF", new StatusFlagRegister() },								// Aritmetic operation logic values
                { "MODE", new ChoiceRegister('N', 'S') },						// Processor mode "N" - user, "S" - supervisor
                { "PI", new ChoiceRegister(0, 1, 2, 3, 4, 5, 6, 7, 8, 9) },		// Program interuptor
                { "SI", new ChoiceRegister(0, 1, 2, 3, 4) },					// Supervisor interuptor
                { "TI", new ChoiceRegister(0, 1) }								// Timer interuptor
            };
        }
		
		public void SetVirtualMemory(VirtualMemoryCode virtualMemoryCode, VirtualMemoryData virtualMemoryData, Pager pager)
        {
            VirtualMemoryCode = virtualMemoryCode;
            VirtualMemoryData = virtualMemoryData;
            Pager = pager;
        }

		/// <summary>
        /// Executes one command
        /// </summary>
        /// <returns>true if successful and false if failed</returns>
		public bool Step()
        {
            string value = VirtualMemoryCode[registers["IC"].GetValue()];
        }
    }
}
