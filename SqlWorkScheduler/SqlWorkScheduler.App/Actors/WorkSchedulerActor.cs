using Akka.Actor;
using SqlWorkScheduler.App.Messeges;
using System;
using System.Collections.Generic;

namespace SqlWorkScheduler.App.Actors
{
    class WorkDescription
    {
        //public ScheduleWorkCmd Command { get; set; }
        public IActorRef Actor { get; set; }
        public ICancelable CancelObject { get; set; }
    }

    public class WorkSchedulerActor : ReceiveActor
    {
        private Dictionary<string, WorkDescription> _scheduledWork;

        protected override void PreStart()
        {
            _scheduledWork = new Dictionary<string, WorkDescription>();

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

                actor.Tell(new WorkerIntiationCmd(cmd));
                var cancelObject = Context.System.Scheduler.ScheduleTellRepeatedlyCancelable(TimeSpan.Zero, TimeSpan.FromMinutes(cmd.Interval), actor, new PerformWorkCmd(), Self);

                var description = new WorkDescription()
                {
                    Actor = actor,
                    CancelObject = cancelObject
                };

                if (cmd.SaveToDisk)
                {
                    StaticActors.SaveToDiskActor
                        .Tell(new SaveWorkItemToDiskCmd(cmd));
                }

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

                    StaticActors.SaveToDiskActor.Tell(new RemoveWorkItemFromDiskCmd(cmd.Id));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: {0}", e.Message);
            }
        }
    }
}
