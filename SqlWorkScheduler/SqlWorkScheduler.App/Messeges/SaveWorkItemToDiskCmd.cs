namespace SqlWorkScheduler.App.Messeges
{
    public class SaveWorkItemToDiskCmd
    {
        public string Id { get; private set; }
        public string SqlQuery { get; private set; }
        public string SqlConnection { get; private set; }
        public int Interval { get; private set; }
        public string EndPoint { get; private set; }

        public SaveWorkItemToDiskCmd(string id, string sqlQuery, string sqlConnection, int interval, string endpoint)
        {
            Id = id;
            SqlQuery = sqlQuery;
            SqlConnection = sqlConnection;
            Interval = interval;
            EndPoint = endpoint;
        }
    }
}