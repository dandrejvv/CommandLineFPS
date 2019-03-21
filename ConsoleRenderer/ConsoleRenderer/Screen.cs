using System;

namespace ConsoleRenderer
{
    public class Screen
    {
        private char[] _buffer;
        private char[] _emptyBuffer;
        private int _screenWidth;
        private int _screenHeight;

        public Screen(int screenWidth, int screenHeight)
        {
            ScreenWidth = screenWidth;
            ScreenHeight = screenHeight;
            _buffer = new char[ScreenWidth * ScreenHeight];
            _emptyBuffer = new char[ScreenWidth * ScreenHeight];

            Console.CursorVisible = false;
            Console.SetWindowSize(ScreenWidth, ScreenHeight+1);
        }

        public int ScreenWidth { get { return _screenWidth; } set { _screenWidth = value; } }
        public int ScreenHeight { get { return _screenHeight; } set { _screenHeight = value; } }

        public void Draw(int x, int y, char character)
        {
            _buffer[y * _screenWidth + x] = character;
        }

        public void Draw(int x, int y, string text)
        {
            text.CopyTo(0, _buffer, y * _screenWidth + x, text.Length);
        }

        public void RenderToScreen()
        {
            Console.SetCursorPosition(0, 0);
            Console.Write(_buffer, 0, _buffer.Length);
            _emptyBuffer.CopyTo(_buffer, 0);
        }
    }
}
