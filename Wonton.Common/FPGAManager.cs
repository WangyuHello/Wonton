using System;
using System.Collections.Generic;
using System.Text;

namespace Wonton.Common
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
