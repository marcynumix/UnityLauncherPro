using System.Diagnostics;
using System.IO;
using System.Windows;
using System;

namespace UnityLauncherPro
{
    internal class NumixTools
    {
        //NUMIX
        public static void CleanProject(Project proj)
        {
            //Run git clean -dxf in project folder
            var proc = new Process();
            proc.StartInfo.FileName = "git";
            proc.StartInfo.Arguments = "clean -dxf";
            proc.StartInfo.WorkingDirectory = proj.Path;
            proc.Start();
        }

        public static void ResetProject(Project proj)
        {
            //Run git reset --hard in project folder
            var proc = new Process();
            proc.StartInfo.FileName = "git";
            proc.StartInfo.Arguments = "reset --hard";
            proc.StartInfo.WorkingDirectory = proj.Path;
            proc.Start();
        }

        public static void ProjectStatus(Project proj)
        {
            //Get git status and display in message box
            var proc = new Process();
            proc.StartInfo.FileName = "git";
            proc.StartInfo.Arguments = "status";
            proc.StartInfo.WorkingDirectory = proj.Path;
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.RedirectStandardOutput = true;
            proc.StartInfo.RedirectStandardError = true;
            proc.StartInfo.CreateNoWindow = true;
            proc.Start();
            proc.WaitForExit();

            string output = proc.StandardOutput.ReadToEnd();
            string error = proc.StandardError.ReadToEnd();

            if (error != "")
            {
                MessageBox.Show(error);
            }
            else
            {
                MessageBox.Show(output);
            }
        }

        public static void OpenWithFork(Project proj)
        {
            //Run fork located in %localappdata%\fork\fork.exe            
            var proc = new Process();
            proc.StartInfo.FileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "fork", "fork.exe");
            proc.StartInfo.Arguments = proj.Path;
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.RedirectStandardOutput = true;
            proc.StartInfo.RedirectStandardError = true;
            proc.StartInfo.CreateNoWindow = true;
            proc.Start();
        }

        public static void OpenGitlabPage(Project proj)
        {
            var projectPath = proj.Path;

            string url = GetGitlabURL(projectPath);

            //Open in browser
            if (url != null)
            {
                Tools.OpenURL(url);
            }
        }

        public static void OpenGitlabCICDPage(Project proj)
        {
            var projectPath = proj.Path;

            string url = GetGitlabURL(projectPath).Replace(".git", "/pipelines");

            //Open in browser
            if (url != null)
            {
                Tools.OpenURL(url);
            }
        }

        public static void EditBuildVersion(Project proj)
        {
            var projectPath = proj.Path;

            string filePath = Path.Combine(projectPath, "..", "build_version.txt");

            //Open in notepad
            if (File.Exists(filePath))
            {
                var proc = new Process();
                proc.StartInfo.FileName = "notepad";
                proc.StartInfo.Arguments = filePath;
                proc.StartInfo.UseShellExecute = false;
                proc.StartInfo.RedirectStandardOutput = true;
                proc.StartInfo.RedirectStandardError = true;
                proc.StartInfo.CreateNoWindow = true;
                proc.Start();
            }

        }

        public static void ApplyBuildVersion(Project proj)
        {
            var projectPath = proj.Path;

            var rootFolder = Path.Combine(projectPath, "..");

            //build_version_SET.ps1

            //Execute build_version_SET.ps1 powershell script
            var proc = new Process();
            proc.StartInfo.FileName = "powershell";
            proc.StartInfo.Arguments = "-ExecutionPolicy Bypass -File build_version_SET.ps1";
            proc.StartInfo.WorkingDirectory = rootFolder;
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.RedirectStandardOutput = true;
            proc.StartInfo.RedirectStandardError = true;
            proc.StartInfo.CreateNoWindow = true;
            proc.Start();
            proc.WaitForExit();
        }

        public static void OpenWithGitBash(Project proj)
        {
            var projectPath = proj.Path;

            //run C:\\Program Files\\Git\\bin\\bash.exe
            var proc = new Process();
            proc.StartInfo.FileName = "C:\\Program Files\\Git\\bin\\bash.exe";
            proc.StartInfo.WorkingDirectory = projectPath;
            proc.Start();
        }

        private static string GetGitlabURL(string projectPath)
        {
            string result = null;

            string dirName = Path.Combine(projectPath, ".git");

            if (!Directory.Exists(dirName))
            {
                // check if its subfolder
                dirName = Path.Combine(projectPath, "..", ".git");
            }

            //Read origin url from .git/config
            if (Directory.Exists(dirName))
            {

                string configPath = Path.Combine(dirName, "config");
                if (File.Exists(configPath))
                {
                    string[] lines = File.ReadAllLines(configPath);
                    foreach (string line in lines)
                    {

                        if (line.Contains("url ="))
                        {
                            result = line.Split('=')[1].Trim();
                            break;
                        }
                    }
                }
            }

            if (result != null && result.Contains("ssh"))
            {
                result = result.Replace("ssh://git@", "https://");
                result = result.Replace(":30001", "/");
            }

            return result;
        }
    }
}
