using System;

namespace ConsoleRenderer
{
    public class FrameTimer
    {
        private DateTime _fromTime;
        private DateTime _toTime;

        public FrameTimer()
        {
            _fromTime = DateTime.Now;
        }

        public TimeSpan ElapsedTime { get; private set; }

        public void Update()
        {
            _toTime = DateTime.Now;
            ElapsedTime = _toTime - _fromTime;
            _fromTime = _toTime;
        }

        public float FramesPerSecond()
        {
            return (1.0f / ElapsedTime.Milliseconds) * 1000.0f;
        }
    }
}
