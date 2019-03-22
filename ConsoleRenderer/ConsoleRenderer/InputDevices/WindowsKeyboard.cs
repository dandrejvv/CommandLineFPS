using System;
using System.Runtime.InteropServices;

namespace ConsoleRenderer.InputDevices
{
    public class WindowsKeyboard : IKeyboard
    {
        const String USER32 = "User32.dll";

        [DllImport(USER32, SetLastError = true)]
        static extern short GetAsyncKeyState(
            int vKey
        );

        struct KeyState
        {
            public bool Pressed;
            public bool Released;
            public bool Held;
        }

        private short[] _keyOldState;
        private short[] _keyNewState;
        private KeyState[] _keyCurrentState;

        public WindowsKeyboard()
        {
            _keyOldState = new short[256];
            _keyNewState = new short[256];
            _keyCurrentState = new KeyState[256];
        }

        public bool HasKeyPressed()
        {
            bool keyPressed = false;

            for (int i = 0; i < 256; i++)
            {
                _keyNewState[i] = GetAsyncKeyState(i);

                _keyCurrentState[i].Pressed = false;
                _keyCurrentState[i].Released = false;

                if (_keyNewState[i] != _keyOldState[i])
                {
                    if (_keyNewState[i] == 1)
                    {
                        _keyCurrentState[i].Pressed = true;
                        _keyCurrentState[i].Held = false;
                        keyPressed = true;
                    }
                    else if ((_keyNewState[i] & Int16.MinValue) != 0)
                    {
                        _keyCurrentState[i].Pressed = false;
                        _keyCurrentState[i].Held = true;
                        keyPressed = true;
                    }
                    else
                    {
                        _keyCurrentState[i].Released = true;
                        _keyCurrentState[i].Held = false;
                    }
                }

                _keyOldState[i] = _keyNewState[i];
            }

            return keyPressed;
        }

        public bool IsKeyPressed(ConsoleKey expectedKey)
        {
            int key = (int)expectedKey;
            var thatKey = _keyCurrentState[key];
            return thatKey.Pressed || thatKey.Held;
        }
    }
}
