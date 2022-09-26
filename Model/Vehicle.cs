namespace Hummer.Model
{
    public record class Vehicle
    {
        public string Colour { get; init; } = "red";
        public int LeftEdge => this.X - Vehicle.StrokeWidth;
        public int Length { get; init; }
        public const int MinBumperDistance = 20;
        /// <summary>Spawnrate must be between 0 and 1</summary>
        public double SpawnRate { get; init; }
        public const int StrokeWidth = 2;
        public int TotalLength => this.Length + (2 * Vehicle.StrokeWidth);
        public int TotalWidth => this.Width + (2 * Vehicle.StrokeWidth);
        public int Width { get; init; }
        public int X { get; set; }
    }
}
