using System;
using System.IO;

namespace BallGame
{
    public class TextFileLevelLoader : ILevelLoader
    {
        public void Load(GameField gameField, string fileName)
        {
            string fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Levels", fileName);

            if (!File.Exists(fullPath))
                throw new FileNotFoundException("Level file not found at: " + fullPath);

            var lines = File.ReadAllLines(fullPath);
            int height = lines.Length;
            int width = lines[0].Length;

            if (width != gameField.Width || height != gameField.Height)
                throw new Exception("The level size in the file does not match the GameField size.");

            gameField.Enemies.Clear();
            gameField.EnergyBallList.Clear();

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    char symbol = lines[y][x];
                    gameField[x, y] = null;

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
                        case 'S':
                            var Senemy = new SmartEnemy(x, y);
                            gameField[x, y] = Senemy;
                            gameField.Enemies.Add(Senemy);
                            break;
                        case 'Q':
                            var Benemy = new BossEnemy(x, y);
                            gameField[x, y] = Benemy;
                            gameField.Enemies.Add(Benemy);
                            break;
                        case '.':
                            break;
                        default:
                            throw new Exception($"Unknown symbol in level file: '{symbol}'");
                    }
                }
            }

            if (gameField.EnergyBallList.Count > 0)
                gameField.EnergyBall = gameField.EnergyBallList[0].Item1;

            gameField.EnergyBallCount = gameField.EnergyBallList.Count;
        }
    }
}
