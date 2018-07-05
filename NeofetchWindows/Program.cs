using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Windows.Forms;

namespace NeofetchWindows
{
    class Program
    {
        //This program very much is still work in progress
        private static ManagementObjectSearcher baseboardSearcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_BaseBoard");
        private static ManagementObjectSearcher motherboardSearcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_MotherboardDevice");

        static void Main(string[] args)
        {
            Console.Title = "NeofetchWindows";
            string userName = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
            int numbers = userName.Count(char.IsLetter);
            string result = new String('-', numbers);

            ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_OperatingSystem");
            String osinfo = String.Empty;
            foreach (ManagementObject wmi in searcher.Get())
            {
                try
                {
                    string os = ((string)wmi["Caption"]).Trim() + ", " + (string)wmi["Version"] + ", " + (string)wmi["OSArchitecture"];
                    osinfo = os;
                }
                catch { }
            }
            var start = Stopwatch.StartNew();
            ManagementObject mo = new ManagementObject(@"\\.\root\cimv2:Win32_OperatingSystem=@");
            DateTime lastBootUp = ManagementDateTimeConverter.ToDateTime(mo["LastBootUpTime"].ToString());
            var DoneUptime = (DateTime.Now.ToUniversalTime() - lastBootUp.ToUniversalTime()).Subtract(start.Elapsed).ToString(@"dd\d\a\y\s\ hh\h\o\u\r\s\ mm\m\i\n\u\t\e\s\ ss\s\e\c\o\n\d\s");

            int width = Screen.PrimaryScreen.Bounds.Width;
            int heigth = Screen.PrimaryScreen.Bounds.Height;
            int MonitorCount = Screen.AllScreens.Count();
            string fullresolution = $"{SystemInformation.VirtualScreen.Width}x{SystemInformation.VirtualScreen.Height}";


            //Dear god I tried getting this to work. For it to get the total amount of storage accross drives and count it all up (I deleted most of the code
            //so you're only seeing a small part of it) but no matter what I tried I cannot get it to show the storage across all drives without using
            //Console.write and even if I do that and save the output of the console to a file it'll still only show one drive.
#if false
            String TotalHDDSize = String.Empty;
            DriveInfo[] allDrives = DriveInfo.GetDrives();
            foreach (DriveInfo drive in allDrives)
            {
                if (drive.IsReady)
                {
                    var did = (
                    "  Total available space:          {0, 15} bytes", drive.TotalFreeSpace);
                    //Console.WriteLine("  Total available space:          {0, 15} bytes", drive.TotalFreeSpace);
                    var totalsize = drive.TotalSize;
                    string[] Suffix = { "B", "KB", "MB", "GB", "TB" };
                    int i;
                    double dblSByte = totalsize;
                    for (i = 0; i < Suffix.Length && totalsize >= 1024; i++, totalsize /= 1024)
                    {
                        dblSByte = totalsize / 1024.0;
                    }

                    var fullsize = String.Format("{0:0.##} {1}", dblSByte, Suffix[i]);
                    TotalHDDSize = fullsize;
                }
            }
#endif

            RegistryKey processor_name = Registry.LocalMachine.OpenSubKey(@"Hardware\Description\System\CentralProcessor\0", RegistryKeyPermissionCheck.ReadSubTree);   //This registry entry contains entry for processor info.
            var CPU = processor_name.GetValue("ProcessorNameString");

            int MemSlots = 0;
            ManagementScope oMs = new ManagementScope();
            ObjectQuery oQuery2 = new ObjectQuery("SELECT MemoryDevices FROM Win32_PhysicalMemoryArray");
            ManagementObjectSearcher oSearcher2 = new ManagementObjectSearcher(oMs, oQuery2);
            ManagementObjectCollection oCollection2 = oSearcher2.Get();
            foreach (ManagementObject obj in oCollection2)
            {
                MemSlots = Convert.ToInt32(obj["MemoryDevices"]);

            }
            string mem = MemSlots.ToString(); //Gets the amount of memory slots that are located on your motherboard

            ManagementScope oMs2 = new ManagementScope();
            ObjectQuery oQuery = new ObjectQuery("SELECT Capacity FROM Win32_PhysicalMemory");
            ManagementObjectSearcher oSearcher = new ManagementObjectSearcher(oMs2, oQuery);
            ManagementObjectCollection oCollection = oSearcher.Get();

            long MemSize = 0;
            long mCap = 0;

            //In case more than one Memory sticks are installed
            foreach (ManagementObject obj in oCollection)
            {
                mCap = Convert.ToInt64(obj["Capacity"]);
                MemSize += mCap;
            }
            MemSize = (MemSize / 1024) / 1024;
            string ram = MemSize.ToString() + "MB";

            //This code gets the GPU I have gotten it to get both installed GPU's on another program I made but that means I'd have to run it mutiple times (
            //Ones per GPU) which is pretty inefficient
            ManagementObjectSearcher objvide = new ManagementObjectSearcher("select * from Win32_VideoController");
            foreach (ManagementObject obj in objvide.Get())
            {
                {
                    List<string> GPU = new List<string>();
                    int GPUCounting = objvide.Get().Count;
                    GPU.Add("" + obj["VideoProcessor"]);

                    foreach (string s in GPU)
                    {
                        //Okay so before you ask yes putting something into mutiple strings and then into a string list and finally posting the list into a CMD is
                        //Not a good example of coding it's stupid as fuck and will be changed in the future. I'm mostly just fucking around for the time being
                        string lineA = $@"    `  `  `  `  `` ````..--::/++++++:      {userName}";
                        string lineB = $@"          ```... -:/++++++oooooooooo:      {result}";
                        string lineC = $@"   `.--://+++oo: /oooooooooooooooooo:`     OS: {osinfo}";
                        string lineD = $@"   :ooooooooooo-`/oooooooooooooooooo:      Uptime: {DoneUptime}";
                        string lineE = $@"   /ooooooooooo: /oooooooooooooooooo:`     CPU: {CPU}";
                        string lineF = $@"   /ooooooooooo: /oooooooooooooooooo:      Memory: {ram}";
                        string lineG = $@"   :ooooooooooo-`/oooooooooooooooooo:      Ramslots: {mem}";
                        string lineH = $@"   /ooooooooooo: /oooooooooooooooooo:`     GPU: {s}";
                        string lineI = $@"   .-::-::-::--. .------------------.      Amount of GPUs: {GPUCounting}";
                        string lineJ = $@"   :+++++++++++- -:--:--:--:--:-----.      Primary displays resolution: {width}x{heigth}";
                        string lineK = $@"   /ooooooooooo: -::::::::::::::::::-`     Amount of monitors: {MonitorCount}";
                        string lineL = $@"   /ooooooooooo:`-::::::::::::::::::-      Complete resolution: {fullresolution}";
                        string lineM = $@"   /ooooooooooo: -::::::::::::::::::-      ";
                        string lineN = $@"   /ooooooooooo: -::::::::::::::::::-`     ";
                        string lineO = $@"   -++ooooooooo:`-::::::::::::::::::-      ";
                        string lineP = $@"   ` ``..--://+- -::::::::::::::::::-`     ";
                        string lineQ = $@"           ` ``...---:::::::::::-          ";
                        string lineR = $@"     `  `  `  `  `` ``  ` ````...---:.     ";


                        List<string> Line = new List<string>();
                        Line.Add($"{lineA}");
                        Line.Add($"{lineB}");
                        Line.Add($"{lineC}");
                        Line.Add($"{lineD}");
                        Line.Add($"{lineE}");
                        Line.Add($"{lineF}");
                        Line.Add($"{lineG}");
                        Line.Add($"{lineH}");
                        Line.Add($"{lineI}");
                        Line.Add($"{lineJ}");
                        Line.Add($"{lineK}");
                        Line.Add($"{lineL}");
                        Line.Add($"{lineM}");
                        Line.Add($"{lineN}");
                        Line.Add($"{lineO}");
                        Line.Add($"{lineP}");
                        Line.Add($"{lineQ}");
                        Line.Add($"{lineR}");
                       
                        Line.ToList().ForEach(System.Console.WriteLine);
                        System.Console.ReadKey();
                    };
                }
            }
        }
    }
}
