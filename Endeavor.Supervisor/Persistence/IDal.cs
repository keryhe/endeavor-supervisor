using Endeavor.Supervisor.Messaging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Endeavor.Supervisor.Persistence
{
    public interface IDal
    {
        List<TaskToBeScheduled> GetTasksByStatus(StatusType status);
        List<TaskToBeScheduled> GetTasksInOvertime();


    }

    public enum StatusType
    {
        Ready = 1,
        Processing,
        Waiting,
        Complete,
        Error
    }
}
