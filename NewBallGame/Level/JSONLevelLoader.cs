using System.Text.Json;
using BallGame.Rendering;
using BallGame.Input;

namespace BallGame.Utils
{
    public static class JSONLevelLoader
    {
        public static string Serialize(GameField field)
        {
            var state = new
            {
                Width = field.Width,
                Height = field.Height,
                Grid = Enumerable.Range(0, field.Height)
                    .Select(y => new string(Enumerable.Range(0, field.Width)
                        .Select(x =>
                        {
                            var element = field.Grid[x, y];
                            if (element is Shield s && Shield.IsShield(s, out var dir))
                                return dir;
                            if (element != null && BallGame.Rendering.ElementRegistry.TypeToInfo.TryGetValue(element.GetType(), out var info))
                                return info.Symbol;
                            return '.';
                        }).ToArray()))
                    .ToList(),
                Player = new { field.Player.X, field.Player.Y, field.Player.Score },
                Ball = field.Ball != null ? new { field.Ball.X, field.Ball.Y, field.Ball.Dx, field.Ball.Dy } : null,
                field.EnergyBallCount,
                field.TotalScore
            };
            return JsonSerializer.Serialize(state);
        }

        public static GameField Deserialize(string json, IRenderer renderer, ISoundManager soundManager, IInputManager inputManager)
        {
            var data = JsonSerializer.Deserialize<SerializedGameField>(json);
            if (data == null) throw new Exception("Failed to deserialize game field.");

            var gameField = new GameField(data.Width, data.Height)
            {
                TotalScore = data.TotalScore,
                EnergyBallCount = data.EnergyBallCount
            };

            for (int y = 0; y < data.Grid.Count; y++)
            {
                for (int x = 0; x < data.Grid[y].Length; x++)
                {
                    char symbol = data.Grid[y][x];
                    if (symbol == '/' || symbol == '\\')
                    {
                        gameField[x, y] = new Shield(symbol);
                        continue;
                    }
                    if (BallGame.Rendering.ElementRegistry.SymbolToInfo.TryGetValue(symbol, out var info))
                    {
                        var element = info.Factory(x, y);
                        switch (element)
                        {
                            case Player player:
                                gameField.Player = player;
                                break;
                            case Ball ball:
                                gameField.Ball = ball;
                                break;
                            case EnergyBall energyBall:
                                gameField[x, y] = energyBall;
                                gameField.EnergyBallList.Add((energyBall, x, y));
                                break;
                            case Enemy enemy:
                                gameField[x, y] = enemy;
                                gameField.Enemies.Add(enemy);
                                break;
                            default:
                                gameField[x, y] = element;
                                break;
                        }
                        continue;
                    }
                    if (symbol == '.')
                    {
                        continue;
                    }
                }
            }

            if (data.Player != null)
            {
                gameField.Player = new Player(data.Player.X, data.Player.Y)
                {
                    Score = data.Player.Score
                };
            }
            if (data.Ball != null)
            {
                gameField.Ball = new Ball(data.Ball.X, data.Ball.Y)
                {
                    Dx = data.Ball.Dx,
                    Dy = data.Ball.Dy
                };
            }
            return gameField;
        }

        private class SerializedGameField
        {
            public int Width { get; set; }
            public int Height { get; set; }
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