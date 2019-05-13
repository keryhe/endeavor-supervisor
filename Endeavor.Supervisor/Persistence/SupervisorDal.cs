using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Endeavor.Supervisor.Messaging;
using Keryhe.Persistence;

namespace Endeavor.Supervisor.Persistence
{
    public class SupervisorDal : IDal
    {
        private readonly IPersistenceProvider _provider;

        public SupervisorDal(IPersistenceProvider provider)
        {
            _provider = provider;
        }

        public List<TaskToBeWorked> GetTasksByStatus(StatusType status)
        {
            List<TaskToBeWorked> results = new List<TaskToBeWorked>();

            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "@Status", (int)status }
            };

            List<Dictionary<string, object>> tasks = Query("GetTasksByStatus", CommandType.StoredProcedure, parameters);

            foreach (Dictionary<string, object> task in tasks)
            {
                results.Add(new TaskToBeWorked(task));
            }
            
            return results;
        }

        public void UpdateTaskStatus(long taskID, StatusType status)
        {
            int statusValue = (int)status;
            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE Task SET StatusValue = ");
            sb.Append(statusValue.ToString());
            sb.Append(" WHERE ID = ");
            sb.Append(taskID.ToString());

            _provider.ExecuteNonQuery(sb.ToString(), CommandType.Text);
        }

        public void UpdateTasksStatuses(List<long> taskIDs, StatusType status)
        {
            string tasks = string.Join(",", taskIDs);

            int statusValue = (int)status;
            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE Task SET StatusValue = ");
            sb.Append(statusValue.ToString());
            sb.Append(" WHERE ID in (");
            sb.Append(tasks);
            sb.Append(")");

            _provider.ExecuteNonQuery(sb.ToString(), CommandType.Text);
        }

        private List<Dictionary<string, object>> Query(string name, CommandType commandType, Dictionary<string, object> parameters)
        {
            List<Dictionary<string, object>> results = new List<Dictionary<string, object>>();

            using (var reader = _provider.ExecuteQuery(name, commandType, parameters))
            {
                while (reader.Read())
                {
                    Dictionary<string, object> result = new Dictionary<string, object>();

                    var columns = Enumerable.Range(0, reader.FieldCount).Select(reader.GetName).ToList();

                    foreach (var column in columns)
                    {
                        object columnValue = reader[column];
                        if (columnValue == DBNull.Value)
                        {
                            result.Add(column, null);
                        }
                        else
                        {
                            result.Add(column, columnValue);
                        }
                    }
                    results.Add(result);
                }
            }
            return results;
        }
    }
}
