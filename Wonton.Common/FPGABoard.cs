using System;
using System.Collections.Generic;
using System.Text;
using VLFD;

namespace Wonton.Common
{
    public class FPGABoard
    {
        public string BitfilePath { get; set; }
        public Memory<ushort> WriteBuffer { get; set; }
        public Memory<ushort> ReadBuffer { get; set; }

        private int NOW_USE_BOARD = 0;
        private const string SerialNo = "F4UP-G2NH-Y0M0-AC05-F805-A478";

        public void InitIO(int writeCount, int readCount)
        {
            WriteBuffer = new Memory<ushort>(new ushort[writeCount]);
            ReadBuffer = new Memory<ushort>(new ushort[readCount]);
        }

        public bool Program()
        {
            if (string.IsNullOrEmpty(BitfilePath))
            {
                throw new Exception("BitfilePath为空");
            }

            var r = VLFDInterop.VLFD_ProgramFPGA(NOW_USE_BOARD, BitfilePath);

            if (r==false)
            {
                var msg = VLFDInterop.VLFD_GetLastErrorMsg(NOW_USE_BOARD);
                throw new FPGAException(msg);
            }

            return true;
        }

        public bool Program(string bitfile)
        {
            BitfilePath = bitfile;
            return Program();
        }

        public bool IoOpen()
        {
            var r = VLFDInterop.VLFD_IO_Open(NOW_USE_BOARD, SerialNo);

            if (r == false)
            {
                var msg = VLFDInterop.VLFD_GetLastErrorMsg(NOW_USE_BOARD);
                throw new FPGAException(msg);
            }
            return true;
        }

        public bool WriteReadData()
        {
            var r = VLFDInterop.VLFD_IO_WriteReadData(NOW_USE_BOARD, WriteBuffer.Span, ReadBuffer.Span);
            if (r == false)
            {
                var msg = VLFDInterop.VLFD_GetLastErrorMsg(NOW_USE_BOARD);
                throw new FPGAException(msg);
            }
            return true;
        }

        public bool IoClose()
        {
            var r = VLFDInterop.VLFD_IO_Close(NOW_USE_BOARD);

            if (r == false)
            {
                var msg = VLFDInterop.VLFD_GetLastErrorMsg(NOW_USE_BOARD);
                throw new FPGAException(msg);
            }

            return true;
        }
    }
}
