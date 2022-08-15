using System;
using System.Threading;

namespace Raptor.Game.Shared
{
    public class Timer
    {
        private bool _stop;
        private readonly Thread _thread;
        private readonly DateTime _startedAt;
        private readonly TimeSpan _frequency;
        private readonly Action<double> _action;
        public int Tick { get; private set; }
        
        public Timer(Action<double> action, TimeSpan frequency)
        {
            _action = action;
            _frequency = frequency;
            _thread = new Thread(Loop);
            _startedAt = DateTime.Now;
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
                var sinceStart = DateTime.Now - _startedAt;
                var tick = sinceStart.TotalSeconds / _frequency.TotalSeconds;
                var tickInt = (int) tick;

                var nextTick = tickInt + 1;
                var nextTickTime = _startedAt + nextTick * _frequency;
                var timeUntilNextTick = nextTickTime - DateTime.Now;
                Thread.Sleep(timeUntilNextTick);

                while (tickInt > Tick)
                {
                    Tick++;
                    _action.Invoke(tick);
                }
            }
        }
    }
}