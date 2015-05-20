using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace snake
{
    public class Point:IEquatable<Point>
    {
        public Point(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }

        public int X { get; private set; }
        public int Y { get; private set; }

        public bool Equals(Point other)
        {
            return (this.X == other.X) && (this.Y == other.Y);
        }

        public override string ToString()
        {
            return string.Format("X = {0}, Y = {1}", X, Y);
        }
    }
}
