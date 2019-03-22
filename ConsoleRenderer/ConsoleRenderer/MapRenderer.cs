using ConsoleRenderer.ConsoleScreens;
using System;
using System.Collections.Generic;

namespace ConsoleRenderer
{
    public class MapRenderer
    {
        const float Default_FOV = MathF.PI / 4.0f;
        const float Default_Depth = 16.0f;

        private readonly IConsoleScreen _screen;
        private readonly Camera _camera;
        private readonly CharMap _charMap;

        public MapRenderer(IConsoleScreen screen, Camera camera, CharMap charMap)
        {
            _screen = screen;
            _camera = camera;
            _charMap = charMap;
        }

        public void Draw()
        {
            float charMapWidth = _charMap.Width;
            float charMapHeight = _charMap.Height;
            float screenWidthFloat = _screen.ScreenWidth;
            float screenHeightFloat = _screen.ScreenHeight;
            int screenHeightInt = _screen.ScreenHeight;
            int screenWidthInt = _screen.ScreenWidth;
            float cameraAngle = _camera.CameraAngle;
            float cameraX = _camera.CameraX;
            float cameraY = _camera.CameraY;

            for (var x = 0; x < screenWidthInt; x++)
            {
                float rayAngle = (cameraAngle - Default_FOV / 2.0f) + (x / screenWidthFloat) * Default_FOV;

                const float StepSize = 0.1f;
                float distanceToWall = 0.0f;

                bool hitWall = false;
                bool boundary = false;

                float eyeX = MathF.Sin(rayAngle);
                float eyeY = MathF.Cos(rayAngle);

                while (!hitWall && distanceToWall < Default_Depth)
                {
                    distanceToWall += StepSize;
                    int testX = (int)(cameraX + eyeX * distanceToWall);
                    int testY = (int)(cameraY + eyeY * distanceToWall);

                    if (testX < 0.0f || testX >= charMapWidth || testY < 0.0f || testY >= charMapHeight)
                    {
                        hitWall = true;
                        distanceToWall = Default_Depth;
                    }
                    else
                    {
                        if (_charMap.GetAtPos(testX, testY) == '#')
                        {
                            hitWall = true;

                            var boundaries = new List<BoundaryProduct>();

                            for (int tx = 0; tx < 2; tx++)
                            {
                                for (int ty = 0; ty < 2; ty++)
                                {
                                    float vy = (float)testY + ty - cameraY;
                                    float vx = (float)testX + tx - cameraX;
                                    float d = MathF.Sqrt(vx * vx + vy * vy);
                                    float dot = (eyeX * vx / d) + (eyeY * vy / d);
                                    boundaries.Add(new BoundaryProduct(d, dot));
                                }
                            }

                            boundaries.Sort((a, b) => (int)(a.Distance - b.Distance));

                            float bound = 0.01f;
                            if (MathF.Cos(boundaries[0].DotProduct) < bound) boundary = true;
                            if (MathF.Cos(boundaries[1].DotProduct) < bound) boundary = true;
                            if (MathF.Cos(boundaries[2].DotProduct) < bound) boundary = true;
                        }
                    }
                }

                int nCeiling = (int)(screenHeightFloat / 2.0f - screenHeightFloat / distanceToWall);
                int nFloor = screenHeightInt - nCeiling;

                char shadeChar = ' ';
                if (distanceToWall <= Default_Depth / 4.0f) shadeChar = (char)0x2588;
                else if (distanceToWall < Default_Depth / 3.0f) shadeChar = (char)0x2593;
                else if (distanceToWall < Default_Depth / 2.0f) shadeChar = (char)0x2592;
                else if (distanceToWall < Default_Depth) shadeChar = (char)0x2591;
                else shadeChar = ' ';

                if (boundary) shadeChar = ' ';

                for (int y = 0; y < screenHeightInt; y++)
                {
                    if (y <= nCeiling)
                    {
                        _screen.Draw(x, y, ' ');
                    }
                    else if (y > nCeiling && y <= nFloor)
                    {
                        _screen.Draw(x, y, shadeChar);
                    }
                    else
                    {
                        float b = 1.0f - ((y - screenHeightFloat / 2.0f) / (screenHeightFloat / 2.0f));
                        if (b < 0.25f) shadeChar = '#';
                        else if (b < 0.5f) shadeChar = 'x';
                        else if (b < 0.75f) shadeChar = '.';
                        else if (b < 0.9f) shadeChar = '-';
                        else shadeChar = ' ';

                        _screen.Draw(x, y, shadeChar);
                    }
                }
            }
        }

        private struct BoundaryProduct
        {
            public BoundaryProduct(float distance, float dotProduct)
            {
                Distance = distance;
                DotProduct = dotProduct;
            }

            public float Distance { get; }
            public float DotProduct { get; }
        }
    }
}
