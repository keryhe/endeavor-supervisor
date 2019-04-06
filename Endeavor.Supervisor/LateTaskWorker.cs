using Endeavor.Supervisor.Messaging;
using Endeavor.Supervisor.Persistence;
using Endeavor.Supervisor.Polling;
using Keryhe.Messaging;
using Keryhe.Polling;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Endeavor.Supervisor
{
    public class LateTaskWorker : BackgroundService
    {
        private readonly IPoller<TaskToBeScheduled> _poller;
        private readonly IDal _dal;
        private readonly ILogger<ReadyTaskWorker> _logger;

        public LateTaskWorker(Func<string, IPoller<TaskToBeScheduled>> pollAccessor, IDal dal, ILogger<ReadyTaskWorker> logger)
        {
            _poller = pollAccessor("late");
            _dal = dal;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Late Task Worker Started");

            await Task.Run(() =>
            {
                _poller.Start(Callback);

            });

            _logger.LogInformation("Late Task Worker Stopped");
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _poller.Stop();
            _poller.Dispose();
            await base.StopAsync(cancellationToken);
        }

        private void Callback(List<TaskToBeScheduled> messages)
        {
            List<long> tasks = new List<long>();
            foreach (TaskToBeScheduled message in messages)
            {
                tasks.Add(message.TaskId);
            }

            _dal.UpdateTasksStatuses(tasks, StatusType.Ready);
        }
    }
}
