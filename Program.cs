using DSO_Utilities.Updater;
using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DSO_Utilities
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            try
            {
                Task.Run(async () => await UpdateChecker.CheckAsync()).Wait();
            } catch (Exception ex)
            {
                MessageBox.Show("Update check failed: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            Application.Run(new MainForm());
        }
    }
}
