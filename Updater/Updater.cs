using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSO_Utilities.Updater
{
    public static class Updater
    {
        public static void ReplaceWith(string zipPath)
        {
            string appDir = AppDomain.CurrentDomain.BaseDirectory;
            string exeName = Process.GetCurrentProcess().MainModule.FileName;
            string batchPath = Path.Combine(Path.GetTempPath(), "update_dsoutilities.bat");

            string commands = $@"
@echo off
echo Updating DSO Utilities...
ping 127.0.0.1 -n 3 > nul
taskkill /IM ""{Path.GetFileName(exeName)}"" /F > nul 2>&1
timeout /t 2 > nul
powershell -Command ""Expand-Archive -Path '{zipPath}' -DestinationPath '{appDir}' -Force""
start """" ""{exeName}""
del ""%~f0""
";

            File.WriteAllText(batchPath, commands);

            Process.Start(new ProcessStartInfo
            {
                FileName = batchPath,
                UseShellExecute = true,
                Verb = "runas"
            });

            Environment.Exit(0);
        }
    }
}
