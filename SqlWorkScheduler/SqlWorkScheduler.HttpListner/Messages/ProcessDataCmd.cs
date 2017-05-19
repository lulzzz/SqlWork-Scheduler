using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlWorkScheduler.HttpReceiver.Messages
{
    class ProcessDataCmd
    {
        public DataTable Data { get; private set; }

        public ProcessDataCmd(DataTable data)
        {
            Data = data;
        }
    }
}
