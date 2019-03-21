using System;

namespace ConsoleRenderer
{
    public interface IScreen
    {
        int ScreenWidth { get; }
        int ScreenHeight { get; }
        void Draw(int x, int y, char character);
        void Draw(int x, int y, string text);
        void RenderToScreen();
    }
}
