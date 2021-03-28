using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using System.Text.RegularExpressions;
using Wonton.Common;
using Wonton.CrossUI.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Wonton.CrossUI.Web.Services;

namespace Wonton.CrossUI.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FPGAController : ControllerBase
    {
        private readonly ILogger<FPGAController> _logger;
        private readonly FPGAManager _manager;
        private Dictionary<string, int> inputDict;
        private Dictionary<string, int> outputDict;

        public FPGAController(ILogger<FPGAController> logger, FPGAManager manager)
        {
            _logger = logger;
            _manager = manager;
            inputDict = new Dictionary<string, int>()
            {
                {"P151", 0},
                {"P148", 1},
                {"P150", 2},
                {"P152", 3},
                {"P160", 4},
                {"P161", 5},
                {"P162", 6},
                {"P163", 7},
                {"P164", 8},
                {"P165", 9},
                {"P166", 10},
                {"P169", 11},
                {"P173", 12},
                {"P174", 13},
                {"P175", 14},
                {"P191", 15},
                {"P120", 16},
                {"P116", 17},
                {"P115", 18},
                {"P114", 19},
                {"P113", 20},
                {"P112", 21},
                {"P111", 22},
                {"P108", 23},
                {"P102", 24},
                {"P101", 25},
                {"P100", 26},
                {"P97", 27},
                {"P96", 28},
                {"P95", 29},
                {"P89", 30},
                {"P88", 31},
                {"P87", 32},
                {"P86", 33},
                {"P81", 34},
                {"P75", 35},
                {"P74", 36},
                {"P70", 37},
                {"P69", 38},
                {"P68", 39},
                {"P64", 40},
                {"P62", 41},
                {"P61", 42},
                {"P58", 43},
                {"P57", 44},
                {"P49", 45},
                {"P47", 46},
                {"P48", 47},
                {"P192", 48},
                {"P193", 49},
                {"P199", 50},
                {"P200", 51},
                {"P201", 52},
                {"P202", 53}
            };
            outputDict = new Dictionary<string, int>()
            {
                {"P7", 0},
                {"P6", 1},
                {"P5", 2},
                {"P4", 3},
                {"P9", 4},
                {"P8", 5},
                {"P16", 6},
                {"P15", 7},
                {"P11", 8},
                {"P10", 9},
                {"P20", 10},
                {"P18", 11},
                {"P17", 12},
                {"P22", 13},
                {"P21", 14},
                {"P23", 15},
                {"P44", 16},
                {"P45", 17},
                {"P46", 18},
                {"P43", 19},
                {"P40", 20},
                {"P41", 21},
                {"P42", 22},
                {"P33", 23},
                {"P34", 24},
                {"P35", 25},
                {"P36", 26},
                {"P30", 27},
                {"P31", 28},
                {"P24", 29},
                {"P27", 30},
                {"P29", 31},
                {"P110", 32},
                {"P109", 33},
                {"P99", 34},
                {"P98", 35},
                {"P94", 36},
                {"P93", 37},
                {"P84", 38},
                {"P83", 39},
                {"P82", 40},
                {"P73", 41},
                {"P71", 42},
                {"P63", 43},
                {"P60", 44},
                {"P59", 45},
                {"P56", 46},
                {"P55", 47},
                {"P167", 48},
                {"P168", 49},
                {"P176", 50},
                {"P187", 51},
                {"P189", 52},
                {"P194", 53}
            };
        }

        [HttpGet("initio")]
        public FPGAResponse InitIO(int writeCount, int readCount)
        {
            var b = _manager.Board;
            b.InitIO(writeCount, readCount);
            _logger.LogInformation("initio成功");
            return new FPGAResponse()
            {
                Message = "成功",
                Status = true
            };
        }

        [HttpGet("program")]
        public FPGAResponse Program(string bitfile)
        {
            var b = _manager.Board;
            try
            {
                var r = b.Program(bitfile);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Program失败");
                //Electron.Notification.Show(new NotificationOptions("馄饨FPGA", "Program失败"));
                return new FPGAResponse()
                {
                    Message = e.Message,
                    Status = false
                };
            }
            _logger.LogInformation("Program成功");
            string logpath = Path.Combine(FPGAManager.GetConfigDir(), "WriteReadLog.txt");
            if (System.IO.File.Exists(logpath))
            {
                System.IO.File.Delete(logpath);
            }
            //Electron.Notification.Show(new NotificationOptions("馄饨FPGA", "Program成功"));
            return new FPGAResponse()
            {
                Message = "成功",
                Status = true
            };
        }

        [HttpGet("ioopen")]
        public FPGAResponse IoOpen()
        {
            var b = _manager.Board;
            try
            {
                var r = b.IoOpen();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "ioopen失败");
                //Electron.Notification.Show(new NotificationOptions("馄饨FPGA", "FPGA未连接"));
                return new FPGAResponse()
                {
                    Message = e.Message,
                    Status = false
                };
            }
            _logger.LogInformation("ioopen成功");
            return new FPGAResponse()
            {
                Message = "成功",
                Status = true
            };
        }

        [HttpGet("ioclose")]
        public FPGAResponse IoClose()
        {
            var b = _manager.Board;
            try
            {
                var r = b.IoClose();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "ioclose失败");
                return new FPGAResponse()
                {
                    Message = e.Message,
                    Status = false
                };
            }
            _logger.LogInformation("ioclose成功");
            return new FPGAResponse()
            {
                Message = "成功",
                Status = true
            };
        }

        [HttpPost("writereadfpga")]
        public async Task<FPGAResponse> WriteReadFPGA([FromBody] ushort[] write)
        {
            var b = _manager.Board;
            b.WriteBuffer.Span.Clear();
            write.CopyTo(b.WriteBuffer.Span);

            try
            {
                b.WriteReadData();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "writereadfpga失败");
                return new FPGAResponse()
                {
                    Message = e.Message,
                    Status = false
                };
            }

            ushort[] read = b.ReadBuffer.ToArray();
            string logpath = Path.Combine(FPGAManager.GetConfigDir(), "WriteReadLog.txt");
            var log = new FileStream(logpath, FileMode.Append, FileAccess.Write);
            var writer = new StreamWriter(log);

            await writer.WriteAsync("Write:");
            foreach (ushort i in write)
                await writer.WriteAsync(i + " ");
            await writer.WriteAsync("\nRead:");

            foreach (ushort i in read)
                await writer.WriteAsync(i + " ");
            await writer.WriteAsync("\n");

            writer.Close();
            log.Close();

            return new FPGAResponse()
            {
                Message = "成功",
                Status = true,
                Data = b.ReadBuffer.ToArray()
            };
        }

        [HttpGet("setprojectfile")]
        public FPGAResponse SetProjectFile(string filename)
        {
            _logger.LogInformation("设置项目: " + filename);
            _manager.CurrentProjectFile = filename;
            return new FPGAResponse()
            {
                Message = filename,
                Status = true,
                ProjectPath = filename
            };
        }

        /// <summary>
        /// 初始化程序
        /// 如果CurrentProjectFile提供了hwproj文件路径,则读取这个文件并把数据返回给UI
        /// 如果CurrentProjectFile没有提供hwproj文件路径则不读取文件,UI显示开始页面
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        [HttpGet("init")]
        public async Task<FPGAResponse> Init()
        {
            //_ = Task.Run(async () => await ElectronIPC.CheckUpdateAsync());

            var filename = "";
            if (string.IsNullOrEmpty(_manager.CurrentProjectFile))
            {
                //未指定打开文件文件
                if (string.IsNullOrEmpty(Web.Program.LaunchingProject))
                {
                    //也没有指定启动文件
                    //保持为空
                }
                else
                {
                    //指定了启动文件
                    filename = Web.Program.LaunchingProject;
                }
            }
            else
            {
                //已经指定打开文件
                filename = _manager.CurrentProjectFile;
            }

            if (string.IsNullOrEmpty(filename))
            {
                //没有项目文件
                //显示开始界面
                //返回RecentProjects
                _logger.LogInformation("没有项目文件");
                await _manager.ReadRecentProjectAsync();

                return new FPGAResponse()
                {
                    Message = _manager.RecentProjectsRaw,
                    Status = false
                };
            }
            _logger.LogInformation("打开项目文件: " + filename);
            var fs = System.IO.File.Open(filename, FileMode.OpenOrCreate);
            var sr = new StreamReader(fs);
            var content = await sr.ReadToEndAsync();

            sr.Close();
            fs.Close();

            //ReadPortsMap();

            //将项目信息添加到RecentProjects
            JObject pjJobj = JObject.Parse(content);
            var pjName = pjJobj["projectName"].Value<string>();
            await _manager.ReadRecentProjectAsync();
            _manager.AddRecentProject(pjName, filename);
            await _manager.SaveRecentProjectAsync();

            return new FPGAResponse()
            {
                Message = content,
                Status = true,
                ProjectPath = filename,
                IsDarkMode =
#if Windows
                DarkMode.InDarkMode
#else
                false
#endif
            };
        }

        [HttpPost("writejson")]
        public async Task<FPGAResponse> WriteJson([FromQuery] string filename, [FromBody] ProjectInfo data)
        {
            await System.IO.File.WriteAllTextAsync(filename, data.data);
            _logger.LogInformation("已保存项目: " + filename);
            //Electron.Notification.Show(new NotificationOptions("馄饨FPGA", "已保存"));

            return new FPGAResponse()
            {
                Message = "成功",
                Status = true
            };
        }

        //没用上？？？
        [HttpGet("readxmltojson")]
        public FPGAResponse ReadXmlToJson(string filename)
        {
            var xml = new XmlDocument();
            xml.Load(filename);
            _logger.LogInformation("Read XML");

            XmlNode design = xml.SelectSingleNode("design");
            var json = JsonConvert.SerializeXmlNode(design);

            return new FPGAResponse()
            {
                Message = json,
                Status = true
            };
        }

        [HttpGet("newproject")]
        public async Task<FPGAResponse> NewProject(string projectdir, string projectname, string projectiofile)
        {
            var fullpath = Path.Combine(projectdir, projectname + ".hwproj");
            _logger.LogInformation("新建项目: " + fullpath);
            XmlDocument xml = new XmlDocument();
            xml.Load(projectiofile);

            JObject jports = new JObject();

            XmlNode design = xml.SelectSingleNode("design");
            var ports = design.ChildNodes;

            for (int i = 0; i < ports.Count; i++)
            {
                var p = ports[i];
                var attr = p.Attributes;
                var name = attr.GetNamedItem("name").Value;
                var pos = attr.GetNamedItem("position").Value;

                jports.Add(name, pos);
            }

            JObject pj = new JObject();
            pj.Add("subscribedInstances", new JObject());
            pj.Add("hardwarePortsMap", new JObject());
            pj.Add("inputPortsMap", new JObject());
            pj.Add("projectInstancePortsMap", new JObject());
            pj.Add("layout", new JArray());
            pj.Add("projectPortsMap", jports);
            pj.Add("portsIndexMap", new JObject());
            pj.Add("xml", projectiofile);
            pj.Add("bitfile", "");
            pj.Add("projectName", projectname);

            var json = pj.ToString();
            await System.IO.File.WriteAllTextAsync(fullpath, json);

            _manager.CurrentProjectFile = fullpath;

            return new FPGAResponse()
            {
                Message = fullpath,
                Status = true,
                ProjectPath = fullpath
            };
        }

        [HttpGet("waveform")]
        public async Task<FPGAResponse> Waveform(int runhz, string portsmap)
        {
            //ReadPortsMap(); //因为每一次io传输都会新建一个fpgaController对象，所以要即建即用

            Dictionary<string, string> ports = JsonConvert.DeserializeObject<Dictionary<string, string>>(portsmap);
            var inputPortsIndexDict = new Dictionary<int, string>();
            var outputPortsIndexDict = new Dictionary<int, string>();
            foreach (KeyValuePair<string, string> i in ports)
            {
                if (inputDict.ContainsKey(i.Value))
                    inputPortsIndexDict.Add(inputDict[i.Value], i.Key);
                else if (outputDict.ContainsKey(i.Value))
                    outputPortsIndexDict.Add(outputDict[i.Value], i.Key);
            }

            string mappath = Path.Combine(FPGAManager.GetConfigDir(), "PortsIndexMap.txt");
            var maplog = new FileStream(mappath, FileMode.Create, FileAccess.Write);
            var mapwriter = new StreamWriter(maplog);
            //调试输出变量名和引脚序号的映射
            await mapwriter.WriteLineAsync("Input:");
            foreach (KeyValuePair<int, string> i in inputPortsIndexDict)
                await mapwriter.WriteLineAsync(i.Key + " " + i.Value);
            await mapwriter.WriteLineAsync("Output:");
            foreach (KeyValuePair<int, string> i in outputPortsIndexDict)
                await mapwriter.WriteLineAsync(i.Key + " " + i.Value);
            mapwriter.Close();
            maplog.Close();

            //变成vcd
            string logpath = Path.Combine(FPGAManager.GetConfigDir(), "WriteReadLog.txt");
            //string vcdpath = Path.Combine(FPGAManager.GetConfigDir(), "VCDLog.txt");
            string waveformpath = Path.Combine(FPGAManager.GetConfigDir(), "Waveform.vcd");
            var log = new FileStream(logpath, FileMode.Open, FileAccess.Read);
            var reader = new StreamReader(log);
            var vcdlog = new FileStream(waveformpath, FileMode.Create, FileAccess.Write);
            var writer = new StreamWriter(vcdlog);

            await writer.WriteAsync("$timescale " + 1000 / (double)runhz + " ms\n$end\n");
            foreach (KeyValuePair<int, string> i in inputPortsIndexDict)
                await writer.WriteLineAsync("$var wire 1 " + i.Value + " " + i.Value + " $end");
            foreach (KeyValuePair<int, string> i in outputPortsIndexDict)
                await writer.WriteLineAsync("$var wire 1 " + i.Value + " " + i.Value + " $end");
            await writer.WriteAsync("$enddefinitions $end\n$dumpvars\n");
            foreach (KeyValuePair<int, string> i in inputPortsIndexDict)
                await writer.WriteLineAsync("b0 " + i.Value);
            foreach (KeyValuePair<int, string> i in outputPortsIndexDict)
                await writer.WriteLineAsync("b0 " + i.Value);
            await writer.WriteLineAsync("$end");

            string line;
            int cycle = 0;
            string writepattern = @"Write:(\d*) (\d*) (\d*) (\d*)"; //4个int16的排列方式是15-0 31-16 47-32 63-48
            string readpattern = @"Read:(\d*) (\d*) (\d*) (\d*)";
            Regex writereg = new Regex(writepattern);
            Regex readreg = new Regex(readpattern);
            List<int> writehist = new List<int>(new int[64]);
            List<int> readhist = new List<int>(new int[64]);
            while ((line = await reader.ReadLineAsync()) != null)
            {
                Match writematch = writereg.Match(line);
                if (writematch.Success)
                {
                    List<ushort> tempfornum = new List<ushort>(); //4个int16
                    for (int i = 1; i < writematch.Groups.Count; i++)
                        tempfornum.Add(ushort.Parse(writematch.Groups[i].ToString()));
                    List<int> splitdata = SplitRawData(tempfornum); //64位
                    var dataforprint = new Dictionary<int, int>();
                    dataforprint = FilterRepetition(writehist, splitdata);
                    if (dataforprint.Count != 0)
                    {
                        await writer.WriteLineAsync("#" + cycle);
                        await writer.WriteAsync(PrintVcd(inputPortsIndexDict, dataforprint, cycle));
                    }
                    writehist = splitdata;
                }
                Match readmatch = readreg.Match(line);
                if (readmatch.Success)
                {
                    List<ushort> tempfornum = new List<ushort>();
                    for (int i = 1; i < readmatch.Groups.Count; i++)
                        tempfornum.Add(ushort.Parse(readmatch.Groups[i].ToString()));
                    List<int> splitdata = SplitRawData(tempfornum); //64位
                    var dataforprint = FilterRepetition(readhist, splitdata);
                    if (dataforprint.Count != 0)
                    {
                        await writer.WriteLineAsync("#" + cycle);
                        await writer.WriteAsync(PrintVcd(outputPortsIndexDict, dataforprint, cycle));
                    }
                    readhist = splitdata;

                    cycle++;
                }
            }
            reader.Close();
            log.Close();
            writer.Close();
            vcdlog.Close();
#if DEBUG
            var t = new RunExeByProcess();
            t.ProcessName = "gtkwave";
            t.Argument = waveformpath;
            t.Execute();
            //if (code != 0)
                //_logger.LogError("gtkwave error!");
#else
            var t = new RunExeByProcess();
            var path = Path.Combine(System.Environment.CurrentDirectory, "gtkwave", "bin", "gtkwave");
            t.ProcessName = path;
            t.Argument = waveformpath;
            //Console.WriteLine(t.Execute());
            t.Execute();
            //if (code != 0)
                //_logger.LogError("gtkwave error!");
#endif

            return new FPGAResponse()
            {
                Message = "成功",
                Status = true
            };
        }

        internal List<int> SplitRawData(List<ushort> rawdata) //rawdata是4个uint16
        {
            var ans = new List<int>();
            for (int i = 0; i < 4; i++)
            {
                ushort tempnum = rawdata[i];
                for (int j = 0; j < 16; j++)
                {
                    //int index = i * 16 + j;
                    int split = (tempnum >> j) & 1;
                    ans.Add(split);
                }
            }
            //foreach (int i in ans)
            //Console.Write(i + " ");
            return ans;
        }

        internal Dictionary<int, int> FilterRepetition(List<int> prev, List<int> now)
        {
            var ans = new Dictionary<int, int>();
            if (prev.Count != 64 || now.Count != 64)
            {
                _logger.LogError("Wrong split for data!");
                return null;
            }
            for (int i = 0; i < 64; i++)
            {
                //Console.WriteLine(prev[i] + " " + now[i]);
                if (prev[i] != now[i])
                {
                    ans.Add(i, now[i]);
                    //Console.WriteLine("Success!");
                }
            }
            //Console.WriteLine("new dict");
            //foreach (KeyValuePair<int, int> i in ans)
            //Console.WriteLine(i.Key + " " + i.Value);
            return ans;
        }

        internal string PrintVcd(Dictionary<int, string> dict, Dictionary<int, int> data, int time)
        {
            string ans = "";
            foreach (KeyValuePair<int, int> i in data)
            {
                if (dict.ContainsKey(i.Key))
                {
                    string t = "b" + i.Value + " " + dict[i.Key] + "\n";
                    ans += t;
                }
            }
            return ans;
        }
    }

    class RunExeByProcess
    {
        public string ProcessName { get; set; }
        //public string ObjectPath { get; set; }
        public string Argument { get; set; }
        internal void Execute()
        {
            //var tcs = new TaskCompletionSource<int>();
            var process = new System.Diagnostics.Process();
            process.StartInfo.FileName = ProcessName;
            process.StartInfo.Arguments = Argument;
            process.StartInfo.UseShellExecute = false;
            //不显示exe的界面
            process.StartInfo.CreateNoWindow = true;
            //process.EnableRaisingEvents = true;
            /*
            process.Exited += (sender, args) =>
            {
                tcs.SetResult(process.ExitCode);
                process.Dispose();
            };
            await Task.Run(() => process.Start());
            return tcs.Task.Result;
            */
            process.Start();
            //await Task.Run(() => process.Start());
            //return process.ExitCode;
        }
    }
}