using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackUpDesktop
{
    internal class DataCopy
    {
        public static Task FFCopy(string[] folders)
        {
            string folderName = "";
            string targetPath = Program.BackUpPath;
            string[] desktop;
            string[] files;
            long TotalSize = 0;

            foreach (string s in folders)
            {
                TotalSize += Info.DirSize(new DirectoryInfo(s));
            }

            Console.WriteLine($"\nTotal Backup Size: {Info.SizeSuffix(TotalSize)}\nPress any key to continue");
            Console.ReadLine();

            foreach (string s in folders)
            {
                desktop = Directory.GetDirectories(s);
                files = Directory.GetFiles(s, "*.*", SearchOption.TopDirectoryOnly);
                Console.WriteLine($"Path: {s} Folders: {desktop.Count()} Files: {files.Count()} size {Info.SizeSuffix(Info.DirSize(new DirectoryInfo(s)))}");
                folderName = new FileInfo(s).Name;
                string EndPath = Path.Combine(targetPath, folderName);
                Directory.CreateDirectory(EndPath);
                CopyFolder(s, EndPath);
            }

            return Task.CompletedTask;
        }

        static public void CopyFolder(string sourceFolder, string destFolder)
        {
            if (!Directory.Exists(destFolder))
                Directory.CreateDirectory(destFolder);
            try
            {
                string[] files = Directory.GetFiles(sourceFolder);
                foreach (string file in files)
                {
                    string name = Path.GetFileName(file);
                    string dest = Path.Combine(destFolder, name);
                    File.Copy(file, dest);
                }
            }catch { };
            try
            {
                string[] folders = Directory.GetDirectories(sourceFolder);
                foreach (string folder in folders)
                {
                    string name = Path.GetFileName(folder);
                    string dest = Path.Combine(destFolder, name);
                    CopyFolder(folder, dest);
                }
            }
            catch { }
        }
    }
}
