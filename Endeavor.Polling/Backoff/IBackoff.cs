using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Endeavor.Polling.Backoff
{
    public interface IBackoff
    {
        void Wait();
        void Cancel();
        void Reset();
    }

    public class FibonacciBackoff : IBackoff, IDisposable
    {
        private int _previousWait;
        private int _wait;
        private int _minWait;
        private int _maxWait;
        private static ManualResetEvent _resetEvent = new ManualResetEvent(false);
        private readonly ILogger<FibonacciBackoff> _logger;

        public FibonacciBackoff(ILogger<FibonacciBackoff> logger)
        {
            _previousWait = 0;
            _wait = 1;
            _minWait = 1;
            _maxWait = 60;
            _logger = logger;
        }

        public void Wait()
        {
            _logger.LogDebug("Waiting " + _wait + " seconds");
            _resetEvent.WaitOne(TimeSpan.FromSeconds(_wait));

            if (_wait < _maxWait)
            {
                int currentWait = _wait;
                _wait = _previousWait + currentWait;
                _previousWait = currentWait;
            }
        }

        public void Cancel()
        {
            _logger.LogDebug("Stopping FibonacciBackoff");
            _resetEvent.Set();
            
        }

        public void Reset()
        {
            _logger.LogDebug("Resetting FibonacciBackoff");
            _wait = _minWait;
            _previousWait = 0;
        }

        public void Dispose()
        {
            _resetEvent.Close();
        }
    }
}
