using System;
namespace SqlWorkScheduler.Core.Messeges
{
    public class CancelScheduledWorkCmd
    {
        public string Id { get; private set; }

        public CancelScheduledWorkCmd(string id)
        {
            Id = id;
        }
    }
}
