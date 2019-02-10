using Endeavor.Supervisor.Messaging;
using Endeavor.Supervisor.Persistence;
using Keryhe.Polling;
using Keryhe.Polling.Delay;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace Endeavor.Supervisor.Polling
{
    public class OvertimeTaskPoller : Poller<TaskToBeScheduled>
    {
        private readonly ILogger<OvertimeTaskPoller> _logger;
        private readonly IDal _dal;

        public OvertimeTaskPoller(IDelay delay, IDal dal, ILogger<OvertimeTaskPoller> logger)
            :base(delay, logger)
        {
            _logger = logger;
            _dal = dal;
        }

        protected override List<TaskToBeScheduled> Poll()
        {
            _logger.LogDebug("Polling for overtime tasks");

            List<TaskToBeScheduled> results = _dal.GetTasksInOvertime();

            return results;
        }
    }
}
