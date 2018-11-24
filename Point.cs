namespace DwarfFortress
{
    /// <summary>
    /// Authors: Alec Mills and Josh DeMoss
    ///
    /// Represents a point in Cartesian space
    /// </summary>
    public struct Point
    {
        public int X { get; }
        public int Y { get; }

        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }

        public Point Add(Point p)
        {
            return new Point(X + p.X, Y + p.Y);
        }
    }
}