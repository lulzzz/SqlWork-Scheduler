using System;
namespace SqlWorkScheduler.App.Messeges
{
    public class CancelScheduledWorkCmd
    {
        public Guid RefrenceGuid { get; private set; }

        public CancelScheduledWorkCmd(Guid refrenceGuid)
        {
            RefrenceGuid = refrenceGuid;
        }
    }
}
