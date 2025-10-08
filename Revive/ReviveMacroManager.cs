using DSO_Utilities.Clicker;
using DSO_Utilities.Config;
using DSO_Utilities.Hotkeys;
using DSO_Utilities.UI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DSO_Utilities.Revive
{
    public class ReviveMacroManager : IDisposable
    {
        private readonly ConfigData config;
        private readonly IntPtr handle;
        private readonly Action<string, Point> onPositionSaved;
        private readonly Dictionary<string, (GlobalHotkey normal, GlobalHotkey ctrl)> hotkeyMap = new Dictionary<string, (GlobalHotkey normal, GlobalHotkey ctrl)>();

        private readonly MouseClicker leftClicker = new MouseClicker(true);

        public ReviveMacroManager(IntPtr handle, ConfigData config, Action<string, Point> onPositionSaved)
        {
            this.handle = handle;
            this.config = config;
            this.onPositionSaved = onPositionSaved;
            

            RegisterHotkeys();
        }

        private void RegisterHotkeys()
        {
            int id = 50;

            foreach (var kvp in config.ReviveHotkeys)
            {
                string slot = kvp.Key;
                string keyName = kvp.Value;

                if (!Enum.TryParse(keyName, out Keys key))
                    continue;

                var hotkey = new GlobalHotkey(handle, id++, key);
                hotkey.Pressed += () =>  OnHotkeyPressed(slot, false);

                var hkCtrl = new GlobalHotkey(handle, id++, key, ModifierKeys.Control);
                hkCtrl.Pressed += () => OnHotkeyPressed(slot, true);

                hotkeyMap[slot] = (hotkey, hkCtrl);
            }
        }

        public void OnHotkeyPressed(string slot, bool isCtrl)
        {
            if (isCtrl)
            {
                Point pos = Cursor.Position;
                config.RevivePositions[slot] = pos;
                ConfigManager.Save(config);
                onPositionSaved?.Invoke(slot, pos);
            }
            else
            {
                if (config.RevivePositions.TryGetValue(slot, out Point pos) && pos != Point.Empty)
                {
                    var prev = Cursor.Position;
                    Cursor.Position = pos;
                    Task.Delay(500);
                    leftClicker.Stop();
                    mouse_event(0x02, 0, 0, 0, 0); // Left down
                    mouse_event(0x04, 0, 0, 0, 0); // Left up
                    Task.Delay(500);
                    Cursor.Position = prev;
                } 
            }
        }

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);

        public void ProcessMessage(Message m )
        {
            foreach (var (normal, ctrl) in hotkeyMap.Values)
            {
                normal.ProcessMessage(m);
                ctrl.ProcessMessage(m);
            }
        }
        
        public void UpdateHotkey(string slot, Keys newKey)
        {
            if (!hotkeyMap.ContainsKey(slot))
                return;

            var oldPair = hotkeyMap[slot];
            oldPair.normal.Dispose();
            oldPair.ctrl.Dispose();

            config.ReviveHotkeys[slot] = newKey.ToString();
            ConfigManager.Save(config);

            int newId = slot.GetHashCode() & 0xFFFF;
            var normal = new GlobalHotkey(handle, newId, newKey);
            var ctrl = new GlobalHotkey(handle, newId +1, newKey, ModifierKeys.Control);

            normal.Pressed += () => OnHotkeyPressed(slot, false);
            ctrl.Pressed += () => OnHotkeyPressed(slot, true);

            hotkeyMap[slot] = (normal, ctrl);
        }

        public void Dispose()
        {
            foreach (var (normal, ctrl) in hotkeyMap.Values)
            {
                normal?.Dispose();
                ctrl?.Dispose();
            }

            hotkeyMap.Clear();
        }
    }
}
