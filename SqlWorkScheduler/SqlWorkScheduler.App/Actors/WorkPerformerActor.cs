using Akka.Actor;
using SqlWorkScheduler.App.Messeges;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlWorkScheduler.App.Actors
{
    class WorkPerformerActor : ReceiveActor
    {
        private DateTime _lastRun;
        private string _sqlQuery;
        private string _endPoint;


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
            _sqlQuery = cmd.SqlQuery;
            _endPoint = cmd.EndPoint;

            Become(Intiated);
        }

        private void ReceivePerformWorkCmd(PerformWorkCmd cmd)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["DefaultConnectionString"].ConnectionString;
            try
            {
                using (var sqlClient = new SqlConnection(connectionString))
                {
                    sqlClient.Open();

                    var sqlCommand = new SqlCommand(_sqlQuery, sqlClient);
                    var sqlDataReader = sqlCommand.ExecuteReader();

                    while(sqlDataReader.Read())
                    {
                        Console.WriteLine(sqlDataReader["OrderId"].ToString() + ": " + sqlDataReader["ShipCountry"].ToString());
                    }

                    sqlClient.Close();
                }

                _lastRun = DateTime.Now;
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: {0}", e.Message);
            }

            Console.WriteLine("Test 1");
        }
    }
}
