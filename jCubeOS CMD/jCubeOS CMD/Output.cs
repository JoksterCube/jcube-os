using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jCubeOS_CMD
{
    abstract class Output
    {
        public abstract void WriteBlock(char[][] block);
        public abstract void WriteWord(char[] word);
    }
}
