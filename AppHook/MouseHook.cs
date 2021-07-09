using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

class MouseHook
{
    private const int WM_MOUSEMOVE = 0x200;
    private const int WM_LBUTTONDOWN = 0x201;
    private const int WM_RBUTTONDOWN = 0x204;
    private const int WM_MBUTTONDOWN = 0x207;
    private const int WM_LBUTTONUP = 0x202;
    private const int WM_RBUTTONUP = 0x205;
    private const int WM_MBUTTONUP = 0x208;
    private const int WM_LBUTTONDBLCLK = 0x203;
    private const int WM_RBUTTONDBLCLK = 0x206;
    private const int WM_MBUTTONDBLCLK = 0x209;

    public event MouseEventHandler OnMouseActivity;

    static int hMouseHook = 0;

    public const int WH_MOUSE_LL = 14;//low level mouse event
    public const int WH_MOUSE = 7;//normal level mouse event

    HookProc MouseHookProcedure;
    //Log _log = new Log("MouseHook", true, Log4netWrapper.Default);
    [StructLayout(LayoutKind.Sequential)]
    public class POINT
    {
        public int x;
        public int y;
    }

    [StructLayout(LayoutKind.Sequential)]
    public class MouseHookStruct
    {
        public POINT pt;
        public int hWnd;
        public int wHitTestCode;
        public int dwExtraInfo;
    }

    [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
    public static extern int SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hInstance, int threadId);

    [DllImport("kernel32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
    public static extern int GetLastError();

    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr GetModuleHandle(string lpModuleName);

    [DllImport("kernel32.dll")]
    private static extern int GetCurrentThreadId();//获取在系统中的线程ID

    [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
    public static extern bool UnhookWindowsHookEx(int idHook);

    [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
    public static extern int CallNextHookEx(int idHook, int nCode, Int32 wParam, IntPtr lParam);


    public delegate int HookProc(int nCode, Int32 wParam, IntPtr lParam);


    public MouseHook()
    {
    }

    ~MouseHook()
    {
        Stop();
    }

    public void Start()
    {
        if (hMouseHook == 0)
        {
            MouseHookProcedure = new HookProc(MouseHookProc);

            hMouseHook = SetWindowsHookEx(WH_MOUSE_LL, MouseHookProcedure, GetModuleHandle("user32"), 0);//第一个参数是WH_MOUSE_LL,表示捕获所有线程的鼠标消息，同时最后一个参数必须是0
                                                                                                         //hMouseHook = SetWindowsHookEx(WH_MOUSE, MouseHookProcedure, GetModuleHandle("user32"), GetCurrentThreadId());//只捕获当前应用程序（当前线程）的鼠标消息，最后一个参数是当前线程id，使用GetCurrentThreadId()获得，一定不要使用托管线程id（Thread.CurrentThread.ManagedThreadId)。
            if (hMouseHook == 0)
            {
                int errorCode = GetLastError();
                //_log.E("SetWindowsHookEx failed.error code:" + errorCode);
                Stop();
            }
        }
    }

    public void Stop()
    {
        bool retMouse = true;
        if (hMouseHook != 0)
        {
            retMouse = UnhookWindowsHookEx(hMouseHook);
            hMouseHook = 0;
        }

        if (!(retMouse))
        {
            //_log.E("UnhookWindowsHookEx failed.");
        }
    }

    private int MouseHookProc(int nCode, Int32 wParam, IntPtr lParam)
    {
        //只处理鼠标左键按下的情况
        if ((wParam == WM_LBUTTONDOWN) && (nCode >= 0) && (OnMouseActivity != null))
        {
            MouseButtons button = MouseButtons.None;
            int clickCount = 0;

            switch (wParam)
            {
                case WM_LBUTTONDOWN:
                    button = MouseButtons.Left;
                    clickCount = 1;
                    break;
                case WM_LBUTTONUP:
                    button = MouseButtons.Left;
                    clickCount = 1;
                    break;
                case WM_LBUTTONDBLCLK:
                    button = MouseButtons.Left;
                    clickCount = 2;
                    break;
                case WM_RBUTTONDOWN:
                    button = MouseButtons.Right;
                    clickCount = 1;
                    break;
                case WM_RBUTTONUP:
                    button = MouseButtons.Right;
                    clickCount = 1;
                    break;
                case WM_RBUTTONDBLCLK:
                    button = MouseButtons.Right;
                    clickCount = 2;
                    break;
            }

            MouseHookStruct MyMouseHookStruct = (MouseHookStruct)Marshal.PtrToStructure(lParam, typeof(MouseHookStruct));
            MouseEventArgs e = new MouseEventArgs(button, clickCount, MyMouseHookStruct.pt.x, MyMouseHookStruct.pt.y, 0);
            OnMouseActivity(this, e);
        }
        return CallNextHookEx(hMouseHook, nCode, wParam, lParam);
    }
}