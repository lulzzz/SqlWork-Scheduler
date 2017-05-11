using Akka.Actor;
using SqlWorkScheduler.App.Messeges;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlWorkScheduler.App.Actors
{
    class WorkDescription
    {
        public ScheduleWorkCmd Command { get; set; }
        public IActorRef Actor { get; set; }
        public ICancelable CancelObject { get; set; }
    }

    public class WorkSchedulerActor : ReceiveActor
    {
        private Dictionary<string, WorkDescription> _scheduledWork;

        protected override void PreStart()
        {
            _scheduledWork = new Dictionary<string, WorkDescription>();

            //try
            //{
            //    // Find all scheduled work items saved to disk
            //    foreach (var filePath in Directory.EnumerateFiles("./ScheduledWork", "*.txt"))
            //    {
            //        using (var file = File.Open(filePath, FileMode.Open, FileAccess.Read))
            //        {
            //            var id = Path.GetFileName(filePath).Replace(".txt", "");
            //            string contents;
            //            using (var sr = new StreamReader(file))
            //            {
            //                contents = sr.ReadToEnd();
            //            }

            //            var arr = contents.Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToList();
            //            var message = new ScheduleWorkCmd(id, arr[0], arr[1], Convert.ToInt32(arr[2]), arr[3], false);

            //            Self.Tell(message);
            //        }
            //    }
            //}
            //catch (Exception e)
            //{
            //    Console.WriteLine("Error: {0}", e.Message);
            //}

            base.PreStart();
        }

        public WorkSchedulerActor()
        {
            Receive<ScheduleWorkCmd>(cmd => ReceiveScheduleWorkCmd(cmd));
            Receive<CancelScheduledWorkCmd>(cmd => ReceiveCancelScheduledWorkCmd(cmd));
        }

        private void ReceiveScheduleWorkCmd(ScheduleWorkCmd cmd)
        {
            try
            {
                var referenceGuid = cmd.Id;
                var actor = Context.ActorOf<WorkPerformerActor>();

                actor.Tell(new WorkerIntiationCmd(cmd.Id, cmd.SqlQuery, cmd.SqlConnection, cmd.EndPoint, cmd.LastRun));
                var cancelObject = Context.System.Scheduler.ScheduleTellRepeatedlyCancelable(TimeSpan.Zero, TimeSpan.FromMinutes(cmd.Interval), actor, new PerformWorkCmd(), Self);

                var description = new WorkDescription()
                {
                    Command = cmd,
                    Actor = actor,
                    CancelObject = cancelObject
                };

                if (cmd.SaveToDisk)
                {
                    StaticActors.SaveToDiskActor
                        .Tell(new SaveWorkItemToDiskCmd(cmd.Id, cmd.SqlQuery, cmd.SqlConnection, cmd.Interval, cmd.EndPoint));
                }


                //if (cmd.WriteToDisk)
                //{
                //    var fileName = string.Format("./ScheduledWork/{0}.txt", cmd.Id);
                //    using (var file = File.Open(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                //    {
                //        var fileContents = string.Format("{0}\r\n{1}\r\n{2}\r\n{3}\r\n",
                //            cmd.SqlQuery,
                //            cmd.SqlConnection,
                //            cmd.Interval,
                //            cmd.EndPoint
                //        );
                //        var bytes = Encoding.ASCII.GetBytes(fileContents);
                //        file.Write(bytes, 0, bytes.Length);
                //        file.Close();
                //    }
                //}

                _scheduledWork.Add(referenceGuid, description);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: {0}", e.Message);
            }
        }

        private void ReceiveCancelScheduledWorkCmd(CancelScheduledWorkCmd cmd)
        {
            try
            {
                var description = _scheduledWork[cmd.Id];

                if (description != null)
                {
                    description.CancelObject.Cancel();
                    description.Actor.Tell(new StoppingSupervisorStrategy());
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: {0}", e.Message);
            }
        }

        //private void ReceiveGetAllScheduledWorkCmd(GetAllScheduledWorkCmd cmd)
        //{

        //}
    }
}
