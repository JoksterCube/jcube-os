using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jCubeOS_CMD
{
    class Register
    {
        protected Cell Cell { get; set; }

        public Register() => Cell = new Cell();

        public virtual char[] GetValue() => Cell.GetValue();

        public virtual int GetSize() => Cell.GetSize();

        public virtual void SetValue(char[] value) => Cell.SetValue(value);
    }
}
