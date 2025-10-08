using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace DSO_Utilities.Hotkeys
{

    [Flags]
    public enum ModifierKeys : uint
    {
        None = 0,
        Alt = 1,
        Control = 2,
        Shift = 4,
        Win = 8
    }

    public class GlobalHotkey
    {
        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);
        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        private readonly int id;
        private readonly IntPtr handle;

        public Keys key { get; }
        public ModifierKeys Modifiers { get; }
        public event Action Pressed;

        public GlobalHotkey(IntPtr handle, int id, Keys key, ModifierKeys modifiers = ModifierKeys.None)
        {
            this.handle = handle;
            this.id = id;
            this.key = key;
            this.Modifiers = modifiers;
            
            

            RegisterHotKey(handle, id, (uint)modifiers, (uint)key);
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
