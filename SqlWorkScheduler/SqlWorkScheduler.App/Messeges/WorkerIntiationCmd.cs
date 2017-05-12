using System.Data.SqlClient;

namespace SqlWorkScheduler.Core.Messeges
{
    public class WorkerIntiationCmd
    {
        public ScheduleWorkCmd ScheduleCommand { get; set; }

        public WorkerIntiationCmd(ScheduleWorkCmd scheduleCommand)
        {
            ScheduleCommand = scheduleCommand;
        }
    }
}