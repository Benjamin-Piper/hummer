namespace Hummer.Model
{
    [Serializable]
    public class SpawnRateOutOfRangeException : Exception
    {
        public SpawnRateOutOfRangeException() : base() { }
        public SpawnRateOutOfRangeException(string message) : base(message) { }
        public SpawnRateOutOfRangeException(string message, Exception inner) : base(message, inner) { }
    }
}
