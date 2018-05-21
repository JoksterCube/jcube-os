using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jCubeOS_CMD
{
    abstract class Output
    {
        public abstract void WriteBlock(byte[][] block);
        public abstract void WriteWord(byte[] word);
    }
}
