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
                          "> ");

            int sw = int.Parse(Console.ReadLine());
            switch (sw)
            {
                case 1:
                    StartBackupAsync();
                    break;
                case 2:
                    StartWinget();
                    break;
            }
        }

        // Winget install Process
        private static void StartWinget()
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
            Process process = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = "cmd.exe";
            startInfo.Arguments = $"/C winget import -i {files[0]} --accept-source-agreements --accept-package-agreements";
            process.StartInfo = startInfo;
            process.Start();
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
                Console.Write("Confirmation to backup all program name as TXT file: ");
                string answer = Console.ReadLine();
                if (answer.ToLower() == "y")
                {
                    ProgConf = false;
                    // Get all app installed 32 / 64
                    List<string> keys = new List<string>() { @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall", @"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall" };

                    InstallApp.FindInstalls(RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64), keys);
                    InstallApp.FindInstalls(RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry64), keys);
                }
            }

            var WinGetConf = true;
            while (WinGetConf)
            {
                Console.Write("Confirmation to create a Winget file: ");
                string answer = Console.ReadLine();
                if (answer.ToLower() == "y")
                {
                    WinGetConf = false;
                    // Make a winGet export 
                    Process process = new Process();
                    ProcessStartInfo startInfo = new ProcessStartInfo();
                    startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    startInfo.FileName = "cmd.exe";
                    startInfo.Arguments = $"/C winget export -o '{BackUpPath}\\WinGet.json'";
                    process.StartInfo = startInfo;
                    startInfo.CreateNoWindow = true;
                    process.Start();
                }
            }

            // Wait Confirmation to start
            var conf = true;
            while (conf)
            {
                Console.Write("Confirmation before starting type Y to start or close this program: ");
                string answer = Console.ReadLine();
                if (answer.ToLower() == "y")
                {
                    conf = false;
                }
            }

            // Staring Detection / Copy
            if (!conf)
            {
                List<string> list = KnownFolders.GetPath();
                string[] str = list.ToArray();
                await DataCopy.FFCopy(str);

                Console.WriteLine("Press any key to Exit.");
                Console.ReadKey();
            }
        }

    }
}