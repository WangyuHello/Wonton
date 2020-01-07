using System;
using System.Collections.Generic;
using System.Text;

namespace FudanFPGA.Common
{
    public class FPGAManager
    {
        public FPGABoard Board { get; private set; }

        public string CurrentProjectFile { get; set; }

        public FPGAManager()
        {
            Board = new FPGABoard();
        }
    }
}
