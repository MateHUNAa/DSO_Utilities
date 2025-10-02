using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DSO_Utilities.Hotkeys
{
    public class GlobalHotkey
    {
        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);
        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        private readonly int id;
        private readonly IntPtr handle;

        public Keys key { get;  }
        public event Action Pressed;

        public GlobalHotkey(IntPtr handle, int id, Keys key)
        {
            this.handle = handle;
            this.id = id;
            this.key = key;

            RegisterHotKey(handle, id, 0, (uint)key);
        }

        public void ProcessMessage(Message m)
        {
            const int WM_HOTKEY = 0x0312;
            if (m.Msg == WM_HOTKEY && (int)m.WParam == id)
            {
                Pressed?.Invoke();
            }
        }

        public void Dispose()
        {
            UnregisterHotKey(handle, id);
        }
    }
}
