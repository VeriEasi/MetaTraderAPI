using System;
using System.Threading;

namespace MTAPIService
{
    public class MTCommandTask
    {
        private readonly EventWaitHandle _responseWaiter = new AutoResetEvent(false);
        private MTResponse _result;
        private readonly object _locker = new object();

        public MTCommandTask(MTCommand command)
        {
            Command = command ?? throw new ArgumentNullException(nameof(command));
        }

        public MTCommand Command { get; }

        public MTResponse WaitResult(int time)
        {
            _responseWaiter.WaitOne(time);
            lock (_locker)
            {
                return _result;
            }
        }

        public void SetResult(MTResponse result)
        {
            lock (_locker)
            {
                _result = result;
            }
            _responseWaiter.Set();
        }

        public override string ToString()
        {
            return $"Command = {Command}";
        }
    }
}