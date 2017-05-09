using Akka.Actor;
using SqlWorkScheduler.App.Messeges;
using System;
using System.Collections.Generic;
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
        private Dictionary<Guid, WorkDescription> _scheduledWork;

        protected override void PreStart()
        {
            _scheduledWork = new Dictionary<Guid, WorkDescription>();

            base.PreStart();
        }

        public WorkSchedulerActor()
        {
            Receive<ScheduleWorkCmd>(cmd => ReceiveScheduleWorkCmd(cmd));
            Receive<CancelScheduledWorkCmd>(cmd => ReceiveCancelScheduledWorkCmd(cmd));
            Receive<GetAllScheduledWorkCmd>(cmd => ReceiveGetAllScheduledWorkCmd(cmd));
        }

        private void ReceiveScheduleWorkCmd(ScheduleWorkCmd cmd)
        {
            try
            {
                var referenceGuid = Guid.NewGuid();
                var actor = Context.ActorOf<WorkPerformerActor>();

                actor.Tell(new WorkerIntiationCmd(cmd.SqlQuery, cmd.EndPoint));
                var cancelObject = Context.System.Scheduler.ScheduleTellRepeatedlyCancelable(cmd.IntialDelay, cmd.Interval, actor, new PerformWorkCmd(), Self);

                var description = new WorkDescription()
                {
                    Command = cmd,
                    Actor = actor,
                    CancelObject = cancelObject
                };

                _scheduledWork.Add(referenceGuid, description);

            }
            catch(Exception e)
            {
                Console.WriteLine("Error: {0}", e.Message);
            }
        }

        private void ReceiveCancelScheduledWorkCmd(CancelScheduledWorkCmd cmd)
        {
            try
            {
                var description = _scheduledWork[cmd.RefrenceGuid];

                if(description != null)
                {
                    description.CancelObject.Cancel();
                    description.Actor.Tell(new StoppingSupervisorStrategy());
                }
            }
            catch(Exception e)
            {
                Console.WriteLine("Error: {0}", e.Message);
            }
        }

        private void ReceiveGetAllScheduledWorkCmd(GetAllScheduledWorkCmd cmd)
        {

        }
    }
}
