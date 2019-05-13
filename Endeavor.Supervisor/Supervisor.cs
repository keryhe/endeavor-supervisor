using Endeavor.Supervisor.Messaging;
using Endeavor.Supervisor.Persistence;
using Keryhe.Messaging;
using Keryhe.Polling;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Endeavor.Supervisor
{
    public class Supervisor : BackgroundService
    {
        private readonly IPoller<TaskToBeWorked> _poller;
        private readonly IDal _dal;
        private readonly IMessagePublisher<TaskToBeWorked> _publisher;
        private readonly ILogger<Supervisor> _logger;
        private ManualResetEvent _resetEvent = new ManualResetEvent(false);

        public Supervisor(IPoller<TaskToBeWorked> poller, IDal dal, IMessagePublisher<TaskToBeWorked> publisher, ILogger<Supervisor> logger)
        {
            _poller = poller;
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
                _resetEvent.WaitOne();
            });

            _logger.LogInformation("ReadyTaskWorker Stopped");
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _poller.Stop();
            _poller.Dispose();
            _publisher.Dispose();
            _resetEvent.Set();
            await base.StopAsync(cancellationToken);
        }

        private void Callback(List<TaskToBeWorked> messages)
        {
            foreach (TaskToBeWorked message in messages)
            {
                _dal.UpdateTaskStatus(message.TaskId, StatusType.Queued);

                _logger.LogDebug("Send Task to Worker");
                _publisher.Send(message);
            }

        }
    }
}