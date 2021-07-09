using log4net;
using log4net.Config;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Management;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Windows.ApplicationModel.AppService;
using Windows.Foundation.Collections;
using static AppStickerService.Win32Helper;

namespace AppStickerService
{
    public static class Startup
    {
        static ILog logger = null;

        private static void InitLog4Net()
        {
            var logCfg = new FileInfo(AppDomain.CurrentDomain.BaseDirectory + "log4net.config");
            XmlConfigurator.ConfigureAndWatch(logCfg);
            logger = LogManager.GetLogger(typeof(Startup));
        }

        private static ConcurrentDictionary<string, IntPtr> HWnds = new ConcurrentDictionary<string, IntPtr>();

        private static AppServiceConnection connection = new AppServiceConnection();

        static KeyBoardHook keyBoardHook;

        private static void SetKeyBoard(object state)
        {
            keyBoardHook = new KeyBoardHook();
            keyBoardHook.OnKeyActivity += Hook_OnKeyActivity;
            keyBoardHook.Start();

            tagMSG Msgs;
            while (GetMessage(out Msgs, IntPtr.Zero, 0, 0) > 0)
            {
                TranslateMessage(ref Msgs);
                DispatchMessage(ref Msgs);
            }
        }

        private static void Hook_OnKeyActivity(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyData == (Keys.B | Keys.Control))
            {
                IntPtr activeHWnd = Win32Helper.GetForegroundWindow();
                Win32Helper.ActivateApp(activeHWnd, true);
            }
        }

        public static void Enter(string[] args)
        {
            ThreadPool.QueueUserWorkItem(SetKeyBoard);

            // MessageBox.Show("Hello");
            //while (true)
            //{

            //}
            //return;

            InitLog4Net();

            connection.AppServiceName = "CommunicationService";
            connection.PackageFamilyName = Windows.ApplicationModel.Package.Current.Id.FamilyName;
            // hook up the connection event handlers
            connection.ServiceClosed += Connection_ServiceClosed; // 超过25秒闲置 服务会Close掉
            connection.RequestReceived += Connection_RequestReceived;

            AppServiceConnectionStatus result = AppServiceConnectionStatus.Unknown;

            Task.Run(async () =>
            {
                // open a connection to the UWP AppService
                result = await connection.OpenAsync();

            }).GetAwaiter().GetResult();

            if (result == AppServiceConnectionStatus.Success)
            {
                while (true)
                {
                    // the below is necessary if this were calling COM and this was STAThread
                    // pump the underlying STA thread
                    // https://blogs.msdn.microsoft.com/cbrumme/2004/02/02/apartments-and-pumping-in-the-clr/
                    // Thread.CurrentThread.Join(0);
                }
            }

            logger.Info($"AppStickerService {result}");
        }

        private static void Connection_ServiceClosed(AppServiceConnection sender, AppServiceClosedEventArgs args)
        {
            logger.Info("AppStickerService Connection_ServiceClosed");
            keyBoardHook.Stop();

            System.Environment.Exit(0);
        }

        private async static void Connection_RequestReceived(AppServiceConnection sender, AppServiceRequestReceivedEventArgs args)
        {
            var deferral = args.GetDeferral();

            ValueSet message = args.Request.Message;
            ValueSet returnData = new ValueSet();

            // get the verb or "command" for this request
            string verb = message["verb"] as String;

            switch (verb)
            {
                // we received a request to get the Startup program names
                case "getRunProcesses":
                    {
                        try
                        {
                            returnData.Add("verb", "success");
                            // open HKLM with a 64bit view. If you use Registry32, your view will be virtualized to the current user
                            var result = GetRunProcesses();

                            // add the names to our response
                            returnData.Add("result", JsonConvert.SerializeObject(result));
                        }
                        catch (Exception ex)
                        {
                            returnData.Add("verb", "error");
                            returnData.Add("exceptionMessage", ex.Message.ToString());
                        }

                        break;
                    }

                case "setForegroundWindow":
                    {
                        try
                        {
                            returnData.Add("verb", "success");
                            var id = message["hWnd"] as string;
                            var on = message["on"] as string;

                            var hWnd = HWnds[id];
                            var isOn = bool.Parse(on);

                            // var result = GetRunProcesses();

                            Win32Helper.ActivateApp(hWnd, isOn);

                            // add the names to our response
                            // returnData.Add("result", JsonConvert.SerializeObject(result));
                        }
                        catch (Exception ex)
                        {
                            returnData.Add("verb", "error");
                            returnData.Add("exceptionMessage", ex.Message.ToString());
                        }

                        break;
                    }
            }

            try
            {
                // Return the data to the caller.
                await args.Request.SendResponseAsync(returnData);
            }
            catch (Exception e)
            {
                // Your exception handling code here.
                logger.Info($"Connection_RequestReceived {e.Message}");
            }
            finally
            {
                // Complete the deferral so that the platform knows that we're done responding to the app service call.
                // Note for error handling: this must be called even if SendResponseAsync() throws an exception.
                deferral.Complete();
            }
        }

