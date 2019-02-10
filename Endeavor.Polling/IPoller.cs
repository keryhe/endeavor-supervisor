using Endeavor.Polling.Backoff;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace Endeavor.Polling
{
    public interface IPoller<T>
    {
        void Start(Action<List<T>> callback);
        void Stop();
    }

    public abstract class PollerBase<T> : IPoller<T>
    {
        private readonly IBackoff _backoff;
        private readonly ILogger<PollerBase<T>> _logger;
        private bool _status;

        public PollerBase(IBackoff backoff, ILogger<PollerBase<T>> logger)
        {
            _backoff = backoff;
            _logger = logger;
            _status = true;
        }

        public void Start(Action<List<T>> callback)
        {
            _logger.LogDebug("Polling started");
            while (_status)
            {
                List<T> items = Poll();

                if (items.Count == 0)
                {
                    _backoff.Wait();
                }
                else
                {
                    callback(items);
                    _backoff.Reset();
                }
            }
        }

        public void Stop()
        {
            _status = false;
            _backoff.Cancel();
            _logger.LogDebug("Polling stopped");
        }

        protected abstract List<T> Poll();
    }
}
