using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wonton.CrossUI.Web.Models
{
    public class FPGAResponse
    {
        public string Message { get; set; }
        public string ProjectPath { get; set; }
        public bool Status { get; set; }
        public ushort[] Data { get; set; }
    }
}
