using System;
using System.IO;

namespace BallGame
{
    public class TextFileLevelLoader : ILevelLoader
    {
        public void Load(GameField gameField, string fileName)
        {
            string fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "assets", "Levels", fileName);

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
                    throw new Exception($"Unknown symbol in level file: '{symbol}'");
                }
            }

            if (gameField.EnergyBallList.Count > 0)
                gameField.EnergyBall = gameField.EnergyBallList[0].Item1;

            gameField.EnergyBallCount = gameField.EnergyBallList.Count;
        }
    }
}