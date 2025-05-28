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
                        .Select(x => field.Grid[x, y] switch
                        {
                            Wall => '#',
                            Player _ => 'P',
                            Ball _ => 'B',
                            EnergyBall _ => 'E',
                            Enemy => 'X',
                            Shield s when Shield.IsShield(s, out var dir) => dir,
                            _ => '.'
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
                    switch (symbol)
                    {
                        case '#':
                            gameField[x, y] = new Wall();
                            break;
                        case 'P':
                            gameField.Player = new Player(x, y);
                            break;
                        case 'B':
                            gameField.Ball = new Ball(x, y);
                            break;
                        case 'E':
                            var energyBall = new EnergyBall();
                            gameField[x, y] = energyBall;
                            gameField.EnergyBallList.Add((energyBall, x, y));
                            break;
                        case 'X':
                            var enemy = new Enemy(x, y);
                            gameField[x, y] = enemy;
                            gameField.Enemies.Add(enemy);
                            break;
                        case '/':
                        case '\\':
                            gameField[x, y] = new Shield(symbol);
                            break;
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