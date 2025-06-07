using System.Text.Json;
using BallGame.Rendering;

namespace BallGame.Utils
{
    public static class JSONLevelLoader
    {
        public static string Serialize(GameField field)
        {
            return JsonSerializer.Serialize(new
            {
                Grid = Enumerable.Range(0, field.Height)
                    .Select(y => new string(Enumerable.Range(0, field.Width)
                        .Select(x =>
                        {
                            var element = field.Grid[x, y];
                            if (element is Shield s && Shield.IsShield(s, out var dir))
                                return dir;
                            if (element != null && ElementRegistry.TypeToInfo.TryGetValue(element.GetType(), out var info))
                                return info.Symbol;
                            return '.';
                        }).ToArray()))
                    .ToList(),
                Player = field.Player == null ? null : new { field.Player.X, field.Player.Y, field.Player.Score },
                Ball = field.Ball == null ? null : new { field.Ball.X, field.Ball.Y, field.Ball.Dx, field.Ball.Dy },
                field.EnergyBallCount,
                field.TotalScore
            });
        }

        public static GameField Deserialize(string json)
        {
            var data = JsonSerializer.Deserialize<SerializedGameField>(json)
                ?? throw new Exception("Failed to deserialize game field.");

            var grid = data.Grid?.Select(row => row.ToCharArray()).ToArray()
                ?? throw new Exception("Grid is missing or empty in saved game.");

            var field = new GameField(grid[0].Length, grid.Length)
            {
                TotalScore = data.TotalScore,
                EnergyBallCount = data.EnergyBallCount
            };

            Level.LevelElementPlacer.LoadSymbols(field, grid);

            if (data.Player is { X: var px, Y: var py, Score: var score })
                field.Player = new Player(px, py) { Score = score };
            if (data.Ball is { X: var bx, Y: var by, Dx: var dx, Dy: var dy })
                field.Ball = new Ball(bx, by) { Dx = dx, Dy = dy };

            return field;
        }

        private class SerializedGameField
        {
            public List<string> Grid { get; set; } = new();
            public int TotalScore { get; set; }
            public int EnergyBallCount { get; set; }
            public PlayerData? Player { get; set; }
            public BallData? Ball { get; set; }

            public class PlayerData
            {
                public int X { get; set; }
                public int Y { get; set; }
                public int Score { get; set; }
            }

            public class BallData
            {
                public int X { get; set; }
                public int Y { get; set; }
                public int Dx { get; set; }
                public int Dy { get; set; }
            }
        }
    }
}