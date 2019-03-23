using System;

namespace ConsoleRenderer.InputDevices
{
    public class DefaultKeyboard : IKeyboard
    {
        public bool HasKeyPressed()
        {
            _consoleKeyPressed = null;
            return Console.KeyAvailable;
        }

        private ConsoleKey? _consoleKeyPressed;
        private ConsoleKey ConsoleKeyPressed
        {
            get
            {
                if (_consoleKeyPressed == null)
                {
                    _consoleKeyPressed = Console.ReadKey(true).Key;
                }
                return _consoleKeyPressed.Value;
            }
        }

        public bool IsKeyPressed(ConsoleKey expectedKey)
        {
            return ConsoleKeyPressed == expectedKey;
        }
    }
}
