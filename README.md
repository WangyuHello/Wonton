# SharpVLFD
复旦FPGA开发板C#编程接口 

VeriLink interface for C#

# 如何安装 | How to install

## 从Nuget包管理安装 | from the Nuget Package Manager
```powershell
Install-Package SharpVLFD -Version 0.1.3
```

## 从.NET命令行安装 | from .NET CLI
```powershell
dotnet add package SharpVLFD --version 0.1.3
```

# 如何使用 | How to use

简单的反相器示例

Simple inverter example

```C#
using System;
using static VLFD.VLFDInterop;

namespace Inverter
{
    class Programm
    {
        const int NOW_USE_BOARD = 0;

        static void Main(string[] args)
        {
            SimpleInverter();
        }

        static void SimpleInverter()
        {
            const int DATA_RUN_COUNT = 1024;
            
            var WriteBuffer = new Span<ushort>(new ushort[DATA_RUN_COUNT]);
            var ReadBuffer = new Span<ushort>(new ushort[DATA_RUN_COUNT]);
            int DataCount;

            DataCount = DATA_RUN_COUNT;

            WriteBuffer.Fill(0);
            ReadBuffer.Fill(0);

            if (!VLFD_ProgramFPGA(NOW_USE_BOARD, @"inverter_fde_dc.bit"))
            {
                Console.WriteLine(VLFD_GetLastErrorMsg(NOW_USE_BOARD));
                Environment.Exit(1);
            }

            if (!VLFD_AppOpen(NOW_USE_BOARD, "F4YF-K2II-Y0Z0-AT05-F805-A478"))
            {
                Console.WriteLine("VLFD_AppOpen error.");
                Console.WriteLine(VLFD_GetLastErrorMsg(NOW_USE_BOARD));
                Environment.Exit(1);
            }

            for (ushort i = 0; i < DataCount; i++)
                WriteBuffer[i] = i;

            if (!VLFD_AppFIFOWriteData(NOW_USE_BOARD, WriteBuffer))
            {
                Console.WriteLine("WriteData failed!");
                Console.WriteLine(VLFD_GetLastErrorMsg(NOW_USE_BOARD));
            }

            if (!VLFD_AppFIFOReadData(NOW_USE_BOARD, ReadBuffer.Slice(0, DataCount / 2)))
            {
                Console.WriteLine("error: VLFD_AppFIFOReadData");
                Console.WriteLine(VLFD_GetLastErrorMsg(NOW_USE_BOARD));
            }

            if (!VLFD_AppFIFOReadData(NOW_USE_BOARD, ReadBuffer.Slice(DataCount / 2, DataCount / 2)))
            {
                Console.WriteLine("error: VLFD_AppFIFOReadData");
                Console.WriteLine(VLFD_GetLastErrorMsg(NOW_USE_BOARD));
            }

            for (int i = 0; i < DataCount; i++)
            {
                Console.WriteLine($"[{i}] {WriteBuffer[i]} -> {ReadBuffer[i]}");
            }

            var bRet = VLFD_AppClose(NOW_USE_BOARD);
            if (!bRet)
            {
                Console.WriteLine("error: VLFD_AppClose");
                Console.WriteLine(VLFD_GetLastErrorMsg(NOW_USE_BOARD));
            }
            else
            {
                Console.WriteLine("VLFD_AppClose OK!");
            }
        }
    }
}
```