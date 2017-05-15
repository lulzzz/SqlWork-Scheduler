using SqlWorkScheduler.App.Actors;
using SqlWorkScheduler.App.Contracts;
using System.Collections.Generic;

namespace SqlWorkScheduler.App.Messeges
{
    public class ScheduleWorkCmd
    {
        public string Id { get; private set; }
        public string SqlQuery { get; private set; }
        public string SqlConnection { get; private set; }
        public int Interval { get; private set; }
        public string EndPoint { get; private set; }
        public SpParameter[] SpParameters { get; set; }
        public bool SaveToDisk { get; private set; }
        public long LastRun { get; private set; }

        public ScheduleWorkCmd(string id, string sqlQuery, string sqlConnection, int interval, string endpoint, SpParameter[] spParameters = null, long lastRun = 0, bool saveToDisk = true)
        {
            Id = id;
            SqlQuery = sqlQuery;
            SqlConnection = sqlConnection;
            Interval = interval;
            EndPoint = endpoint;
            SaveToDisk = saveToDisk;
            LastRun = lastRun;
            SpParameters = spParameters;
        }
    }
}
