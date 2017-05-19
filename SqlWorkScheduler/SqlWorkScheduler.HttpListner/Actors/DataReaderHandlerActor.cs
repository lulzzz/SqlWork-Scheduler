using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using SqlWorkScheduler.HttpReceiver.Messages;
using Microsoft.SqlServer.Server;
using System.Data;
using Akka.Configuration;

namespace SqlWorkScheduler.HttpReceiver.Actors
{
    public class StaticActor
    {
        public static IActorRef ArticleManagerActor { get; set; }
    }

    class DataReaderHandlerActor : ReceiveActor
    {
        public DataReaderHandlerActor()
        {
            Receive<ProcessDataCmd>(cmd => ReceiveProcessDataCmd(cmd));
        }

        private void ReceiveProcessDataCmd(ProcessDataCmd cmd)
        {
            //Creating actorsystem
            var system = ActorSystem.Create("ArticleManagerSystem");

            StaticActor.ArticleManagerActor = system.ActorOf<ArticleManagerActor>("ArticleManagerActor");

            //All rows in the stream
            var rows = cmd.Data.Rows;

            for (int i = 0; i < rows.Count; i++)
            {
                StaticActor.ArticleManagerActor.Tell(new ArticleDataCmd(rows[i]));
            }
        }
    }
}
