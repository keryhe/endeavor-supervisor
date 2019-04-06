using System;
using System.Collections.Generic;
using System.Text;

namespace Endeavor.Supervisor.Messaging
{
    [Serializable]
    public class TaskToBeScheduled
    {
        public long TaskId;
        public int StepId;
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
                        TaskId = (long)properties[key];
                        break;
                    case "StepID":
                        StepId = (int)properties[key];
                        break;
                    case "StepType":
                        StepType = properties[key].ToString();
                        break;
                }
            }
        }
    }
}
