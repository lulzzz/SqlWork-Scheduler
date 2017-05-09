namespace SqlWorkScheduler.App.Messeges
{
    public class WorkerIntiationCmd
    {
        public string SqlQuery { get; private set; }
        public string EndPoint { get; private set; }

        public WorkerIntiationCmd(string sqlQuery, string endPoint)
        {
            SqlQuery = sqlQuery;
            EndPoint = endPoint;
        }
    }
}