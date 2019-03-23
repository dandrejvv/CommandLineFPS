using ConsoleRenderer.ConsoleScreens;
using ConsoleRenderer.InputDevices;
using System;
using System.Runtime.InteropServices;

namespace ConsoleRenderer
{
    static class Program
    {
        static void Main(string[] args)
        {
            // Biggest size is (180, 60)
            const int CONSOLE_WIDTH = 120;
            const int CONSOLE_HEIGHT = 40;

            IConsoleScreen screen;
            IKeyboard keyboard;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                screen = new NativeWindowsScreen(CONSOLE_WIDTH, CONSOLE_HEIGHT);
                keyboard = new WindowsKeyboard();
            }
            else
            {
                screen = new DefaultScreen(CONSOLE_WIDTH, CONSOLE_HEIGHT);
                keyboard = new DefaultKeyboard();
            }

            var charMap = new CharMap("Map1.txt");
            var camera = new Camera(charMap);
            var mapRenderer = new MapRenderer(screen, camera, charMap);
            var frameTimer = new FrameTimer();

            const float Speed = 5.0f;

            while (true)
            {
                frameTimer.Update();

                if (keyboard.HasKeyPressed())
                {
                    if (keyboard.IsKeyPressed(ConsoleKey.W))
                    {
                        camera.MoveForward(Speed, frameTimer.FrameTime);
                    }
                    else if (keyboard.IsKeyPressed(ConsoleKey.S))
                    {
                        camera.MoveBackward(Speed, frameTimer.FrameTime);
                    }

                    if (keyboard.IsKeyPressed(ConsoleKey.LeftArrow))
                    {
                        camera.TurnLeft(Speed, frameTimer.FrameTime);
                    }
                    else if (keyboard.IsKeyPressed(ConsoleKey.RightArrow))
                    {
                        camera.TurnRight(Speed, frameTimer.FrameTime);
                    }
                }

                mapRenderer.Draw();

                charMap.DrawMap(screen, 1, 1, camera.GetCameraPosition());
                screen.Draw(1, 17, $"FPS: {frameTimer.Fps.ToString("0.00")}");
                screen.RenderToConsole();
            }
        }
    }
}
