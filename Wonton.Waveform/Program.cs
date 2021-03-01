using System;
using Wonton.Common;
using System.IO;
using System.Diagnostics;

namespace Wonton.Waveform
{
    class Program
    {
        public static void Main(string[] args)
        {
            const int delta_time = 1;
            FPGABoard b = new FPGABoard();
            b.InitIO(4, 4);
            b.Program(@"H:\bishe\csharptest\AlarmClock_fde_dc.bit");

            b.IoOpen();

            var log = new FileStream(@"H:\bishe\Wonton\Wonton.Waveform\log.txt", FileMode.Create, FileAccess.Write);
            var writer = new StreamWriter(log);

            for (int i = 0; i < 700; i++)
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

                Console.WriteLine($"[{i}] hr_out[{hr_out}] min_out[{min_out}] sec_out[{sec_out}] hr_alarm[{hr_alarm}] min_alarm[{min_alarm}] alarm[{alarm}]");
                writer.WriteLine($"#{i * delta_time} hr_out {hr_out} 4");
                writer.WriteLine($"#{i * delta_time} min_out {min_out} 6");
                writer.WriteLine($"#{i * delta_time} sec_out {sec_out} 6");
                writer.WriteLine($"#{i * delta_time} hr_alarm {hr_alarm} 4");
                writer.WriteLine($"#{i * delta_time} min_alarm {min_alarm} 6");
                writer.WriteLine($"#{i * delta_time} alarm {alarm} 1");
            }
            writer.Flush();
            writer.Close();
            log.Close();

            b.IoClose();

            var t = new RunExeByProcess();
            t.ProcessName = "vcdmaker";
            t.ObjectPath = @"H:\bishe\Wonton\Wonton.Waveform\log.txt";
            t.TargetPath = @"H:\bishe\Wonton\Wonton.Waveform\test.vcd";
            t.Argument = "-t us -v -o " + t.TargetPath + " " + t.ObjectPath;
            Console.WriteLine(t.Argument);
            Console.WriteLine(t.Execute());

            t.ProcessName = "gtkwave";
            t.ObjectPath = @"H:\bishe\Wonton\Wonton.Waveform\test.vcd";
            t.Argument = t.TargetPath;
            Console.WriteLine(t.Execute());
        }
    }
    class RunExeByProcess
    {
        public string ProcessName { get; set; }
        public string ObjectPath { get; set; }
        public string TargetPath { get; set; }
        public string Argument { get; set; }
        public string Result { get; set; }
        public string Execute()
        {
            var process = new Process();
            process.StartInfo.FileName = ProcessName;
            process.StartInfo.Arguments = Argument;
            process.StartInfo.UseShellExecute = false;
            //不显示exe的界面
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.RedirectStandardOutput = true;
            //process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.RedirectStandardError = true;
            process.Start();
            //向cmd窗口发送输入信息
            //process.StandardInput.WriteLine(argument + "&exit");

            //process.StandardInput.AutoFlush = true;
            Result = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            if (process.ExitCode != 0)
            {
                throw new VCDException(process.StandardError.ReadToEnd());
            }
            process.Close();
            return Result;
        }
    }
    public class VCDException : Exception
    {
        public VCDException(string message) : base(message) { }

        public VCDException()
        {
        }

        public VCDException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
