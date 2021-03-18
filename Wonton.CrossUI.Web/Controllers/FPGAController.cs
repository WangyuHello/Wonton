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
        private Dictionary<string, ushort> inputDict;
        private Dictionary<string, ushort> outputDict;

        public FPGAController(ILogger<FPGAController> logger, FPGAManager manager)
        {
            _logger = logger;
            _manager = manager;
            //ReadPortsMap();
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
                await writer .WriteAsync(i + " ");
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
            _logger.LogInformation("打开项目文件: "+ filename);
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
        public async Task<FPGAResponse> WriteJson([FromQuery]string filename, [FromBody]ProjectInfo data)
        {
            await System.IO.File.WriteAllTextAsync(filename, data.data);
            _logger.LogInformation("已保存项目: "+ filename);
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
            _logger.LogInformation("新建项目: "+fullpath);
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
            pj.Add("subscribedInstances",new JObject());
            pj.Add("hardwarePortsMap", new JObject());
            pj.Add("inputPortsMap", new JObject());
            pj.Add("projectInstancePortsMap", new JObject());
            pj.Add("layout",new JArray());
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
        public async Task<FPGAResponse> Waveform(string portsMap)
        {
            ReadPortsMap(); //因为每一次io传输都会新建一个fpgaController对象，所以要即建即用
            Dictionary<string, string> ports = JsonConvert.DeserializeObject<Dictionary<string, string>>(portsMap);
            var inputPortsIndexDict = new Dictionary<ushort, string>();
            var outputPortsIndexDict = new Dictionary<ushort, string>();
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
            foreach (KeyValuePair<ushort, string> i in inputPortsIndexDict)
                await mapwriter.WriteLineAsync(i.Key + " " + i.Value);
            await mapwriter.WriteLineAsync("Output:");
            foreach (KeyValuePair<ushort, string> i in outputPortsIndexDict)
                await mapwriter.WriteLineAsync(i.Key + " " + i.Value);
            mapwriter.Close();
            maplog.Close();
            /*
            int total; //将同一变量的不同位合并
            Dictionary<string>
            if (i.Key != "clk")
            {
                Regex reg = new Regex("(\S)[(\d)]");
                Match match = reg.Match(i.Key);
                if (match.Success)
                {

                }
            }
        }*/
            string logpath = Path.Combine(FPGAManager.GetConfigDir(), "WriteReadLog.txt");
            string vcdpath = Path.Combine(FPGAManager.GetConfigDir(), "VCDLog.txt");
            string waveformpath = Path.Combine(FPGAManager.GetConfigDir(), "Waveform.vcd");
            var log = new FileStream(logpath, FileMode.Open, FileAccess.Read);
            var reader = new StreamReader(log);
            var vcdlog = new FileStream(vcdpath, FileMode.Create, FileAccess.Write);
            var writer = new StreamWriter(vcdlog);
            string line;
            int cycle = 0;
            while ((line = await reader.ReadLineAsync()) != null)
            {
                string writepattern = @"Write:(\d*) (\d*) (\d*) (\d*)"; //4个int16的排列方式是15-0 31-16 47-32 63-48
                string readpattern = @"Read:(\d*) (\d*) (\d*) (\d*)";
                Regex writereg = new Regex(writepattern);
                Regex readreg = new Regex(readpattern);
                Match writematch = writereg.Match(line);
                if (writematch.Success)
                {
                    List<ushort> tempfornum = new List<ushort>();
                    for (int i = 1; i < writematch.Groups.Count; i++)
                        tempfornum.Add(ushort.Parse(writematch.Groups[i].ToString()));
                    await writer.WriteAsync(SplitLog(inputPortsIndexDict, tempfornum, cycle));
                }
                Match readmatch = readreg.Match(line);
                if (readmatch.Success)
                {
                    List<ushort> tempfornum = new List<ushort>();
                    for (int i = 1; i < readmatch.Groups.Count; i++)
                        tempfornum.Add(ushort.Parse(readmatch.Groups[i].ToString()));
                    await writer.WriteAsync(SplitLog(outputPortsIndexDict, tempfornum, cycle));
                    cycle++;
                }
            }
            reader.Close();
            log.Close();
            writer.Close();
            vcdlog.Close();

            var t = new RunExeByProcess();
            t.ProcessName = "vcdmaker";
            t.ObjectPath = vcdpath;
            t.TargetPath = waveformpath;
            t.Argument = "-t us -v -o " + t.TargetPath + " " + t.ObjectPath;
            Console.WriteLine(t.Argument);
            Console.WriteLine(t.Execute());

            t.ProcessName = "gtkwave";
            t.ObjectPath = waveformpath;
            t.Argument = t.TargetPath;
            Console.WriteLine(t.Execute());

            return new FPGAResponse()
            {
                Message = "成功",
                Status = true
            };
        }
        
        internal string SplitLog(Dictionary<ushort, string> dict, List<ushort> num, int time)
        {
            string ans = "";
            int cycle = dict.Count() / 16; //默认向下取整
            //Console.WriteLine("Cycle = " + cycle);
            for (int i = 0; i <= cycle; i++)
            {
                ushort tempnum = num[i];
                for (int j = 0; j < 16; j++)
                {
                    ushort index = (ushort)(i * 16 + j);
                    if (dict.ContainsKey(index))
                    {
                        ushort split = (ushort)((tempnum >> j) & 1);
                        string t = "#" + time + " " + dict[index] + " " + split + " 1\n";
                        ans += t;
                    }
                }
            }
            return ans;
        }
        
        //读取引脚名和引脚序号的映射
        internal void ReadPortsMap()
        {
            this.inputDict = new Dictionary<string, ushort>();
            this.outputDict = new Dictionary<string, ushort>();
            _logger.LogInformation("读取PortsMap");
            var portsmapfile = @"./Services/FPGAPortsMap.js";
            var fs = System.IO.File.Open(portsmapfile, FileMode.Open);
            var sr = new StreamReader(fs);
            var content = sr.ReadToEnd();
            JObject portsmapJobj = JObject.Parse(content);
            var inputportsmap = portsmapJobj["inputPortsMapping"];
            var outputportsmap = portsmapJobj["outputPortsMapping"];
            foreach (var i in inputportsmap)
                inputDict.Add(i[0].ToString(), i[1].ToObject<ushort>());
            foreach (var i in outputportsmap)
                outputDict.Add(i[0].ToString(), i[1].ToObject<ushort>());
            sr.Close();
            fs.Close();
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
            var process = new System.Diagnostics.Process();
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
    class VCDException : Exception
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