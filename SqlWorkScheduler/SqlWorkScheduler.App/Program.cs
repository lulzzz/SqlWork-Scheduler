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
    public class StaticActors
    {
        public static IActorRef SchedulerActor { get; set; }
        public static IActorRef SaveToDiskActor { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var system = ActorSystem.Create("SqlWorkScheduler");
            StaticActors.SchedulerActor = system.ActorOf<WorkSchedulerActor>("WorkSchedulerActor");
            StaticActors.SaveToDiskActor = system.ActorOf<SaveToDiskActor>("SaveToDiskActor");

            var connectionString = ConfigurationManager.ConnectionStrings["DefaultConnectionString"].ConnectionString;

            //StaticActors.SchedulerActor.Tell(
            //    new ScheduleWorkCmd(
            //        Guid.NewGuid().ToString(),
            //        "select * from Orders where RequiredDate < {lastRun}",
            //        connectionString,
            //        5,
            //        "http://localhost:3550/"
            //    ));

            //schedulerActor.
            //var cancel = system.Scheduler.ScheduleTellRepeatedlyCancelable(TimeSpan.FromSeconds(50), TimeSpan.FromSeconds(50), )

            Console.ReadLine();
        }
    }
}
