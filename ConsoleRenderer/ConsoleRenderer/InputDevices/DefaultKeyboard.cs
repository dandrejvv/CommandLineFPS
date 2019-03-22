using System;

namespace ConsoleRenderer.InputDevices
{
    public class DefaultKeyboard : IKeyboard
    {
        public bool HasKeyPressed()
        {
            return Console.KeyAvailable;
        }

        public bool IsKeyPressed(ConsoleKey expectedKey)
        {
            return Console.ReadKey().Key == expectedKey;
        }
    }
}
