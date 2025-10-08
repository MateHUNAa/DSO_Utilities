using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DSO_Utilities.UI
{
    public static class ToastNotifier
    {
        public static async void Show(Form parent, string message, int duration = 2000)
        {
            Label toast = new Label()
            {
                Text= message,
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.FromArgb(40,40,40),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Padding= new Padding(10),
                Width=220,
                Height=40,
                BorderStyle=BorderStyle.None,
            };

            toast.Left = parent.ClientSize.Width - toast.Width - 20;
            toast.Top = 20;
            toast.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            toast.BringToFront();

            toast.Region = Region.FromHrgn(
                CreateRoundRectRgn(0,0, toast.Width, toast.Height, 15, 15)   
            );

            parent.Controls.Add(toast);
            toast.BringToFront();

            for (int i=0;i <= 10; i++)
            {
                toast.BackColor = Color.FromArgb(40 + (i * 10), 40, 40);
                await Task.Delay(20);
            }

            await Task.Delay(duration);

            for (int i = 10; i>=0; i--)
            {
                toast.BackColor = Color.FromArgb(40 + (i*10), 40, 40);
                await Task.Delay(20);
            }

            parent.Controls.Remove(toast);
            toast.Dispose();
        }


        [System.Runtime.InteropServices.DllImport("gdi32.dll", SetLastError = true)]
        private static extern IntPtr CreateRoundRectRgn(
            int nLeftRect, int nTopRect, int nRightRect, int nBottomRect, int nWidthEllipse, int nHeightEllipse
        );
    }
}
