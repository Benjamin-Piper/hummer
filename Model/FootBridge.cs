namespace Hummer.Model
{
    public readonly record struct Footbridge
    {
        public string Colour { get; init; }
        public int LeftEdge { get; init; }
        public int RightEdge { get; init; }
        public int Width => this.RightEdge - this.LeftEdge;
    }
}