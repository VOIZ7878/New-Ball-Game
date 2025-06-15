namespace BallGame
{
    public class GenerationSettings
    {
        public Range EnergyBallRange { get; set; } = new Range(1, 5);
        public Range WallRange { get; set; } = new Range(1, 5);
        public Range EnemyRange { get; set; } = new Range(1, 3);
        public Range FieldWidth { get; set; } = new Range(10, 20);
        public Range FieldHeight { get; set; } = new Range(10, 20);
    }

    public struct Range
    {
        public int Min { get; set; }
        public int Max { get; set; }

        public Range(int min, int max)
        {
            Min = min;
            Max = max;
        }
        public override string ToString() => $"{Min}-{Max}";
    }
}
