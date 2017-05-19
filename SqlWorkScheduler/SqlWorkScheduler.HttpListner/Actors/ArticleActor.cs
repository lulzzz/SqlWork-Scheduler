using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using SqlWorkScheduler.HttpReceiver.Messages;

namespace SqlWorkScheduler.HttpReceiver.Actors
{
    class ArticleActor : ReceiveActor
    {
        //One ArticleActor is one article row
        public string Id { get; private set; }
        public IDictionary<string, object> Article { get; private set; }
        public ArticleActor()
        {
            Receive<ArticleCmd>(cmd => ReceiveArticleCmd(cmd));
        }

        public void ReceiveArticleCmd(ArticleCmd cmd)
        {
            Id = cmd.Id;
            Article = cmd.Article;

            Console.WriteLine("This is article with id: " + Id);

            foreach (var value in cmd.Article)
            {
                Console.Write(value.Value + ", ");
            }
            Console.WriteLine("\n");
            
        }
    }
}

