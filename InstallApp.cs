using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace BackUpDesktop
{
    internal class InstallApp
    {
        static string path = Program.BackUpPath + "\\SoftwareName.txt";
        public static void FindInstalls(RegistryKey regKey, List<string> keys)
        {


            foreach (string key in keys)
            {
                using (RegistryKey rk = regKey.OpenSubKey(key))
                {
                    if (rk == null)
                    {
                        continue;
                    }
                    foreach (string skName in rk.GetSubKeyNames())
                    {
                        using (RegistryKey sk = rk.OpenSubKey(skName))
                        {
                            try
                            {
                                if (!File.Exists(path))
                                {
                                    using (StreamWriter sw = File.CreateText(path))
                                    {
                                        sw.WriteLine(Convert.ToString(sk.GetValue("DisplayName")));
                                    }
                                }

                                using (StreamWriter sw = File.AppendText(path))
                                {
                                    sw.WriteLine(Convert.ToString(sk.GetValue("DisplayName")));
                                }
                            }
                            catch (Exception ex)
                            { }
                        }
                    }
                }
            }
        }
    }
}
