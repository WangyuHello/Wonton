using System;
using System.Diagnostics;
using FudanFPGA.Common;
using Xunit;

namespace FudanFPGA.Test
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            FPGABoard b = new FPGABoard();
            b.InitIO(4,4);
            b.Program(@"E:\Documents\Repo\ProjectFDB\FudanFPGAInterface\FudanFPGA.Test\AlarmClock_fde_dc.bit");

            b.IoOpen();

            for (int i = 0; i < 70; i++)
            {
                if (i < 2)
                {
                    b.WriteBuffer.Span[0] = 0x00;
                }
                else
                {
                    b.WriteBuffer.Span[0] = 0x01;
                }

                b.WriteBuffer.Span[1] = 0x00;
                b.WriteBuffer.Span[2] = 0x00;
                b.WriteBuffer.Span[3] = 0x00;

                b.ReadBuffer.Span.Clear();

                b.WriteReadData();

                var hr_out = b.ReadBuffer.Span[0] & 0x000F;
                var min_out = (b.ReadBuffer.Span[0] & 0x03F0) >> 4;
                var sec_out = (b.ReadBuffer.Span[0] & 0xFC00) >> 10;
                var hr_alarm = b.ReadBuffer.Span[1] & 0x000F;
                var min_alarm = (b.ReadBuffer.Span[1] & 0x03F0) >> 4;
                var alarm = (b.ReadBuffer.Span[1] & 0x0400) >> 10;

                Debug.WriteLine($"[{i}] hr_out[{hr_out}] min_out[{min_out}] sec_out[{sec_out}] hr_alarm[{hr_alarm}] min_alarm[{min_alarm}] alarm[{alarm}]");
            }

            b.IoClose();

        }
    }
}
