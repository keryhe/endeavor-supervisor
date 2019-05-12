using Endeavor.Supervisor.Messaging;
using System.Collections.Generic;

namespace Endeavor.Supervisor.Persistence
{
    public interface IDal
    {
        List<TaskToBeWorked> GetTasksByStatus(StatusType status);
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
