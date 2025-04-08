using System.Drawing;

namespace BallGame.Rendering
{
    public class WinFormsRenderer : IRenderer
    {
#if WINDOWS
        private Graphics graphics;

        public WinFormsRenderer(Graphics graphics)
        {
            this.graphics = graphics;
        }

        public void RenderAt(int x, int y, string content)
        {
            graphics.DrawString(content, SystemFonts.DefaultFont, Brushes.Black, x * 10, y * 10);
        }

        public void Clear()
        {
            graphics.Clear(Color.White);
        }

        public void RenderFrame()
        {
        }
#else
        public void RenderAt(int x, int y, string content)
        {
            throw new PlatformNotSupportedException("WinFormsRenderer is only supported on Windows.");
        }

        public void Clear()
        {
            throw new PlatformNotSupportedException("WinFormsRenderer is only supported on Windows.");
        }

        public void RenderFrame()
        {
            throw new PlatformNotSupportedException("WinFormsRenderer is only supported on Windows.");
        }
#endif
    }
}