using Microsoft.Win32;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace BackUpDesktop
{
    internal class Program
    {
        public static string BackUpPath { get; private set; }

        [STAThread]
        static async Task Main(string[] args)
        {
            Console.Write("1: Backup\n" +
                          "2: Winget install\n" +
                          "3: Chocolatey install\n" +
                          "> ");

            int sw = int.Parse(Console.ReadLine());
            switch (sw)
            {
                case 1:
                    StartBackupAsync();
                    break;
                case 2:
                    StartWingetAsync();
                    break;
                case 3:
                    StartChocoInstallAsync();
                        break;
            }
        }

        // choco install <path-to-exported-file> -y
        // choco upgrade chocolatey -y

        // Set-ExecutionPolicy Bypass -Scope Process -Force; [System.Net.ServicePointManager]::SecurityProtocol = [System.Net.ServicePointManager]::SecurityProtocol -bor 3072; iwr https://community.chocolatey.org/install.ps1 -UseBasicParsing | iex

        // Winget install Process
        private static async Task StartWingetAsync()
        {
            Console.WriteLine("Select foler where is saved Winget.json");

            // Select Back up folder
            var dlg = new FolderPicker();
            dlg.InputPath = @"c:";
            dlg.Title = "Auto Winget";
            if (dlg.ShowDialog(owner: IntPtr.Zero) == true)
            {
                BackUpPath = dlg.ResultPath;
            }

            string[] files = Directory.GetFiles(BackUpPath, "WinGet.json", SearchOption.TopDirectoryOnly);
            Console.WriteLine("Winget.json file found ! {0}", files[0]);
            Console.WriteLine("Please wait!");
            Thread.Sleep(3000);

            await Exec("cmd.exe", "winget import -i {files[0]} --accept-source-agreements --accept-package-agreements");
            // winget install -e -h --accept-source-agreements --accept-package-agreements --id $app.name
        }

        // Backup Process
        private static async Task StartBackupAsync()
        {
            Console.WriteLine("Select your backup folder");

            // Select Back up folder
            var dlg = new FolderPicker();
            dlg.InputPath = @"c:";
            dlg.Title = "Auto BackUp";
            if (dlg.ShowDialog(owner: IntPtr.Zero) == true)
            {
                BackUpPath = dlg.ResultPath;
            }

            Console.WriteLine("BackUp Path : " + BackUpPath);

            var ProgConf = true;
            while (ProgConf)
            {
                Console.Write("Confirmation to backup all program name as TXT file (Y)es | (N)o: ");
                string answer = Console.ReadLine();
                if (answer.ToLower() == "y")
                {
                    ProgConf = false;
                    // Get all app installed 32 / 64
                    List<string> keys = new List<string>() { @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall", @"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall" };

                    InstallApp.FindInstalls(RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64), keys);
                    InstallApp.FindInstalls(RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry64), keys);
                }
                else
                {
                    break;
                }
            }

            var WinGetConf = true;
            while (WinGetConf)
            {
                Console.Write("Confirmation to create a Winget export file (Y)es | (N)o: ");
                string answer = Console.ReadLine();
                if (answer.ToLower() == "y")
                {
                    WinGetConf = false;
                    // Make a winGet export 
                    await Exec("cmd.exe", $"winget export -o {BackUpPath}\\WinGet.json");
                }
                else
                {
                    break;
                }
            }

            var ChocoConf = true;
            while (ChocoConf)
            {
                Console.Write("Confirmation to create a Chocolatey export file (Y)es | (N)o: ");
                string answer = Console.ReadLine();
                if (answer.ToLower() == "y")
                {
                    ChocoConf = false;
                    // Chocolatey install
                    await Exec("powershell.exe", "Set-ExecutionPolicy Bypass -Scope Process -Force; [System.Net.ServicePointManager]::SecurityProtocol = [System.Net.ServicePointManager]::SecurityProtocol -bor 3072; iwr https://community.chocolatey.org/install.ps1 -UseBasicParsing | iex");
                    // Make a Chocolatey export 
                    await Exec("cmd.exe", $"choco export --output-file-path=\"'{BackUpPath}\\ChocolateyBackup.config'\"");
                }
                else
                {
                    break;
                }
            }

            // Wait Confirmation to start
            var conf = true;
            while (conf)
            {
                Console.Write("Confirmation before starting type Y to start or close this program (Y)es | (N)o: ");
                string answer = Console.ReadLine();
                if (answer.ToLower() == "y")
                {
                    conf = false;
                    List<string> list = KnownFolders.GetPath();
                    string[] str = list.ToArray();
                    await DataCopy.FFCopy(str);

                    Console.WriteLine("Press any key to Exit.");
                    Console.ReadKey();
                }
                else
                {
                    break;
                }
            }
        }

        private static async Task StartChocoInstallAsync()
        {
            Console.WriteLine("Select foler where is saved packages.config");

            // Select Back up folder
            var dlg = new FolderPicker();
            dlg.InputPath = @"c:";
            dlg.Title = "Auto Chocolatey";
            if (dlg.ShowDialog(owner: IntPtr.Zero) == true)
            {
                BackUpPath = dlg.ResultPath;
            }

            string[] files = Directory.GetFiles(BackUpPath, "*.config", SearchOption.TopDirectoryOnly);
            Console.WriteLine("ChocolateyBackup.config file found ! {0}", files[0]);
            Console.WriteLine("Please wait!");
            Thread.Sleep(3000);
            await Exec("powershell.exe", "Set-ExecutionPolicy Bypass -Scope Process -Force; [System.Net.ServicePointManager]::SecurityProtocol = [System.Net.ServicePointManager]::SecurityProtocol -bor 3072; iwr https://community.chocolatey.org/install.ps1 -UseBasicParsing | iex");

            await Exec("cmd.exe", $"choco install {BackUpPath}\\packages.config");
        }

        private static Task Exec(string filename, string cmd)
        {
            Process process = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.WindowStyle = ProcessWindowStyle.Normal;
            startInfo.FileName = filename;
            startInfo.Arguments = $"/C " + cmd;
            process.StartInfo = startInfo;
            //startInfo.CreateNoWindow = true;
            process.Start();
            process.WaitForExit();
            process.Close();
            return Task.CompletedTask;
        }


    }
}