        public static List<RunProcess> GetRunProcesses()
        {
            List<RunProcess> result = new List<RunProcess>();
            HWnds = new ConcurrentDictionary<string, IntPtr>();

            //var localByName = EnumerateOpenedWindows.GetDesktopWindowsTitles();
            var openWindows = EnumerateOpenedWindows.GetDesktopWindowAlls();

            foreach (var openWindow in openWindows)
            {
                var process = Process.GetProcessById((int)openWindow.ProcessId);
                if (process.Responding)
                {
                    var id = Guid.NewGuid().ToString();

                    result.Add(new RunProcess
                    {
                        Title = openWindow.Title,
                        HWnd = id,
                        Name = Process.GetProcessById((int)openWindow.ProcessId).ProcessName,
                        IsTop = openWindow.IsTop
                    });

                    HWnds.TryAdd(id, openWindow.HWnd);
                }
            }

            return result;
        }

        public static List<Process> GetChild(Process process)
        {
            List<Process> children = new List<Process>();
            ManagementObjectSearcher mos = new ManagementObjectSearcher(String.Format("Select * From Win32_Process Where ParentProcessID={0}", process.Id));

            foreach (ManagementObject mo in mos.Get())
            {
                children.Add(Process.GetProcessById(Convert.ToInt32(mo["ProcessID"])));
            }

            return children;
        }
    }

    public class Win32Helper
    {
        [DllImport("user32.dll")]
        static extern bool SetForegroundWindow(IntPtr hWnd);
        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();
        [DllImport("user32.dll", EntryPoint = "SetActiveWindow")]
        public static extern IntPtr SetActiveWindow(IntPtr hWnd);
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        private static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
        private static readonly IntPtr HWND_NOTOPMOST = new IntPtr(-2);

        private const UInt32 SWP_NOSIZE = 0x0001;
        private const UInt32 SWP_NOMOVE = 0x0002;
        private const UInt32 TOPMOST_FLAGS = SWP_NOMOVE | SWP_NOSIZE;

        public static void ActivateApp(IntPtr hWnd, bool isOn)
        {
            if (isOn)
            {
                SetWindowPos(hWnd, HWND_TOPMOST, 0, 0, 0, 0, TOPMOST_FLAGS);
            }
            else
            {
                SetWindowPos(hWnd, HWND_NOTOPMOST, 0, 0, 0, 0, TOPMOST_FLAGS);
            }

            //SetForegroundWindow(hWnd);
        }

        public struct tagMSG
        {
            public int hwnd;
            public uint message;
            public int wParam;
            public long lParam;
            public uint time;
            public int pt;
        }

        [DllImport("user32", EntryPoint = "GetMessage")]
        public static extern int GetMessage(out tagMSG lpMsg, IntPtr hwnd, int wMsgFilterMin, int wMsgFilterMax);
        [DllImport("user32", EntryPoint = "DispatchMessage")]
        public static extern int DispatchMessage(ref tagMSG lpMsg);
        [DllImport("user32", EntryPoint = "TranslateMessage")]
        public static extern int TranslateMessage(ref tagMSG lpMsg);
    }
}
