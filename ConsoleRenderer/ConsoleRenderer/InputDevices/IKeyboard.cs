using System;

namespace ConsoleRenderer.InputDevices
{
    public interface IKeyboard
    {
        bool HasKeyPressed();
        bool IsKeyPressed(ConsoleKey expectedKey);
    }
}
