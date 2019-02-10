using Endeavor.Supervisor.Messaging;
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
    public class Worker : BackgroundService
    {
        private readonly IPoller<TaskToBeScheduled> _poller;
        private readonly IMessagePublisher<TaskToBeScheduled> _publisher;
        private readonly ILogger<Worker> _logger;

        public Worker(IPoller<TaskToBeScheduled> poller, IMessagePublisher<TaskToBeScheduled> publisher, ILogger<Worker> logger)
        {
            _poller = poller;
            _publisher = publisher;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Worker Started");

            await Task.Run(() =>
            {
                _poller.Start(Callback);

            });

            _logger.LogInformation("Worker Stopped");
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
                _logger.LogDebug("publish message");
                _publisher.Send(message);
            }

        }
    }

    public class ReadyTaskWorker : Worker
    {
        public ReadyTaskWorker(Func<TaskPoller, IPoller<TaskToBeScheduled>> pollAccessor, IMessagePublisher<TaskToBeScheduled> publisher, ILogger<Worker> logger)
            : base(pollAccessor(TaskPoller.Ready), publisher, logger)
        {
        }
    }

    public class OvertimeTaskWorker : Worker
    {
        public OvertimeTaskWorker(Func<TaskPoller, IPoller<TaskToBeScheduled>> pollAccessor, IMessagePublisher<TaskToBeScheduled> publisher, ILogger<Worker> logger)
            : base(pollAccessor(TaskPoller.Overtime), publisher, logger)
        {
        }
    }
}