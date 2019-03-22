using ConsoleRenderer.ConsoleScreens;
using System;

namespace ConsoleRenderer
{
    static class Program
    {
        static void Main(string[] args)
        {
            // Biggest size is (180, 60)
            //var screen = new NativeWindowsScreen(120, 40);
            var screen = new DefaultScreen(120, 40);
            var charMap = new CharMap("Map1.txt");
            var camera = new Camera(charMap);
            var mapRenderer = new MapRenderer(screen, camera, charMap);
            var frameTimer = new FrameTimer();

            const float Speed = 5.0f;

            while (true)
            {
                frameTimer.Update();

                if (Console.KeyAvailable)
                {
                    var pressedKey = Console.ReadKey(true);
                    if (pressedKey.Key == ConsoleKey.W)
                    {
                        camera.MoveForward(Speed, frameTimer.FrameTime);
                    }
                    else if (pressedKey.Key == ConsoleKey.S)
                    {
                        camera.MoveBackward(Speed, frameTimer.FrameTime);
                    }

                    if (pressedKey.Key == ConsoleKey.LeftArrow)
                    {
                        camera.TurnLeft(Speed, frameTimer.FrameTime);
                    }
                    else if (pressedKey.Key == ConsoleKey.RightArrow)
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
