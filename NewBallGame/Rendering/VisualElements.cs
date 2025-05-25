using System;
using System.Collections.Generic;
using System.Drawing;

namespace BallGame.Rendering
{
    public struct VisualElement
    {
        public string Symbol { get; }
        public ConsoleColor ConsoleColor { get; }
        public Brush WinFormsBrush { get; }

        public VisualElement(string symbol, ConsoleColor consoleColor, Brush winFormsBrush)
        {
            Symbol = symbol;
            ConsoleColor = consoleColor;
            WinFormsBrush = winFormsBrush;
        }
    }

    public static class ElementVisuals
    {
        public static readonly Dictionary<Type, VisualElement> Visuals = new()
        {
            { typeof(Wall), new VisualElement("#", ConsoleColor.Gray, Brushes.DarkGray) },
            { typeof(EnergyBall), new VisualElement("@", ConsoleColor.Cyan, Brushes.Yellow) },
            { typeof(Enemy), new VisualElement("E", ConsoleColor.Magenta, Brushes.Red) },
            { typeof(Player), new VisualElement("I", ConsoleColor.Green, Brushes.Green) },
            { typeof(Ball), new VisualElement("O", ConsoleColor.Red, Brushes.White) },
            { typeof(SmartEnemy), new VisualElement("S", ConsoleColor.Red, Brushes.White) },
            { typeof(BossEnemy), new VisualElement("B", ConsoleColor.Red, Brushes.White) },
            { typeof(Hint), new VisualElement("*", ConsoleColor.Yellow, Brushes.Yellow) }
        };

        public static VisualElement Get(GameElement element)
        {
            if (element is Shield shield)
            {
                return new VisualElement(
                    shield.Direction.ToString(),
                    ConsoleColor.Blue,
                    Brushes.Cyan
                );
            }

            return Visuals.TryGetValue(element.GetType(), out var visual)
                ? visual
                : new VisualElement(" ", ConsoleColor.White, Brushes.White);
        }
    }
}