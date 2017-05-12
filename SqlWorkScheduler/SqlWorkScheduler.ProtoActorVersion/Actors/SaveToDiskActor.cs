using Proto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlWorkScheduler.ProtoActorVersion.Actors
{
    class SaveToDiskActor : IActor
    {
        public Task ReceiveAsync(IContext context)
        {

            return Actor.Done;
        }
    }
}
