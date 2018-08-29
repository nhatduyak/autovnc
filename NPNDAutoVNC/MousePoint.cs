namespace NPNDAutoVNC
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct MousePoint
    {
        public int X;
        public int Y;
        public MousePoint(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }
    }
}

