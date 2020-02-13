using System;
using System.Collections.Generic;
using System.Text;

namespace Wonton.Common
{
    public class FPGAException : Exception
    {
        public FPGAException(string message) : base(message) { }
    }
}
