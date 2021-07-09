using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppStickerService
{
    public class RunProcess
    {
        public string HWnd { get; set; }
        public string Title { get; set; }
        public string Name { get; set; }
        public bool IsTop { get; set; }
    }

    public class OpenWindow
    {
        public string Title { get; set; }
        public IntPtr HWnd { get; set; }
        public uint ProcessId { get; set; }
        public bool IsTop { get; set; }
    }
}
