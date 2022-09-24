public record struct Vehicle
{
    public string Colour { get; init; }
    public int Length { get; init; }
    /// <summary>Spawnrate must be between 0 and 1</summary>
    public double SpawnRate { get; init; }
    public int Width { get; init; }
    public int X { get; set; }
}