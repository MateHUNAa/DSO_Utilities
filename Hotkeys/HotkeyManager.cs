using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DSO_Utilities.Hotkeys
{
    public class HotkeyManager
    {
        public Keys LeftHotkey { get; private set; } = Keys.F6;
        public Keys RightHotkey { get; private set; } = Keys.F7;

        public bool WaitingForLeft { get; private set; } 
        public bool WaitingForRight { get; private set; }

        public void StartWaitingLeft() => WaitingForLeft = true;
        public void StartWaitingRight() => WaitingForRight = true;

        public bool HandleKeyDown(Keys key)
        {
            if (WaitingForLeft)
            {
                LeftHotkey = key;
                WaitingForLeft = false;
                return true;
            }

            if (WaitingForRight)
            {
                RightHotkey = key;
                WaitingForRight = false;
                return true;
            }

            return false;
        }
    }
}
