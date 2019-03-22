using System;

namespace ConsoleRenderer
{
    public class FrameTimer
    {
        private DateTime _fromTime;
        private DateTime _toTime;
        private long _millisecondsPassed;
        private float _prevFps;
        private long _internalFrameTime;
        private float _floatFrameTime;

        public FrameTimer()
        {
            _fromTime = DateTime.Now;
        }

        public float  FrameTime { get { return _floatFrameTime; } }
        public float Fps { get { return _prevFps; } }

        public void Update()
        {
            if (_millisecondsPassed >= 1000)
            {
                _prevFps = 1.0f / _floatFrameTime;
                _millisecondsPassed = 0;
            }

            _toTime = DateTime.Now;
            var elapsed = _toTime - _fromTime;
            _internalFrameTime = elapsed.Milliseconds;
            _fromTime = _toTime;
            _millisecondsPassed += _internalFrameTime;
            _floatFrameTime = _internalFrameTime / 1000.0f;
        }
    }
}
