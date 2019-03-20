using System;

namespace ConsoleRenderer
{
    public class FrameTimer
    {
        private DateTime _fromTime;
        private DateTime _toTime;
        private long _millisecondsPassed;
        private float _prevFps;
        private long _frameTime;

        public FrameTimer()
        {
            _fromTime = DateTime.Now;
        }

        public long FrameTime { get { return _frameTime; } }
        public float Fps { get { return _prevFps; } }

        public void Update()
        {
            if (_millisecondsPassed >= 1000)
            {
                _prevFps = (1.0f / FrameTime) * 1000.0f;
                _millisecondsPassed = 0;
            }

            _toTime = DateTime.Now;
            var elapsed = _toTime - _fromTime;
            _frameTime = elapsed.Milliseconds;
            _fromTime = _toTime;
            _millisecondsPassed += FrameTime;
        }
    }
}
