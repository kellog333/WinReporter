using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinReporter
{
    internal class Helpers
    {
        public class ButtonPosition
        {
            public int TopLeftX { get; set; }
            public int TopLeftY { get; set; }
            public int BottomRightX
            {
                get { return TopLeftX + Width; }
            }
            public int BottomRightY
            {
                get { return TopLeftY + Height; }
            }
            public int Width { get; set; }
            public int Height { get; set; }

            public ButtonPosition(int topLeftX, int topLeftY, int width, int height)
            {
                TopLeftX = topLeftX;
                TopLeftY = topLeftY;
                Width = width;
                Height = height;
            }

            public bool Overlaps(ButtonPosition buttonPosition)
            {
                return (buttonPosition.TopLeftX < this.BottomRightX && buttonPosition.BottomRightX > this.TopLeftX && buttonPosition.TopLeftY < this.BottomRightY && buttonPosition.BottomRightY > this.TopLeftY);
            }
        }
    }
}
