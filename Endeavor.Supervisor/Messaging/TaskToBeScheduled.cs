using System;
using System.Collections.Generic;
using System.Text;

namespace Endeavor.Supervisor.Messaging
{
    [Serializable]
    public class TaskToBeScheduled
    {
        public long TaskId;
        public long StepTaskId;
        public int StepId;
        public string StepName;
        public int Status;
        public string StepType;

        public TaskToBeScheduled()
        {

        }

        public TaskToBeScheduled(Dictionary<string, object> properties)
        {
            foreach (string key in properties.Keys)
            {
                switch(key)
                {
                    case "ID":
                        StepTaskId = (long)properties[key];
                        break;
                    case "TaskID":
                        TaskId = (long)properties[key];
                        break;
                    case "StepID":
                        StepId = (int)properties[key];
                        break;
                    case "StatusID":
                        Status = (int)properties[key];
                        break;
                    case "StepType":
                        StepType = properties[key].ToString();
                        break;
                }
            }
        }
    }
}
