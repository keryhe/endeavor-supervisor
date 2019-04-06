using Endeavor.Supervisor.Messaging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Endeavor.Supervisor.Persistence
{
    public interface IDal
    {
        List<TaskToBeScheduled> GetTasksByStatus(StatusType status);
        List<TaskToBeScheduled> GetLateTasks();
        void UpdateTaskStatus(long taskID, StatusType status);
        void UpdateTasksStatuses(List<long> taskIDs, StatusType status);
    }

    public enum StatusType
    {
        Ready,
        Queued,
        Processing,
        Waiting,
        Complete,
        Error
    }
}
