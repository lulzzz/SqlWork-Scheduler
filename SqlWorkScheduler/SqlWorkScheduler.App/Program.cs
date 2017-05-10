using Akka.Actor;
using SqlWorkScheduler.App.Actors;
using SqlWorkScheduler.App.Messeges;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlWorkScheduler.App
{
    class Program
    {
        static void Main(string[] args)
        {
            var system = ActorSystem.Create("SqlWorkScheduler");
            var schedulerActor = system.ActorOf<WorkSchedulerActor>("WorkSchedulerActor");

            var connectionString = ConfigurationManager.ConnectionStrings["DefaultConnectionString"].ConnectionString;

            //schedulerActor.Tell(
            //    new ScheduleWorkCmd(
            //        Guid.NewGuid().ToString(),
            //        "select * from Orders where RequiredDate < {lastRun}",
            //        connectionString,
            //        5,
            //        "localhost:80/api/orders"
            //    ));

            //schedulerActor.
            //var cancel = system.Scheduler.ScheduleTellRepeatedlyCancelable(TimeSpan.FromSeconds(50), TimeSpan.FromSeconds(50), )

            Console.ReadLine();
        }
    }
}
