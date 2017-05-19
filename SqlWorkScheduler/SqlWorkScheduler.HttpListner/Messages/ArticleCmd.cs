using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlWorkScheduler.HttpReceiver.Messages
{
    class ArticleCmd
    {
        public string Id { get; set; }
        public Dictionary<string, object> Article { get; private set; }
        public ArticleCmd(Dictionary<string, object> article)
        {
            //sets Id to articles id value
            Id = article.Values.First().ToString();
            Article = article;
        }
    }
}
