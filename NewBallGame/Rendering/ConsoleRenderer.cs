using System;

namespace BallGame.Rendering
{
    public class ConsoleRenderer : IRenderer
    {
        public void RenderAt(int x, int y, string content)
        {
            Console.SetCursorPosition(x, y);
            Console.Write(content);
        }

        public void Clear()
        {
            Console.Clear();
        }

        public void RenderFrame()
        {
            Console.SetCursorPosition(0, 0);
        }
    }
}