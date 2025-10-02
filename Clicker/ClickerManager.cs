using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSO_Utilities.Clicker
{
    public class ClickerManager
    {
        public MouseClicker LeftClicker { get; private set; }
        public MouseClicker RightClicker { get; private set; }


        public ClickerManager() {
            LeftClicker = new MouseClicker(true);
            RightClicker = new MouseClicker(false);
        }
    }
}
