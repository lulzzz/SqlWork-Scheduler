using Akka.Actor;
using SqlWorkScheduler.App.Actors;
using System;
using System.Collections.Generic;
using System.Configuration;

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

            var parameters = new Dictionary<string, string>() {
                        { "@CategoryName", "Beverages" },
                        { "@OrdYear", "1998" }
                    };

            //StaticActors.SchedulerActor.Tell(
            //    new ScheduleWorkCmd(
            //        Guid.NewGuid().ToString(),
            //        "SalesByCategory",
            //        connectionString,
            //        5,
            //        "http://localhost:3550/",
            //        parameters
            //    ));

            //StaticActors.SchedulerActor.Tell(
            //    new ScheduleWorkCmd(
            //        Guid.NewGuid().ToString(),
            //        "select * from Orders",
            //        connectionString,
            //        1,
            //        "http://localhost:3550/"
            //    ));

            Console.ReadLine();
        }
    }
}
