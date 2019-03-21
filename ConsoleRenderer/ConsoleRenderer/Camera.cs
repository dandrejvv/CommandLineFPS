using System;
using System.Collections.Generic;
using System.Globalization;

namespace ConsoleRenderer
{
    public class Camera
    {
        private readonly IScreen _screen;
        private readonly CharMap _charMap;

        const float Default_FOV = MathF.PI / 4.0f;  // Field of View
        const float Default_Depth = 16.0f;          // Maximum rendering distance

        private float _cameraX;
        private float _cameraY;
        private float _cameraAngle;

        public Camera(IScreen screen, CharMap charMap)
        {
            _screen = screen;
            _charMap = charMap;

            _cameraX = _charMap.PlayerStartingPosition.PosX;
            _cameraY = _charMap.PlayerStartingPosition.PosY;
        }

        public PositionInt2D GetCameraPosition()
        {
            return new PositionInt2D((int)_cameraX, (int)_cameraY);
        }

        public void MoveForward(float amount, float frameElapsed)
        {
            _cameraX += MathF.Sin(_cameraAngle) * amount * frameElapsed;
            _cameraY += MathF.Cos(_cameraAngle) * amount * frameElapsed;
            if (_charMap.GetAtPos((int)_cameraX, (int)_cameraY) == '#')
            {
                _cameraX -= MathF.Sin(_cameraAngle) * amount * frameElapsed;
                _cameraY -= MathF.Cos(_cameraAngle) * amount * frameElapsed;
            }
        }

        public void MoveBackward(float amount, float frameElapsed)
        {
            _cameraX -= MathF.Sin(_cameraAngle) * amount * frameElapsed;
            _cameraY -= MathF.Cos(_cameraAngle) * amount * frameElapsed;
            if (_charMap.GetAtPos((int)_cameraX, (int)_cameraY) == '#')
            {
                _cameraX += MathF.Sin(_cameraAngle) * amount * frameElapsed;
                _cameraY += MathF.Cos(_cameraAngle) * amount * frameElapsed;
            }
        }

        public void TurnLeft(float amount, float frameElapsed)
        {
            _cameraAngle -= (amount * 0.75f) * frameElapsed;
        }

        public void TurnRight(float amount, float frameElapsed)
        {
            _cameraAngle += (amount * 0.75f) * frameElapsed;
        }

        public void Draw()
        {
            float charMapWidth = _charMap.Width;
            float charMapHeight = _charMap.Height;
            float screenWidth = _screen.ScreenWidth;
            float screenHeight = _screen.ScreenHeight;
            int screenHeightInt = _screen.ScreenHeight;
            int screenWidthInt = _screen.ScreenWidth;

            for (var x = 0; x < screenWidthInt; x++)
            {
                float rayAngle = (_cameraAngle - Default_FOV / 2.0f) + (x / screenWidth) * Default_FOV;
                
                const float StepSize = 0.1f;
                float distanceToWall = 0.0f;

                bool hitWall = false;
                bool boundary = false;

                float eyeX = MathF.Sin(rayAngle);
                float eyeY = MathF.Cos(rayAngle);

                while (!hitWall && distanceToWall < Default_Depth)
                {
                    distanceToWall += StepSize;
                    int testX = (int)(_cameraX + eyeX * distanceToWall);
                    int testY = (int)(_cameraY + eyeY * distanceToWall);

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
                                    float vy = (float)testY + ty - _cameraY;
                                    float vx = (float)testX + tx - _cameraX;
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

                int nCeiling = (int)(screenHeight / 2.0f - screenHeight / distanceToWall);
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
                        _screen.Draw(x, y, ' ');
                    else if (y > nCeiling && y <= nFloor)
                        _screen.Draw(x, y, shadeChar);
                    else 
                    {
                        float b = 1.0f - ((y - screenHeight / 2.0f) / (screenHeight / 2.0f));
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
