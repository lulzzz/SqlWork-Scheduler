using Akka.Actor;
using ProtoBuf.Data;
using SqlWorkScheduler.App.Messeges;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SqlWorkScheduler.App.Actors
{
    class WorkPerformerActor : ReceiveActor
    {
        private string _lastRunReplacement { get { return ConfigurationManager.AppSettings["LastRunReplacement"]; } }

        private DateTime _lastRun;
        private string _id;
        private string _sqlQuery;
        private string _endPoint;
        private string _sqlConnection;
        private int _interval;

        public WorkPerformerActor()
        {
            NotIntiated();
        }

        #region states
        private void NotIntiated()
        {
            Receive<WorkerIntiationCmd>(cmd => ReceiveWorkerIntiationCmd(cmd));
        }

        private void Intiated()
        {
            Receive<PerformWorkCmd>(cmd => ReceivePerformWorkCmd(cmd));
        }

        #endregion

        private void ReceiveWorkerIntiationCmd(WorkerIntiationCmd cmd)
        {
            _id = cmd.Id;
            _sqlQuery = cmd.SqlQuery;
            _endPoint = cmd.EndPoint;
            _sqlConnection = cmd.SqlConnection;
            _interval = cmd.Interval;

            var filePath = string.Format("./ScheduledWork/{0}.txt", _id);

            try
            {
                if (File.Exists(filePath))
                {
                    using (var file = File.Open(filePath, FileMode.Open, FileAccess.Read))
                    {
                        string contents;
                        using (var sr = new StreamReader(file))
                        {
                            contents = sr.ReadToEnd();
                        }

                        var arr = contents.Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

                        if (arr.Length == 5)
                        {
                            if (!string.IsNullOrEmpty(arr[4]))
                            {
                                _lastRun = DateTime.Parse(arr[4]);
                            }
                            else
                            {
                                _lastRun = DateTime.MinValue.AddYears(10);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: {0}", e.Message);
            }

            Become(Intiated);
        }

        private void ReceivePerformWorkCmd(PerformWorkCmd cmd)
        {
            try
            {
                using (var sqlClient = new SqlConnection(_sqlConnection))
                {
                    sqlClient.Open();

                    var sqlQuery = _sqlQuery.Replace(_lastRunReplacement, "'" + _lastRun.ToString("yyyy-MM-dd h:mm:ss") + "'");
                    using (var sqlCommand = new SqlCommand(sqlQuery, sqlClient))
                    {
                        var sqlDataReader = sqlCommand.ExecuteReader();

                        // web request
                        var request = WebRequest.CreateHttp(_endPoint);
                        request.Method = "POST";

                        using (var webStream = request.GetRequestStream())
                        {
                            using (var protoStream = new ProtoDataStream(sqlDataReader))
                            {
                                protoStream.CopyTo(webStream);
                                protoStream.Close();
                            }
                            webStream.Close();
                        }

                        sqlDataReader.Close();
                    }

                    sqlClient.Close();
                }
                _lastRun = DateTime.Now;
                var filePath = string.Format("./ScheduledWork/{0}.txt", _id);
                using (var file = File.Open(filePath, FileMode.OpenOrCreate, FileAccess.Write))
                {
                    var fileContents = string.Format("{0}\r\n{1}\r\n{2}\r\n{3}\r\n{4}",
                            _sqlQuery,
                            _sqlConnection,
                            _interval,
                            _endPoint,
                            _lastRun.ToString("yyyy-MM-dd h:mm:ss")
                        );
                    var bytes = Encoding.ASCII.GetBytes(fileContents);
                    file.Write(bytes, 0, bytes.Length);
                    file.Close();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: {0}", e.Message);
            }


        }
    }
}
