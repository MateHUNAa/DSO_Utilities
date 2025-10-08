using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSO_Utilities.Config
{
    public class ConfigData
    {
        public int SleepTime { get; set; } = 1;
        public string LeftHotkey { get; set; } = "F6";
        public string RightHotkey { get; set; } = "F7";

        public Point InventoryFirstCellPosition { get; set; } = Point.Empty;
        public Point InventoryFirstBagPosition { get; set; } = Point.Empty;
        public int AmountOfBagsUnlocked { get; set; } = 9;
        public Dictionary<string, string> ReviveHotkeys {  get; set; } = new Dictionary<string, string>
        {
            { "Slot1", "F1" },
            { "Slot2", "F2" },
            { "Slot3", "F3" },
            { "Slot4", "F4" },
            { "Slot5", "F5" }
        };

        public Dictionary<string, Point> RevivePositions { get; set; } = new Dictionary<string, Point>
        {
            { "Slot1", Point.Empty },
            { "Slot2", Point.Empty },
            { "Slot3", Point.Empty },
            { "Slot4", Point.Empty },
            { "Slot5", Point.Empty }
        };
    }
}
