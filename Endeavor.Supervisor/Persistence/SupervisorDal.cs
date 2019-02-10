﻿using System;
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

        public List<TaskToBeScheduled> GetTasksByStatus(StatusType status)
        {
            List<TaskToBeScheduled> results = new List<TaskToBeScheduled>();
            /*
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@StatusID", (int)status);

            List<Dictionary<string, object>> tasks = Query("GetStepTasksByStatus", CommandType.StoredProcedure, parameters);

            foreach (Dictionary<string, object> task in tasks)
            {
                results.Add(new TaskToBeScheduled(task));
            }
            */
            return results;
        }

        public List<TaskToBeScheduled> GetTasksInOvertime()
        {
            List<TaskToBeScheduled> results = new List<TaskToBeScheduled>();
            /*
            List<Dictionary<string, object>> tasks = Query("GetStepTasksByStatus", CommandType.StoredProcedure, null);

            foreach (Dictionary<string, object> task in tasks)
            {
                results.Add(new TaskToBeScheduled(task));
            }
            */
            return results;
        }

        private List<Dictionary<string, object>> Query(string name, System.Data.CommandType commandType, Dictionary<string, object> parameters)
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