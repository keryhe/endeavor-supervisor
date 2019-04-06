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
    public class ReadyTaskWorker : BackgroundService
    {
        private readonly IPoller<TaskToBeScheduled> _poller;
        private readonly IDal _dal;
        private readonly IMessagePublisher<TaskToBeScheduled> _publisher;
        private readonly ILogger<ReadyTaskWorker> _logger;

        public ReadyTaskWorker(Func<string, IPoller<TaskToBeScheduled>> pollAccessor, IDal dal, IMessagePublisher<TaskToBeScheduled> publisher, ILogger<ReadyTaskWorker> logger)
        {
            _poller = pollAccessor("ready");
            _dal = dal;
            _publisher = publisher;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("ReadyTaskWorker Started");

            await Task.Run(() =>
            {
                _poller.Start(Callback);

            });

            _logger.LogInformation("ReadyTaskWorker Stopped");
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _poller.Stop();
            _poller.Dispose();
            _publisher.Dispose();
            await base.StopAsync(cancellationToken);
        }

        private void Callback(List<TaskToBeScheduled> messages)
        {
            foreach (TaskToBeScheduled message in messages)
            {
                _dal.UpdateTaskStatus(message.TaskId, StatusType.Queued);

                _logger.LogDebug("Send Task to Scheduler");
                _publisher.Send(message);
            }

        }
    }
}