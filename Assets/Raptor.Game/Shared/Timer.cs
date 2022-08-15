using System;
using System.Threading;
using UnityEngine;
using UnityEngine.Assertions;

namespace Raptor.Game.Shared
{
    public class Timer
    {
        private bool _stop;
        private readonly Thread _thread;
        private readonly TimeSpan _frequency;
        private readonly Func<DateTime> _timeNowSource;
        private readonly Action<double> _action;
        public DateTime StartedAt { get; }
        public int Tick { get; private set; }

        public Timer(Action<double> action, TimeSpan frequency, Func<DateTime> timeNowSource,
            DateTime? startedAtOverride = null)
        {
            _action = action;
            _frequency = frequency;
            _timeNowSource = timeNowSource;
            _thread = new Thread(Loop);
            StartedAt = startedAtOverride.HasValue ? startedAtOverride.Value : _timeNowSource.Invoke();
            _thread.Start();
        }

        public void Stop()
        {
            _stop = true;
            _thread.Abort();
            _thread.Interrupt();
        }

        private void Loop()
        {
            while (!_stop)
            {
                var sinceStart = _timeNowSource.Invoke() - StartedAt;
                var tick = sinceStart.TotalSeconds / _frequency.TotalSeconds;
                var tickInt = (int) tick;

                var nextTick = tickInt + 1;
                var nextTickTime = StartedAt + nextTick * _frequency;
                var timeUntilNextTick = nextTickTime - _timeNowSource.Invoke();

                if (timeUntilNextTick > TimeSpan.Zero)
                {
                    Thread.Sleep(timeUntilNextTick);
                }

                while (tickInt > Tick)
                {
                    Tick++;
                    _action.Invoke(tick);
                }
            }
        }
    }
}