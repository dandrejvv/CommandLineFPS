using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace ConsoleRenderer.ConsoleScreens
{
    public class NativeWindowsScreen : IConsoleScreen
    {
        const String KERNEL32 = "kernel32.dll";

        [DllImport(KERNEL32, SetLastError = true)]
        static extern bool WriteConsoleOutputCharacter(
            IntPtr hConsoleOutput,
            byte[] lpCharacter, 
            int nLength, 
            COORD dwWriteCoord,
            out int lpNumberOfCharsWritten);

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

            Console.CursorVisible = false;
            Console.SetWindowSize(ScreenWidth, ScreenHeight + 1);

            _consoleHandle = GetConsoleHandle();
        }

        // I'm cheating a little bit. Instead of creating my own handle of the console and trying to setup all the various
        // bells and whistles, I'm using the normal .NET Core version of the Windows implementation and fetching the handle
        // from there.
        private IntPtr GetConsoleHandle()
        {
            var consoleType = typeof(Console);
            var consolePalType = consoleType.Assembly.GetType("System.ConsolePal");
            var outputHandleProp = consolePalType.GetProperty("OutputHandle", BindingFlags.Static | BindingFlags.NonPublic);
            return (IntPtr)outputHandleProp.GetValue(null);
        }

        public int ScreenWidth { get { return _screenWidth; } private set { _screenWidth = value; } }
        public int ScreenHeight { get { return _screenHeight; } private set { _screenHeight = value; } }

        public void Draw(int x, int y, char character)
        {
            _buffer[y * _screenWidth + x] = character;
        }

        public void Draw(int x, int y, char[] chars, int offset, int length)
        {
            Array.Copy(chars, offset, _buffer, y * _screenWidth + x, length);
        }

        public void Draw(int x, int y, string text)
        {
            text.CopyTo(0, _buffer, y * _screenWidth + x, text.Length);
        }

        public void RenderToConsole()
        {
            int writtenChars = 0;

            var correctBytes = Console.OutputEncoding.GetBytes(_buffer);

            // Once the byte version of the text is obtained (in the correct format for Unicode characters)
            // we can pass it through as-is to the underlying Windows function.
            // There is a weird top-line flicker happening for some reason.
            if (!WriteConsoleOutputCharacter(_consoleHandle, correctBytes, _buffer.Length, new COORD(0, 0), out writtenChars))
            {
                var error = Marshal.GetLastWin32Error();
            }
            _emptyBuffer.CopyTo(_buffer, 0);
        }
    }
}
