using ConsoleRenderer.ConsoleScreens;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace ConsoleRenderer
{
    public class Camera
    {
        private readonly CharMap _charMap;

        private float _cameraX;
        private float _cameraY;
        private float _cameraAngle;

        public Camera(CharMap charMap)
        {
            _charMap = charMap;
            CameraX = _charMap.PlayerStartingPosition.PosX;
            CameraY = _charMap.PlayerStartingPosition.PosY;
        }

        public float CameraX { get => _cameraX; private set => _cameraX = value; }
        public float CameraY { get => _cameraY; private set => _cameraY = value; }
        public float CameraAngle { get => _cameraAngle; private set => _cameraAngle = value; }

        public PositionInt2D GetCameraPosition()
        {
            return new PositionInt2D((int)CameraX, (int)CameraY);
        }

        public void MoveForward(float amount, float frameElapsed)
        {
            var testX = CameraX + MathF.Sin(CameraAngle) * amount * frameElapsed;
            var testY = CameraY + MathF.Cos(CameraAngle) * amount * frameElapsed;
            if (_charMap.GetAtPos((int)testX, (int)testY) != '#')
            {
                CameraX = testX;
                CameraY = testY;
            }
        }

        public void MoveBackward(float amount, float frameElapsed)
        {
            var testX = CameraX - MathF.Sin(CameraAngle) * amount * frameElapsed;
            var testY = CameraY - MathF.Cos(CameraAngle) * amount * frameElapsed;
            if (_charMap.GetAtPos((int)testX, (int)testY) != '#')
            {
                CameraX = testX;
                CameraY = testY;
            }
        }

        public void TurnLeft(float amount, float frameElapsed)
        {
            CameraAngle -= (amount * 0.75f) * frameElapsed;
        }

        public void TurnRight(float amount, float frameElapsed)
        {
            CameraAngle += (amount * 0.75f) * frameElapsed;
        }
    }
}
