using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FudanFPGA.CrossUI.Web.Models
{
    public class FPGAResponse
    {
        public string Message { get; set; }
        public bool Status { get; set; }
        public ushort[] Data { get; set; }
    }
}
