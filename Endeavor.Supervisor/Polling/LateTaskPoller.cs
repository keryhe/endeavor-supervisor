using Endeavor.Supervisor.Messaging;
using Endeavor.Supervisor.Persistence;
using Keryhe.Polling;
using Keryhe.Polling.Delay;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace Endeavor.Supervisor.Polling
{
    public class LateTaskPoller : Poller<TaskToBeScheduled>
    {
        private readonly ILogger<LateTaskPoller> _logger;
        private readonly IDal _dal;

        public LateTaskPoller(IDelay delay, IDal dal, ILogger<LateTaskPoller> logger)
            :base(delay, logger)
        {
            _logger = logger;
            _dal = dal;
        }

        protected override List<TaskToBeScheduled> Poll()
        {
            _logger.LogDebug("Polling for late tasks");

            List<TaskToBeScheduled> results = _dal.GetLateTasks();

            return results;
        }
    }
}
