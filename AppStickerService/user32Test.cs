using System;
using System.Runtime.InteropServices;
using System.Text;

public  class user32Test
{
    [DllImport("User32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    public static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, ref IntPtr lParam);

    [DllImport("User32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    public static extern bool EnumDesktopWindows(IntPtr hDesktop, EnumWindowsProc lpEnumFunc, ref IntPtr lParam);

    public delegate bool EnumWindowsProc(IntPtr hWnd, ref IntPtr lParam);

    public const long SYNCHRONIZE = (0x00100000L);
    public const int STANDARD_RIGHTS_REQUIRED = (0x000F0000);

    public const int PROCESS_VM_READ = (0x0010);
    public const int PROCESS_VM_WRITE = (0x0020);
    public const int PROCESS_DUP_HANDLE = (0x0040);
    public const int PROCESS_CREATE_PROCESS = (0x0080);
    public const int PROCESS_SET_QUOTA = (0x0100);
    public const int PROCESS_SET_INFORMATION = (0x0200);
    public const int PROCESS_QUERY_INFORMATION = (0x0400);
    public const int PROCESS_SUSPEND_RESUME = (0x0800);
    public const int PROCESS_QUERY_LIMITED_INFORMATION = (0x1000);
    public const int PROCESS_SET_LIMITED_INFORMATION = (0x2000);
    public const long PROCESS_ALL_ACCESS = (STANDARD_RIGHTS_REQUIRED | SYNCHRONIZE | 0xFFFF);

    [DllImport("Kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    public static extern IntPtr OpenProcess(uint dwDesiredAccess, bool bInheritHandle, int dwProcessId);

    [DllImport("Kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    public static extern bool QueryFullProcessImageName(IntPtr hProcess, int dwFlags, StringBuilder lpExeName, ref uint lpdwSize);

    [DllImport("Kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    public static extern bool CloseHandle(IntPtr hObject);

    [DllImport("User32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out int lpdwProcessId);

    [DllImport("User32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    public static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

    [DllImport("Kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    public static extern bool GetPackageFullName(IntPtr hProcess, ref uint packageFullNameLength, StringBuilder packageFullName);

    [DllImport("User32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    public static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);

    [DllImport("User32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    public static extern bool IsWindowVisible(IntPtr hWnd);

    public static bool ListWindows(IntPtr hWnd, ref IntPtr lParam)
    {
        ENUMPARAM ep = (ENUMPARAM)Marshal.PtrToStructure(lParam, typeof(ENUMPARAM));

        StringBuilder sbClassName = new StringBuilder(260);
        string sClassName = null;

        GetClassName(hWnd, sbClassName, (int)(sbClassName.Capacity));
        sClassName = sbClassName.ToString();

        // Minimized
        if (sClassName == "Windows.UI.Core.CoreWindow")
        {
            int nPID = 0;
            uint nThreadId = GetWindowThreadProcessId(hWnd, out nPID);
            IntPtr hProcess = OpenProcess(PROCESS_QUERY_LIMITED_INFORMATION, false, nPID);
            string sPackage = string.Empty;
            if (hProcess != IntPtr.Zero)
            {
                uint nSize = 260;
                StringBuilder sPackageFullName = new StringBuilder((int)nSize);
                GetPackageFullName(hProcess, ref nSize, sPackageFullName);
                if (sPackageFullName.ToString().ToLower().Contains("calculator") && IsWindowVisible(hWnd))
                {
                    nSize = 260;
                    StringBuilder sProcessImageName = new StringBuilder((int)nSize);
                    QueryFullProcessImageName(hProcess, 0, sProcessImageName, ref nSize);

                    ep.hWnd = hWnd;
                    ep.sExeName = sProcessImageName.ToString();
                    ep.nPID = nPID;
                    ep.nState = 1;
                    Marshal.StructureToPtr(ep, lParam, false);
                    CloseHandle(hProcess);
                    return false;
                }
                CloseHandle(hProcess);
            }
        }

        // Normal
        if (sClassName == "ApplicationFrameWindow")
        {
            IntPtr hWndFind = FindWindowEx(hWnd, IntPtr.Zero, "Windows.UI.Core.CoreWindow", null);
            if (hWndFind != IntPtr.Zero)
            {
                int nPID = 0;
                uint nThreadId = GetWindowThreadProcessId(hWndFind, out nPID);
                IntPtr hProcess = OpenProcess(PROCESS_QUERY_LIMITED_INFORMATION, false, nPID);
                string sPackage = string.Empty;
                if (hProcess != IntPtr.Zero)
                {
                    uint nSize = 260;
                    StringBuilder sPackageFullName = new StringBuilder((int)nSize);
                    GetPackageFullName(hProcess, ref nSize, sPackageFullName);
                    if (sPackageFullName.ToString().ToLower().Contains("calculator") && IsWindowVisible(hWnd))
                    {
                        nSize = 260;
                        StringBuilder sProcessImageName = new StringBuilder((int)nSize);
                        QueryFullProcessImageName(hProcess, 0, sProcessImageName, ref nSize);

                        ep.hWnd = hWnd;
                        ep.sExeName = sProcessImageName.ToString();
                        ep.nPID = nPID;
                        Marshal.StructureToPtr(ep, lParam, false);
                        CloseHandle(hProcess);
                        return false;
                    }
                    CloseHandle(hProcess);
                }
            }
        }
        return true;
    }
    

    private void button1_Click(object sender, EventArgs e)
    {
        EnumWindowsProc Callback = new EnumWindowsProc(ListWindows);
        ENUMPARAM ep = new ENUMPARAM();
        IntPtr plParam = Marshal.AllocHGlobal(Marshal.SizeOf(ep));
        Marshal.StructureToPtr(ep, plParam, false);
        EnumWindows(Callback, ref plParam);
        ENUMPARAM epret = (ENUMPARAM)Marshal.PtrToStructure(plParam, typeof(ENUMPARAM));
        Marshal.FreeHGlobal(plParam);
        if (epret.hWnd != IntPtr.Zero)
        {
            //string sState = (epret.nState == 1) ? "\n(state = minimized)" : "";
            //MessageBox.Show(string.Format("Window handle = {0}\nPID = {1}\nExecutable = {2}" + sState, epret.hWnd.ToString("X"), epret.nPID, epret.sExeName), "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }

    [StructLayoutAttribute(LayoutKind.Sequential)]
    private struct ENUMPARAM
    {
        public IntPtr hWnd;
        public int nPID;
        public string sExeName;
        public int nState;
    }
}