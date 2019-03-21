using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace ConsoleRenderer
{
    public class NativeWindowsScreen : IScreen
    {
        [DllImport("Kernel32.dll", SetLastError = true)]
        private static extern IntPtr CreateConsoleScreenBuffer(
            UInt32 dwDesiredAccess,
            UInt32 dwShareMode,
            IntPtr secutiryAttributes,
            UInt32 flags,
            IntPtr screenBufferData);

        [DllImport("Kernel32.dll", SetLastError = true)]
        private static extern bool SetConsoleActiveScreenBuffer(IntPtr hConsoleOutput);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool WriteConsoleOutputCharacter(
          IntPtr hConsoleOutput,
          char[] lpCharacter,
          int nLength,
          COORD dwWriteCoord,
          ref int lpumberOfCharsWritten);

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern bool SetConsoleWindowInfo(IntPtr hConsoleOutput,
            bool absolute, ref SMALL_RECT consoleWindow);

        [StructLayout(LayoutKind.Sequential)]
        internal struct COORD
        {
            public COORD(short x, short y)
            {
                X = x;
                Y = y;
            }
            internal short X;
            internal short Y;
        }

        [StructLayoutAttribute(LayoutKind.Sequential)]
        internal struct SMALL_RECT
        {
            internal short Left;
            internal short Top;
            internal short Right;
            internal short Bottom;
        }

        const uint GENERIC_READ = 0x80000000;
        const uint GENERIC_WRITE = 0x40000000;
        const uint FILE_SHARE_READ = 0x00000001;
        const uint FILE_SHARE_WRITE = 0x00000002;
        const uint CONSOLE_TEXTMODE_BUFFER = 1;

        private char[] _buffer;
        private char[] _emptyBuffer;
        private int _screenWidth;
        private int _screenHeight;
        private readonly IntPtr _consoleHandle;

        public NativeWindowsScreen(int screenWidth, int screenHeight)
        {
            ScreenWidth = screenWidth;
            ScreenHeight = screenHeight;
            _buffer = new char[ScreenWidth * ScreenHeight];
            _emptyBuffer = new char[ScreenWidth * ScreenHeight];

            _consoleHandle = CreateConsoleScreenBuffer(GENERIC_READ | GENERIC_WRITE, 0, IntPtr.Zero, CONSOLE_TEXTMODE_BUFFER, IntPtr.Zero);

            SetConsoleActiveScreenBuffer(_consoleHandle);
        }

        public int ScreenWidth { get { return _screenWidth; } private set { _screenWidth = value; } }
        public int ScreenHeight { get { return _screenHeight; } private set { _screenHeight = value; } }

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
            int writtenChars = 0;
            WriteConsoleOutputCharacter(_consoleHandle, _buffer, _buffer.Length, new COORD(0 , 0), ref writtenChars);
            _emptyBuffer.CopyTo(_buffer, 0);
        }
    }
}
