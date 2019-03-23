using System;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;
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

        [DllImport(KERNEL32, SetLastError = true)]
        static extern bool SetConsoleWindowInfo(
            IntPtr hConsoleOutput,
            bool bAbsolute,
            ref SMALL_RECT lpConsoleWindow);

        [DllImport(KERNEL32, SetLastError = true)]
        static extern bool SetConsoleScreenBufferSize(
            IntPtr hConsoleOutput,
            COORD  dwSize);

        [DllImport(KERNEL32, SetLastError = true)]
        static extern bool SetConsoleActiveScreenBuffer(
            IntPtr hConsoleOutput);

        [DllImport(KERNEL32, SetLastError = true)]
        static extern bool SetCurrentConsoleFontEx(
            IntPtr ConsoleOutput,
            bool MaximumWindow,
            ref CONSOLE_FONT_INFO_EX ConsoleCurrentFontEx);

        [DllImport(KERNEL32, SetLastError = true)]
        static extern bool GetConsoleScreenBufferInfo(
            IntPtr hConsoleOutput,
            out CONSOLE_SCREEN_BUFFER_INFO ConsoleScreenBufferInfo);

        [DllImport(KERNEL32, SetLastError = true)]
        static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);

        [DllImport(KERNEL32, SetLastError = true)]
        static extern bool SetConsoleCtrlHandler(
            ConsoleCtrlDelegate HandlerRoutine,
            bool Add);

        [StructLayout(LayoutKind.Sequential)]
        struct SMALL_RECT
        {
            public SMALL_RECT(short left, short top, short right, short bottom)
            {
                Left = left;
                Top = top;
                Right = right;
                Bottom = bottom;
            }

            public short Left;
            public short Top;
            public short Right;
            public short Bottom;
        };

        [StructLayout(LayoutKind.Sequential)]
        struct COORD
        {
            public COORD(short x, short y)
            {
                X = x;
                Y = y;
            }
            internal short X;
            internal short Y;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        struct CONSOLE_FONT_INFO_EX
        {
            public uint cbSize;
            public uint nFont;
            public COORD dwFontSize;
            public int FontFamily;
            public int FontWeight;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)] // Edit sizeconst if the font name is too big
            public string FaceName;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct CONSOLE_SCREEN_BUFFER_INFO
        {
            public COORD dwSize;
            public COORD dwCursorPosition;
            public short wAttributes;
            public SMALL_RECT srWindow;
            public COORD dwMaximumWindowSize;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct COLORREF
        {
            public uint ColorDWORD;

            public COLORREF(System.Drawing.Color color)
            {
                ColorDWORD = (uint)color.R + (((uint)color.G) << 8) + (((uint)color.B) << 16);
            }

            public System.Drawing.Color GetColor()
            {
                return System.Drawing.Color.FromArgb((int)(0x000000FFU & ColorDWORD),
                   (int)(0x0000FF00U & ColorDWORD) >> 8, (int)(0x00FF0000U & ColorDWORD) >> 16);
            }

            public void SetColor(System.Drawing.Color color)
            {
                ColorDWORD = (uint)color.R + (((uint)color.G) << 8) + (((uint)color.B) << 16);
            }
        }

        delegate Boolean ConsoleCtrlDelegate(CtrlTypes CtrlType);

        enum CtrlTypes : uint
        {
            CTRL_C_EVENT = 0,
            CTRL_BREAK_EVENT,
            CTRL_CLOSE_EVENT,
            CTRL_LOGOFF_EVENT = 5,
            CTRL_SHUTDOWN_EVENT
        }

        [Flags]
        enum ConsoleModes : uint
        {
            ENABLE_PROCESSED_INPUT = 0x0001,
            ENABLE_LINE_INPUT = 0x0002,
            ENABLE_ECHO_INPUT = 0x0004,
            ENABLE_WINDOW_INPUT = 0x0008,
            ENABLE_MOUSE_INPUT = 0x0010,
            ENABLE_INSERT_MODE = 0x0020,
            ENABLE_QUICK_EDIT_MODE = 0x0040,
            ENABLE_EXTENDED_FLAGS = 0x0080,
            ENABLE_AUTO_POSITION = 0x0100,

            ENABLE_PROCESSED_OUTPUT = 0x0001,
            ENABLE_WRAP_AT_EOL_OUTPUT = 0x0002,
            ENABLE_VIRTUAL_TERMINAL_PROCESSING = 0x0004,
            DISABLE_NEWLINE_AUTO_RETURN = 0x0008,
            ENABLE_LVB_GRID_WORLDWIDE = 0x0010
        }

        enum FamilyFont : int
        {
            FF_DONTCARE = 0x00,
            FF_ROMAN = 0x01,
            FF_SWISS = 0x02,
            FF_MODERN = 0x03,
            FF_SCRIPT = 0x04,
            FF_DECORATIVE = 0x05
        }

        const int FW_DONTCARE = 0;
        const int FW_NORMAL = 400;
        const int FW_BOLD = 700;

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

            _consoleHandle = GetConsoleHandle();

            SMALL_RECT m_rectWindow = new SMALL_RECT( 0, 0, 1, 1 );
            SetConsoleWindowInfo(_consoleHandle, true, ref m_rectWindow);

            COORD coord = new COORD ((short)ScreenWidth, (short)ScreenHeight);
            if (!SetConsoleScreenBufferSize(_consoleHandle, coord))
            {
                var error = Marshal.GetLastWin32Error();
                throw new Win32Exception(error);
            }

            if (!SetConsoleActiveScreenBuffer(_consoleHandle))
            {
                var error = Marshal.GetLastWin32Error();
                throw new Win32Exception(error);
            }

            CONSOLE_FONT_INFO_EX cfi = new CONSOLE_FONT_INFO_EX();
            cfi.cbSize = (uint)System.Runtime.InteropServices.Marshal.SizeOf<CONSOLE_FONT_INFO_EX>(cfi);
            cfi.nFont = 0;
            cfi.dwFontSize.X = 4;
            cfi.dwFontSize.Y = 4;
            cfi.FontFamily = (int)FamilyFont.FF_DONTCARE;
            cfi.FontWeight = FW_NORMAL;

            cfi.FaceName = "Consolas";
            if (!SetCurrentConsoleFontEx(_consoleHandle, false, ref cfi))
            {
                var error = Marshal.GetLastWin32Error();
                throw new Win32Exception(error);
            }

            CONSOLE_SCREEN_BUFFER_INFO csbi;
            if (!GetConsoleScreenBufferInfo(_consoleHandle, out csbi))
            {
                var error = Marshal.GetLastWin32Error();
                throw new Win32Exception(error);
            }
            if (ScreenHeight > csbi.dwMaximumWindowSize.Y)
            {
                var error = Marshal.GetLastWin32Error();
                throw new Win32Exception(error);
            }
            if (ScreenWidth > csbi.dwMaximumWindowSize.X)
            {
                var error = Marshal.GetLastWin32Error();
                throw new Win32Exception(error);
            }

            m_rectWindow = new SMALL_RECT( 0, 0, (short)(ScreenWidth - 1), (short)(ScreenHeight - 1));
            if (!SetConsoleWindowInfo(_consoleHandle, true, ref m_rectWindow))
            {
                var error = Marshal.GetLastWin32Error();
                throw new Win32Exception(error);
            }

            // Set flags to allow mouse input		
            //if (!SetConsoleMode(_consoleHandle, (uint)(ConsoleModes.ENABLE_EXTENDED_FLAGS | ConsoleModes.ENABLE_WINDOW_INPUT)))
            //{
            //    var error = Marshal.GetLastWin32Error();
            //    throw new Win32Exception(error);
            //}

            SetConsoleCtrlHandler(CloseHandler, true);
        }

        static bool CloseHandler(CtrlTypes evt)
        {
            if (evt == CtrlTypes.CTRL_CLOSE_EVENT)
            {
                
            }
            return true;
        }

        // I'm leveraging the existing .NET Core Console Handle since I don't know what it would entail to
        // create my own handle while one is already running. Hopefully I won't have to change this anytime soon.
        // I have tried to fetch this in other ways but no luck there. This seems to be the only way.
        // I'm so glad this is just for an experiment and not for production apps otherwise I would be hating myself now! :-D
        private IntPtr GetConsoleHandle()
        {
            var consoleType = typeof(Console);
            var consolePalType = consoleType.Assembly.GetType("System.ConsolePal");
            var outputHandleProp = consolePalType.GetProperty("OutputHandle", BindingFlags.Static | BindingFlags.NonPublic);
            return (IntPtr)outputHandleProp.GetValue(null);
        }

        public int ScreenWidth { get { return _screenWidth; } private set { _screenWidth = value; } }
        public int ScreenHeight { get { return _screenHeight; } private set { _screenHeight = value; } }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Draw(int x, int y, char character)
        {
            _buffer[y * _screenWidth + x] = character;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Draw(int x, int y, char[] chars, int offset, int length)
        {
            Array.Copy(chars, offset, _buffer, y * _screenWidth + x, length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
                throw new Win32Exception(error);
            }
            _emptyBuffer.CopyTo(_buffer, 0);
        }
    }
}
