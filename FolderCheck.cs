using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackUpDesktop
{
    internal class FolderCheck
    {
        static string name = DateTime.Now.Date.ToShortDateString().Replace('/', '_');

        public static void foldercheck(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory("" + name);
                Console.WriteLine("CreateDirectory >" + path);
            }

            if (Directory.Exists(path))
            {
                //DataCopy.FFCopy(path);
            }
            else
            {
                Console.WriteLine("Source path does not exist!");
            }
        }
    }
}
