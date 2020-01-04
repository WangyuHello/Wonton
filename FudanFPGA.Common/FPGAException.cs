using System;
using System.Collections.Generic;
using System.Text;

namespace FudanFPGA.Common
{
    public class FPGAException : Exception
    {
        public FPGAException(string message) : base(message) { }
    }
}
