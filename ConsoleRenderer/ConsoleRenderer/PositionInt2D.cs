using System;

namespace ConsoleRenderer
{
    public struct PositionInt2D
    {
        public PositionInt2D(int posX, int posY)
        {
            PosX = posX;
            PosY = posY;
        }

        public int PosX { get; }
        public int PosY { get; }
    }
}
