using Proto;
using SqlWorkScheduler.Core.Messeges;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlWorkScheduler.ProtoActorVersion.Actors
{
    class WorkSchedulerActor : IActor
    {
        public Task ReceiveAsync(IContext context)
        {
            if(context.Message is ScheduleWorkCmd)
            {
                
            }

            return Actor.Done;
        }
    }
}
