﻿namespace SqlWorkScheduler.App.Messeges
{
    public class WorkerIntiationCmd
    {
        public string Id { get; private set; }
        public string SqlQuery { get; private set; }
        public string EndPoint { get; private set; }
        public string SqlConnection { get; private set; }

        public WorkerIntiationCmd(string id,string sqlQuery, string sqlConnection, string endPoint)
        {
            Id = id; 
            SqlQuery = sqlQuery;
            SqlConnection = sqlConnection;
            EndPoint = endPoint;
        }
    }
}