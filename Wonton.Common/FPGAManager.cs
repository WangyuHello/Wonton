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

        private string RecentProjectFile = "RecentProjects.json";
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
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                //如果是Windows则保存到%LocalAppData%目录中
                var localappdata = Environment.GetEnvironmentVariable("LocalAppData");
                var localwonton = Path.Combine(localappdata, "Wonton");
                if (!Directory.Exists(localwonton))
                {
                    Directory.CreateDirectory(localwonton);
                }
                RecentProjectFile = Path.Combine(localwonton, RecentProjectFile);
            }

            var json = JsonConvert.SerializeObject(RecentProjects, Formatting.Indented);
            await File.WriteAllTextAsync(RecentProjectFile, json).ConfigureAwait(false);
        }

        public async Task ReadRecentProjectAsync()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                //如果是Windows则保存到%LocalAppData%目录中
                var localappdata = Environment.GetEnvironmentVariable("LocalAppData");
                var localwonton = Path.Combine(localappdata, "Wonton");
                if (!Directory.Exists(localwonton))
                {
                    Directory.CreateDirectory(localwonton);
                }
                RecentProjectFile = Path.Combine(localwonton, RecentProjectFile);
            }

            if (File.Exists(RecentProjectFile))
            {
                RecentProjectsRaw = await File.ReadAllTextAsync(RecentProjectFile).ConfigureAwait(false);
                RecentProjects = JsonConvert.DeserializeObject<List<(string projectName, string projectFile)>>(RecentProjectsRaw);
            }
        }
    }
}
