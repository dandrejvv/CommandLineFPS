using System;
using System.Reflection;
using System.Runtime.InteropServices;

namespace ConsoleRenderer
{
    public class NativeWindowsScreen : IScreen
    {
        const String KERNEL32 = "kernel32.dll";

        [DllImport(KERNEL32, SetLastError = true)]
        static extern bool WriteConsole(
            IntPtr hConsoleOutput,
            byte[] lpCharacter,
            int nLength,
            out int lpumberOfCharsWritten,
            IntPtr lpReserved);

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

        public void Draw(int x, int y, string text)
        {
            text.CopyTo(0, _buffer, y * _screenWidth + x, text.Length);
        }

        public void RenderToScreen()
        {
            int writtenChars = 0;
            // Finally figured out why my wall characters didn't get drawn the right way. Has to do with the Unicode
            // characters that are not being properly marshaled between .NET and the Windows Native console.
            // This is the main reason why it renders so slow using the normal Console. There goes my performance! :-(
            var bufferBytes = Console.OutputEncoding.GetBytes(_buffer);

            // Once the byte version of the text is obtained (in the correct format for unicode characters)
            // we can pass it through as-is to the underlying Windows function.
            // There is a weird top-line flicker happening for some reason.
            if (!WriteConsole(_consoleHandle, bufferBytes, _buffer.Length, out writtenChars, IntPtr.Zero))
            {
                var error = Marshal.GetLastWin32Error();
            }
            _emptyBuffer.CopyTo(_buffer, 0);
        }
    }
}
