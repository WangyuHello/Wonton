using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Wonton.Common
{
    public class FPGAManager
    {
        public FPGABoard Board { get; private set; }

        public string CurrentProjectFile { get; set; }

        public List<(string projectName, string projectFile)> RecentProjects { get; private set; }

        private string RecentProjectFile = Path.Combine(GetConfigDir(),"RecentProjects.json");
        public string RecentProjectsRaw { get; private set; } = "[]"; //初始为空数组

        public FPGAManager()
        {
            Board = new FPGABoard();
            RecentProjects = new List<(string projectName, string projectFile)>();
        }

        public void AddRecentProject(string pjName, string pjFile)
        {
            RecentProjects.RemoveAll(tu => (tu.projectFile == pjFile) && (tu.projectName == pjName));
            RecentProjects.Insert(0, (pjName, pjFile));
            if (RecentProjects.Count > 10)
            {
                RecentProjects.RemoveRange(10, RecentProjects.Count - 10);
            }
        }

        public async Task SaveRecentProjectAsync()
        {
            var json = JsonConvert.SerializeObject(RecentProjects, Formatting.Indented);
            await File.WriteAllTextAsync(RecentProjectFile, json).ConfigureAwait(false);
        }

        public async Task ReadRecentProjectAsync()
        { 
            if (File.Exists(RecentProjectFile))
            {
                RecentProjectsRaw = await File.ReadAllTextAsync(RecentProjectFile).ConfigureAwait(false);
                RecentProjects = JsonConvert.DeserializeObject<List<(string projectName, string projectFile)>>(RecentProjectsRaw);
            }
        }

        public static string GetConfigDir()
        {
            string localwonton = "./";
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                //如果是Windows则保存到%LocalAppData%/Wonton目录中
                var localappdata = Environment.GetEnvironmentVariable("LocalAppData");
                localwonton = Path.Combine(localappdata, "Wonton");
                if (!Directory.Exists(localwonton))
                {
                    Directory.CreateDirectory(localwonton);
                }

            }
            else
            {
                //如果是macOS/Linux则保存到$HOME/.wonton目录中
                var localappdata = Environment.GetEnvironmentVariable("HOME");
                localwonton = Path.Combine(localappdata, ".wonton");
                if (!Directory.Exists(localwonton))
                {
                    Directory.CreateDirectory(localwonton);
                }
            }

            return localwonton;
        }
    }
}
