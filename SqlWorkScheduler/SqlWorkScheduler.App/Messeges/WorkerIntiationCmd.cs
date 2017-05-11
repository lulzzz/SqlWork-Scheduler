namespace SqlWorkScheduler.App.Messeges
{
    public class WorkerIntiationCmd
    {
        public string Id { get; private set; }
        public string SqlQuery { get; private set; }
        public string SqlConnection { get; private set; }
        public long LastRun { get; private set; }
        public string EndPoint { get; private set; }

        public WorkerIntiationCmd(string id, string sqlQuery, string sqlConnection, string endPoint, long lastRun = 0)
        {
            Id = id;
            SqlQuery = sqlQuery;
            SqlConnection = sqlConnection;
            LastRun = lastRun;
            EndPoint = endPoint;
        }
    }
}