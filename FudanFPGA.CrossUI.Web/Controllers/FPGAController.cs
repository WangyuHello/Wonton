using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using ElectronNET.API;
using ElectronNET.API.Entities;
using FudanFPGA.Common;
using FudanFPGA.CrossUI.Web.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FudanFPGA.CrossUI.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FPGAController : ControllerBase
    {
        private readonly ILogger<FPGAController> _logger;
        private readonly FPGAManager _manager;

        public FPGAController(ILogger<FPGAController> logger, FPGAManager manager)
        {
            _logger = logger;
            _manager = manager;
        }

        [HttpGet("initio")]
        public FPGAResponse InitIO(int writeCount, int readCount)
        {
            var b = _manager.Board;
            b.InitIO(writeCount, readCount);
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
#if RELEASE
                Electron.Notification.Show(new NotificationOptions("复旦FPGA","Program失败"));
#endif
                return new FPGAResponse()
                {
                    Message = e.Message,
                    Status = false
                };
            }
#if RELEASE
            Electron.Notification.Show(new NotificationOptions("复旦FPGA", "Program成功"));
#endif
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
                return new FPGAResponse()
                {
                    Message = e.Message,
                    Status = false
                };
            }
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
                return new FPGAResponse()
                {
                    Message = e.Message,
                    Status = false
                };
            }
            return new FPGAResponse()
            {
                Message = "成功",
                Status = true
            };
        }

        [HttpPost("writereadfpga")]
        public FPGAResponse WriteReadFPGA([FromBody] ushort[] write)
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
                return new FPGAResponse()
                {
                    Message = e.Message,
                    Status = false
                };
            }
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
            _manager.CurrentProjectFile = filename;
            return new FPGAResponse()
            {
                Message = filename,
                Status = true,
                ProjectPath = filename
            };
        }

        [HttpGet("readjson")]
        public async Task<FPGAResponse> ReadJson(string filename)
        {
            if (string.IsNullOrEmpty(filename))
            {
                //初始化
                filename = _manager.CurrentProjectFile;
            }

            if (string.IsNullOrEmpty(filename))
            {
                //没有项目文件
                return new FPGAResponse()
                {
                    Message = "",
                    Status = true
                };
            }

            var fs = System.IO.File.Open(filename, FileMode.OpenOrCreate);
            var sr = new StreamReader(fs);
            var content = await sr.ReadToEndAsync();

            sr.Close();
            fs.Close();

            return new FPGAResponse()
            {
                Message = content,
                Status = true,
                ProjectPath = filename
            };
        }

        [HttpPost("writejson")]
        public async Task<FPGAResponse> WriteJson([FromQuery]string filename, [FromBody]ProjectInfo data)
        {
            await System.IO.File.WriteAllTextAsync(filename, data.data);

            return new FPGAResponse()
            {
                Message = "成功",
                Status = true
            };
        }

        [HttpGet("readxmltojson")]
        public FPGAResponse ReadXmlToJson(string filename)
        {
            XmlDocument xml = new XmlDocument();
            xml.Load(filename);

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

            pj.Add("bitfile", "");

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
    }
}