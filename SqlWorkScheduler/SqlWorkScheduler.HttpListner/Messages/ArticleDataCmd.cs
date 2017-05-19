using Akka.Actor;
using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlWorkScheduler.HttpReceiver.Messages
{
    class ArticleDataCmd
    {
        public DataRow Row { get; private set; }
        public ArticleDataCmd(DataRow row)
        {
            Row = row;
        }
    }
}
