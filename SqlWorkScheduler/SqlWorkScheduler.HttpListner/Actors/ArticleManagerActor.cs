using Akka.Actor;
using SqlWorkScheduler.HttpReceiver.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlWorkScheduler.HttpReceiver.Actors
{
    class ArticleManagerActor : ReceiveActor
    {
        public class StaticActor
        {
            public static IActorRef ArticleActor { get; set; }
        }
        public ArticleManagerActor()
        {
            Receive<ArticleDataCmd>(cmd => ReceiveArticleDataCmd(cmd));
        }
        private void ReceiveArticleDataCmd(ArticleDataCmd cmd)
        {
            //Creating actorsystem
            var system = ActorSystem.Create("ArticleSystem");

            StaticActor.ArticleActor = system.ActorOf<ArticleActor>("ArticleActor");

            //cmd contains all articles
            //dict is a list of row values and columnnames as keys
            var dict = new Dictionary<string, object>();

            for (int i = 0; i < cmd.Row.ItemArray.Length; i++)
            {
                if (!(cmd.Row[i] is DBNull))
                {
                    dict.Add(cmd.Row.Table.Columns[i].ColumnName, cmd.Row[i]);
                }
                else
                {
                    dict.Add(cmd.Row.Table.Columns[i].ColumnName, null);
                }
            }
            
            StaticActor.ArticleActor.Tell(new ArticleCmd(dict));
        }

    }
}
