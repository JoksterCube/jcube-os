using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jCubeOS_CMD
{
    abstract class Input
    {
        public abstract byte[][] ReadBlock();
        public abstract byte[] ReadWord();
    }
}
