using System.Runtime.InteropServices;
using System.Threading;
using System.Timers;
using System.Windows.Forms;
using Timer = System.Windows.Forms.Timer;


namespace DSO_Utilities.Clicker
{
    public class MouseClicker
    {
        [DllImport("user32.dll")]
        private static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);

        private const int MOUSEEVENTF_LEFTDOWN = 0x02;
        private const int MOUSEEVENTF_LEFTUP = 0x04;
        private const int MOUSEEVENTF_RIGHTDOWN = 0x08;
        private const int MOUSEEVENTF_RIGHTUP = 0x10;

        private Timer timer = new System.Windows.Forms.Timer();
        private readonly bool isLeftClick;

        public bool IsRunning => timer.Enabled;

        public MouseClicker(bool isLeftClick) {
            this.isLeftClick = isLeftClick;

            timer.Tick += (s, e) =>
            {
                if (isLeftClick)
                {
                    mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
                    mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
                }
                else
                {
                    mouse_event(MOUSEEVENTF_RIGHTDOWN, 0, 0, 0, 0);
                    mouse_event(MOUSEEVENTF_RIGHTUP, 0, 0, 0, 0);
                }
            };
            
        }

        public void Start(int interval) {
            if (interval < 1) interval = 1;
            timer.Interval = interval;
            timer.Start();
        }

        public void Stop()
        {
            timer.Stop();
        }
    }
}
