using DSO_Utilities.Config;
using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsInput;

namespace DSO_Utilities
{
    public class Inventory
    {
        private readonly InputSimulator _input;
        private readonly string _tempPath;

        [DllImport("kernel32.dll")]
        private static extern bool AllocConsole();

        [DllImport("user32.dll")]
        private static extern bool BlockInput(bool fBlockIt);

        [DllImport("user32.dll")]
        private static extern bool GetWindowRect(IntPtr hWnd, out RECT rect);

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        public Inventory(string templatePath)
        {
            //AllocConsole();
            _input = new InputSimulator();
            _tempPath = templatePath;
        }
        public async Task ScanAndSell(IntPtr hWnd, int bagNumber=0)
        {
            int rows = 4;
            int cols = 7;
            Point inventoryStart = ConfigManager.Load().InventoryFirstCellPosition;

            int cellWidth = 75;
            int cellHeight = 75;
            int border = 8;

            using var template = new Image<Gray, byte>(_tempPath);
            string folderPath = "Assets";
            string screenshotPath = Path.Combine(folderPath, "screen.png");

            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols; col++)
                {
                    int cellX = inventoryStart.X-5 + col * (cellWidth+5) ;
                    int cellY = inventoryStart.Y-5 + row * (cellHeight+5) ;
                    Rectangle cellRect = new Rectangle(cellX, cellY, cellWidth, cellHeight);

                    using var cellBmp = CaptureRegion(hWnd, cellRect);
                    string cellPath = Path.Combine(folderPath, $"cell_{row}_{col}.png");
                    cellBmp.Save(cellPath);

                    using var cellImg = new Image<Gray, byte>(cellPath);
                    using var result = cellImg.MatchTemplate(template, Emgu.CV.CvEnum.TemplateMatchingType.CcoeffNormed);

                    double[] minValues, maxValues;
                    Point[] minLocations, maxLocations;
                    result.MinMax(out minValues, out maxValues, out minLocations, out maxLocations);

                    Console.WriteLine($"bag{bagNumber} row:{row} col:{col} pred:{maxValues[0]}");

                    if (maxValues[0] > 0.39)
                    {
                        //string foundPath = Path.Combine(folderPath, $"found_{row}_{col}.png");
                        //Rectangle matchRect = new Rectangle(maxLocations[0], template.Size);
                        //cellImg.Draw(matchRect, new Gray(255), 2);
                        //cellImg.Save(foundPath);

                        GetWindowRect(hWnd, out RECT windowRect);
                        int windowX = windowRect.Left;
                        int windowY = windowRect.Top;

                        Cursor.Position = new Point(
                            windowX + cellX + maxLocations[0].X + template.Width / 2,
                            windowY + cellY + maxLocations[0].Y + template.Height / 2
                        );

                        await Task.Delay(35);
                        _input.Mouse.RightButtonClick();
                        await Task.Delay(35);
                    }
                }
            }
            Console.WriteLine("--------------------------------");
        }
        public async Task ScanAllBags(IntPtr hWnd)
        {
            ConfigData config = ConfigManager.Load();
            GetWindowRect(hWnd, out RECT windowRect);
            int windowX = windowRect.Left;
            int windowY = windowRect.Top;


            BlockInput(true);
            for (int bag = 0; bag<config.AmountOfBagsUnlocked; bag++)
            {
                int mX = windowX + config.InventoryFirstBagPosition.X + (50*bag);
                int mY = windowY+ config.InventoryFirstBagPosition.Y;

                Cursor.Position = new Point(mX,mY);
                await Task.Delay(15);
                _input.Mouse.LeftButtonClick();
                await Task.Delay(25);
                Cursor.Position = new Point(1, 1);
                await Task.Delay(30);
                await ScanAndSell(hWnd, bag);
            }
            BlockInput(false);
        }
        private static Bitmap CaptureWindow(IntPtr hWnd)
        {
            GetWindowRect(hWnd, out RECT rect);
            int width = rect.Right - rect.Left;
            int height = rect.Bottom - rect.Top;
            Bitmap bmp = new Bitmap(width, height);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.CopyFromScreen(rect.Left, rect.Top, 0, 0, new Size(width, height), CopyPixelOperation.SourceCopy);
            }
            return bmp;
        }
        private static Bitmap CaptureRegion(IntPtr hWnd, Rectangle region)
        {
            GetWindowRect(hWnd, out RECT rect);
            var bmp = new Bitmap(region.Width, region.Height);
            using var g = Graphics.FromImage(bmp);
            g.CopyFromScreen(rect.Left + region.Left, rect.Top + region.Top, 0, 0,
                new Size(region.Width, region.Height), CopyPixelOperation.SourceCopy);
            return bmp;
        }

        public static Point GetMouseRelToWindow(IntPtr hWnd)
        {
            GetWindowRect(hWnd, out RECT rect);
            Point m = Cursor.Position;

            int relX = m.X - rect.Left-30;
            int relY = m.Y - rect.Top-30;
            return new Point(relX, relY);
        }
    }
}
