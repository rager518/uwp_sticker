using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AppHook
{
    public partial class Form1 
    {
        public Form1()
        {

            //InitializeComponent();
            ThreadPool.QueueUserWorkItem(SetKeyBoard);

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            

            //kh = new Win32KeyboardHook();
            //kh.SetHook();
            //kh.OnKeyDownEvent += kh_OnKeyDownEvent;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            keyBoardHook.Stop();

            //kh.UnHook();
        }

        //Win32KeyboardHook kh;



        //void kh_OnKeyDownEvent(object sender, KeyEventArgs e)
        //{

        //    if (e.KeyData == (Keys.S | Keys.Control)) { this.Show(); }//Ctrl+S显示窗口

        //    if (e.KeyData == (Keys.H | Keys.Control)) { this.Hide(); }//Ctrl+H隐藏窗口

        //    if (e.KeyData == (Keys.C | Keys.Control)) { this.Close(); }//Ctrl+C 关闭窗口 

        //    if (e.KeyData == (Keys.A | Keys.Control | Keys.Alt)) { this.Text = "你发现了什么？"; }//Ctrl+Alt+A
        //}




        #region Hook
        [DllImport("user32", EntryPoint = "GetMessage")]
        public static extern int GetMessage(out tagMSG lpMsg, IntPtr hwnd, int wMsgFilterMin, int wMsgFilterMax
            );
        [DllImport("user32", EntryPoint = "DispatchMessage")]
        public static extern int DispatchMessage(ref tagMSG lpMsg);
        [DllImport("user32", EntryPoint = "TranslateMessage")]
        public static extern int TranslateMessage(ref tagMSG lpMsg);
        [StructLayout(LayoutKind.Sequential)]
        public class POINT
        {
            public int x;
            public int y;
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
        MouseHook hook;
        KeyBoardHook keyBoardHook;

        #endregion

        private void SetHK(object state)
        {
            hook = new MouseHook();
            hook.OnMouseActivity += Hook_OnMouseActivity;
            // if (StringContent.Build)
            {
                hook.Start(/*Thread.CurrentThread.ManagedThreadId*/);
                tagMSG Msgs;
                while (GetMessage(out Msgs, IntPtr.Zero, 0, 0) > 0)
                {
                    TranslateMessage(ref Msgs);
                    DispatchMessage(ref Msgs);
                }
            }
        }

        private void SetKeyBoard(object state)
        {
            keyBoardHook = new KeyBoardHook();
            keyBoardHook.OnKeyActivity += Hook_OnKeyActivity;
            // if (StringContent.Build)
            {
                keyBoardHook.Start(/*Thread.CurrentThread.ManagedThreadId*/);
                tagMSG Msgs;
                while (GetMessage(out Msgs, IntPtr.Zero, 0, 0) > 0)
                {
                    TranslateMessage(ref Msgs);
                    DispatchMessage(ref Msgs);
                }
            }
        }

        private void Hook_OnMouseActivity(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            //e.X  e.Y   e.Button == System.Windows.Forms.MouseButtons.Left
            var a = 5;

        }

        private void Hook_OnKeyActivity(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            //e.X  e.Y   e.Button == System.Windows.Forms.MouseButtons.Left
            //var a = 5;
            if (e.KeyData == (Keys.S | Keys.Control))
            {
                MessageBox.Show("Control+s");

            }//Ctrl+S显示窗口

        }

    }
}
