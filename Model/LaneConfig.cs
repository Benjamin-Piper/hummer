namespace Hummer.Model
{
    public enum Direction { Left, Right }
    public readonly record struct Point(int X, int Y);
    public readonly record struct LaneConfig
    {
        public Direction Direction { get; init; }
        public Point Origin { get; init; }
        public int Width { get; init; }
    }
}