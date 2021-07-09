using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Windows.UI.Core;
using Windows.UI.Xaml.Input;

namespace AppSticker
{
    class KeyBoardHook
    {
        //public const int WM_KEYDOWN = 0x100;
        //public const int WM_KEYUP = 0x101;
        //public const int WM_SYSKEYDOWN = 0x104;
        //public const int WM_SYSKEYUP = 0x105;

        //public event KeyEventHandler OnKeyActivity;

        //static IntPtr hKeyHook = IntPtr.Zero;

        //public const int WH_KEYBOARD_LL = 13;//low level keyboard event
        //public const int WH_KEYBOARD = 2;//normal level keyboard event

        //HookProc KeyHookProcedure;
        ////Log _log = new Log("MouseHook", true, Log4netWrapper.Default);
        //[StructLayout(LayoutKind.Sequential)] //声明键盘钩子的封送结构类型 
        //public class KeyboardHookStruct
        //{
        //    public int vkCode; //表示一个在1到254间的虚似键盘码 
        //    public int scanCode; //表示硬件扫描码 
        //    public int flags;
        //    public int time;
        //    public int dwExtraInfo;
        //}

        //[DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        //public static extern IntPtr SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hInstance, int threadId);

        //[DllImport("kernel32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        //public static extern int GetLastError();

        //[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        //private static extern IntPtr GetModuleHandle(string lpModuleName);

        //[DllImport("kernel32.dll")]
        //private static extern int GetCurrentThreadId();//获取在系统中的线程ID

        //[DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        //public static extern bool UnhookWindowsHookEx(IntPtr idHook);

        //[DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        //public static extern IntPtr CallNextHookEx(IntPtr idHook, int nCode, int wParam, IntPtr lParam);


        //public delegate IntPtr HookProc(int nCode, int wParam, IntPtr lParam);


        //public KeyBoardHook()
        //{
        //}

        //~KeyBoardHook()
        //{
        //    Stop();
        //}

        //public void Start()
        //{
        //    if (hKeyHook == IntPtr.Zero)
        //    {
        //        KeyHookProcedure = new HookProc(KeyHookProc);

        //        hKeyHook = SetWindowsHookEx(WH_KEYBOARD_LL,
        //                                    KeyHookProcedure,
        //                                    //GetModuleHandle("user32"),
        //                                    //GetModuleHandle("AppHook"),
        //                                    IntPtr.Zero,
        //                                    0);//第一个参数是WH_KEYBOARD_LL,表示捕获所有线程的键盘消息，同时最后一个参数必须是0

        //        if (hKeyHook == IntPtr.Zero)
        //        {
        //            int errorCode = GetLastError();
        //            //_log.E("SetWindowsHookEx failed.error code:" + errorCode);
        //            Stop();
        //        }
        //    }
        //}

        //public void Stop()
        //{
        //    bool retMouse = true;
        //    if (hKeyHook != IntPtr.Zero)
        //    {
        //        retMouse = UnhookWindowsHookEx(hKeyHook);
        //        hKeyHook = IntPtr.Zero;
        //    }

        //    if (!(retMouse))
        //    {
        //        //_log.E("UnhookWindowsHookEx failed.");
        //    }
        //}

        //private IntPtr KeyHookProc(int nCode, int wParam, IntPtr lParam)
        //{
        //    //只处理键盘按下的情况
        //    if ((nCode >= 0) && (OnKeyActivity != null))
        //    {
        //        KeyboardHookStruct KeyDataFromHook = (KeyboardHookStruct)Marshal.PtrToStructure(lParam, typeof(KeyboardHookStruct));
        //        Keys keyData = (Keys)KeyDataFromHook.vkCode;

        //        //按下控制键
        //        if (wParam == WM_KEYDOWN || wParam == WM_SYSKEYDOWN)
        //        {
        //            if (IsCtrlAltShiftKeys(keyData) && preKeysList.IndexOf(keyData) == -1)
        //            {
        //                preKeysList.Add(keyData);
        //            }

        //            KeyEventArgs e = new KeyEventArgs(GetDownKeys(keyData));
        //            OnKeyActivity(this, e);
        //        }

        //        //松开控制键
        //        if (wParam == WM_KEYUP || wParam == WM_SYSKEYUP)
        //        {
        //            if (IsCtrlAltShiftKeys(keyData))
        //            {
        //                for (int i = preKeysList.Count - 1; i >= 0; i--)
        //                {
        //                    if (preKeysList[i] == keyData) { preKeysList.RemoveAt(i); }
        //                }
        //            }
        //        }
        //    }
        //    return CallNextHookEx(hKeyHook, nCode, wParam, lParam);
        //}

        //private List<Keys> preKeysList = new List<Keys>();//存放被按下的控制键，用来生成具体的键

        ////根据已经按下的控制键生成key
        //private Keys GetDownKeys(Keys key)
        //{
        //    Keys rtnKey = Keys.None;

        //    foreach (Keys i in preKeysList)
        //    {
        //        if (i == Keys.LControlKey || i == Keys.RControlKey) { rtnKey = rtnKey | Keys.Control; }
        //        if (i == Keys.LMenu || i == Keys.RMenu) { rtnKey = rtnKey | Keys.Alt; }
        //        if (i == Keys.LShiftKey || i == Keys.RShiftKey) { rtnKey = rtnKey | Keys.Shift; }
        //    }

        //    return rtnKey | key;
        //}

        //private Boolean IsCtrlAltShiftKeys(Keys key)
        //{
        //    if (key == Keys.LControlKey || key == Keys.RControlKey || key == Keys.LMenu || key == Keys.RMenu || key == Keys.LShiftKey || key == Keys.RShiftKey) { return true; }

        //    return false;
        //}
    }
}