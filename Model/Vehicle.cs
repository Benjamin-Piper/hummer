namespace Hummer.Model
{
    /*
        L = Innerlength
        W = InnerWidth

         (LeftEdge)  +-------------+ (RightEdge, also OuterLength)
                     |             |
                     | (X)+---+(L) |
                     |    |   |    |
                     | (W)+---+    |
                     |             |
        (OuterWidth) +-------------+

        Vehicles only move in one direction. They don't know Y.
    */
    public record class Vehicle
    {
        public static int DrawHeightOffset = (Vehicle.InnerWidth / 2) - Vehicle.StrokeWidth;
        public const int InnerLength = 30;
        public const int InnerWidth = 20;
        public static int OuterLength => Vehicle.InnerLength + (2 * Vehicle.StrokeWidth);
        public static int OuterWidth => Vehicle.InnerWidth + (2 * Vehicle.StrokeWidth);
        public const int MinBumperDistance = 20;
        public const int StrokeWidth = 2;
        public static int GetTopEdge(int y) => y - Vehicle.StrokeWidth;

        public string Colour { get; init; } = "red";
        public int LeftEdge => this.X - Vehicle.StrokeWidth;
        public int RightEdge => this.X + Vehicle.InnerLength + Vehicle.StrokeWidth;
        /// <summary>Spawnrate must be between 0 and 1</summary>
        public double SpawnRate { get; init; }
        public int X { get; set; }
    }
}
