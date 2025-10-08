using System;
using System.Diagnostics;
using System.IO;

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

rem Kill the running application
taskkill /IM ""{Path.GetFileName(exeName)}"" /F > nul 2>&1
echo ""{exeName}"" Killed.
timeout /t 2 > nul

rem Delete old files (keep config.json)
echo Deleting old files...
pushd ""{appDir}""
attrib -R /S /D * > nul 2>&1
for %%f in (*.*) do (
    if /I not ""%%~nxf""==""config.json"" del /F /Q ""%%f"" > nul 2>&1
)
for /D %%d in (*) do rd /S /Q ""%%d"" > nul 2>&1
popd

rem Extract new files into temp folder and copy Release contents
echo Extracting new files...
powershell -NoLogo -NoProfile -Command ^
""$tmp = [System.IO.Path]::Combine([System.IO.Path]::GetTempPath(), 'dso_update_tmp'); ^
if (Test-Path $tmp) {{ Remove-Item $tmp -Recurse -Force }}; ^
Expand-Archive -LiteralPath '{zipPath}' -DestinationPath $tmp -Force; ^
$release = Join-Path $tmp 'Release'; ^
if (Test-Path $release) {{ Copy-Item -Path (Join-Path $release '*') -Destination '{appDir}' -Recurse -Force }} else {{ Copy-Item -Path (Join-Path $tmp '*') -Destination '{appDir}' -Recurse -Force }}; ^
Remove-Item $tmp -Recurse -Force""

rem Start the application
echo Starting DSO Utilities...
start """" ""{exeName}""

echo.
echo Update complete! Press any key to close this window.
pause > nul

rem Self-delete batch after 3 seconds
(
    start cmd /c ""timeout /t 3 > nul & del ""%~f0"" ""
) > nul 2>&1

exit /b
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
