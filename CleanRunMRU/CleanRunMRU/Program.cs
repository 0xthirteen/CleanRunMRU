using System;
using Microsoft.Win32;
using System.Collections.Generic;

namespace CleanRunMRU
{
    class Program
    {
        static void QueryReg()
        {
            try
            {
                string keypath = "Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\RunMRU";
                Console.WriteLine("Current HKCU:{0} values", keypath);
                RegistryKey regkey;
                regkey = Registry.CurrentUser.OpenSubKey(keypath, true);
                if (regkey.ValueCount > 0)
                {
                    foreach (string subKey in regkey.GetValueNames())
                    {
                        Console.WriteLine("[+]  Key Name : {0}", subKey);
                        Console.WriteLine("        Value : {0}", regkey.GetValue(subKey).ToString());
                        Console.WriteLine();
                    }
                }
                regkey.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("[-] Error: {0}", ex.Message);
            }
        }

        static void CleanSingle(string command)
        {
            string keypath = "Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\RunMRU";
            string keyvalue = string.Empty;
            string regcmd = string.Empty;
            if (command.EndsWith("\\1"))
            {
                regcmd = command;
            }
            else
            {
                regcmd = string.Format("{0}\\1", command);
            }
             
            try
            {
                RegistryKey regkey;
                regkey = Registry.CurrentUser.OpenSubKey(keypath, true);

                if (regkey.ValueCount > 0)
                {
                    foreach (string subKey in regkey.GetValueNames())
                    {
                        if(regkey.GetValue(subKey).ToString() == regcmd)
                        {
                            keyvalue = subKey;
                            regkey.DeleteValue(subKey);
                            Console.WriteLine(regcmd);
                            Console.WriteLine("[+] Cleaned {0} from HKCU:{1}", command, keypath);
                        }
                    }
                    if(keyvalue != string.Empty)
                    {
                        string mruchars = regkey.GetValue("MRUList").ToString();
                        int index = mruchars.IndexOf(keyvalue);
                        mruchars = mruchars.Remove(index, 1);
                        regkey.SetValue("MRUList", mruchars);
                    }
                }
                regkey.Close();
            }
            catch (ArgumentException)
            {
                Console.WriteLine("[-] Error: Selected Registry value does not exist");
            }
        }

        static void CleanAll()
        {
            try
            {
                string keypath = "Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\RunMRU";
                Console.WriteLine("HKCU:{0}", keypath);
                RegistryKey regkey;
                regkey = Registry.CurrentUser.OpenSubKey(keypath, true);
                if (regkey.ValueCount > 0)
                {
                    foreach (string subKey in regkey.GetValueNames())
                    {
                        regkey.DeleteValue(subKey);
                    }
                }
                Console.WriteLine("[+] Cleaned all RunMRU values");
                regkey.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("[-] Error: {0}", ex.Message);
            }
        }

        static void HowTo()
        {
            Console.WriteLine("CleanRunMRU");
            Console.WriteLine("  Query current RunMRU key");
            Console.WriteLine("    CleanRunMRU.exe query\n");
            Console.WriteLine("  Clear all the values");
            Console.WriteLine("    CleanRunMRU.exe clearall\n");
            Console.WriteLine("  Clear one specific value");
            Console.WriteLine("    CleanRunMRU.exe command=\"C:\\Windows\\System32\\cmd.exe\"\n");
        }

        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                HowTo();
                return;
            }
            var arguments = new Dictionary<string, string>();
            foreach (string argument in args)
            {
                int idx = argument.IndexOf('=');
                if (idx > 0)
                    arguments[argument.Substring(0, idx)] = argument.Substring(idx + 1);
            }

            if (arguments.ContainsKey("command"))
            {
                CleanSingle(arguments["command"]);
            }
            else if (args[0].ToLower() == "clearall")
            {
                CleanAll();
            }
            else if (args[0].ToLower() == "query")
            {
                QueryReg();
            }
            else
            {
                HowTo();
                return;
            }
        }
    }
}
