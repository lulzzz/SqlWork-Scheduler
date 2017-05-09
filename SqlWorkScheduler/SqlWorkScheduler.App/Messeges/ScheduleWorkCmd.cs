using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlWorkScheduler.App.Messeges
{
    public class ScheduleWorkCmd
    {
        public string SqlQuery { get; private set; }
        public TimeSpan Interval { get; private set; }
        public TimeSpan IntialDelay { get; private set; }
        public string EndPoint { get; private set; }

        public ScheduleWorkCmd(string sqlQuery, TimeSpan delay, TimeSpan intialDelay, string endpoint)
        {
            SqlQuery = sqlQuery;
            IntialDelay = intialDelay;
            Interval = delay;
            EndPoint = endpoint;
        }

        public ScheduleWorkCmd(string sqlQuery, TimeSpan delay, string endpoint)
        {
            SqlQuery = sqlQuery;
            Interval = delay;
            IntialDelay = TimeSpan.Zero;
            EndPoint = endpoint;
        }
    }
}
