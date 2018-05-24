using jCubeOS_CMD.Real;
using System;
using System.Collections.Generic;
using System.Text;

namespace jCubeOS_CMD
{
    class Interruptor
    {
        private Processor Processor { get; set; }

        private enum ReadWriteSize { Word, Block }

        public Interruptor(Processor processor) => Processor = processor;

        public bool Interrupt()
        {
            int PI = Processor.GetChoiceRegisterIntValue("PI");
            int SI = Processor.GetChoiceRegisterIntValue("SI");
            int TI = Processor.GetHexRegisterIntValue("TI");

            if (PI != 0)
            {
                switch (PI)
                {

                }
            }
            if (SI != 0)
            {
                switch (SI)
                {
                    case 1:
                    case 2:
                    case 3:
                    case 4:
                        return InputOutputInterrupt();
                    case 6:
                        return ProgramEndInterrupt();

                }
            }

            if (TI == 0) return TimerInterrupt();

            return true;
        }

        private bool TimerInterrupt()
        {
            Processor.SetHexRegisterValue("TI", Utility.TIMER_VALUE);
            return true;
        }

        private bool InputOutputInterrupt()
        {
            Processor.SetChoiceRegisterValue("SI", 0);
            Processor.ExecuteChannelToolXCHGCommand();
            return true;
        }

        private bool ProgramEndInterrupt() => false;
    }
}
