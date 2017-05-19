using Akka.Actor;
using ProtoBuf.Data;
using SqlWorkScheduler.App.Messeges;
using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.IO;
using System.Net;
using System.Text;

namespace SqlWorkScheduler.App.Actors
{
    class WorkPerformerActor : ReceiveActor
    {
        private string _lastRunReplacement { get { return ConfigurationManager.AppSettings["LastRunReplacement"]; } }

        private long _lastRun;
        private ScheduleWorkCmd _cmd;

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
            _lastRun = cmd.ScheduleCommand.LastRun;
            _cmd = cmd.ScheduleCommand;
            Become(Intiated);
        }

        private void ReceivePerformWorkCmd(PerformWorkCmd cmd)
        {
            try
            {
                using (var sqlClient = new SqlConnection(_cmd.SqlConnection))
                {
                    sqlClient.Open();

                    string sqlQuery;
                    if (_cmd.LastRun == 0)
                    {
                        sqlQuery = _cmd.SqlQuery.Replace(_lastRunReplacement, "'" + SqlDateTime.MinValue.ToSqlString().ToString() + "'");
                    }
                    else
                    {
                        var temp = new DateTime().AddTicks(_lastRun);
                        var date = "'" + new SqlDateTime(temp).ToSqlString().ToString() + "'";

                        sqlQuery = _cmd.SqlQuery.Replace(_lastRunReplacement, date);
                    }


                    using (var sqlCommand = new SqlCommand(sqlQuery, sqlClient))
                    {
                        if (_cmd.SpParameters != null)
                        {
                            if (_cmd.SpParameters.Length > 0)
                            {
                                sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;

                                foreach (var parameter in _cmd.SpParameters)
                                {
                                    sqlCommand.Parameters.Add(new SqlParameter(parameter.ParameterName, parameter.ParameterValue));
                                }
                            }
                        }

                        // web request
                        var request = WebRequest.CreateHttp(_cmd.EndPoint);
                        request.Method = "POST";

                        using (var reader = sqlCommand.ExecuteReader())
                        {
                            using (var webStream = request.GetRequestStream())
                            {
                                DataSerializer.Serialize(webStream, reader);
                                webStream.Close();
                                var response = request.GetResponse();
                            }

                            reader.Close();
                        }
                    }

                    sqlClient.Close();
                }

                _lastRun = DateTime.Now.Ticks;
                StaticActors.SaveToDiskActor
                    .Tell(new UpdateLastRunTickCmd(_cmd.Id, _lastRun));
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: {0}", e.Message);
            }
        }
    }
}
