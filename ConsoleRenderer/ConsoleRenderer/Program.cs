using ConsoleRenderer.ConsoleScreens;
using System;

namespace ConsoleRenderer
{
    class Program
    {
        static void Main(string[] args)
        {
            //var screen = new NativeWindowsScreen(120, 40);
            var screen = new DefaultScreen(120, 40);
            var charMap = new CharMap("Map1.txt");
            var camera = new Camera(screen, charMap);
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
                        camera.MoveForward(Speed, frameTimer.FrameTime / 1000.0f);
                    }
                    else if (pressedKey.Key == ConsoleKey.S)
                    {
                        camera.MoveBackward(Speed, frameTimer.FrameTime / 1000.0f);
                    }

                    if (pressedKey.Key == ConsoleKey.LeftArrow)
                    {
                        camera.TurnLeft(Speed, frameTimer.FrameTime / 1000.0f);
                    }
                    else if (pressedKey.Key == ConsoleKey.RightArrow)
                    {
                        camera.TurnRight(Speed, frameTimer.FrameTime / 1000.0f);
                    }
                }

                camera.Draw();

                charMap.DrawMap(screen, 1, 1, camera.GetCameraPosition());
                screen.Draw(1, 17, $"FPS: {frameTimer.Fps.ToString("0.00")}");
                screen.RenderToScreen();
            }
            
        }
    }
}
