using Akka.Actor;
using ProtoBuf.Data;
using SqlWorkScheduler.HttpReceiver.Actors;
using SqlWorkScheduler.HttpReceiver.Messages;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SqlWorkScheduler.HttpReceiver
{
    public class StaticActor
    {
        public static IActorRef DataReaderHandlerActor { get; set; }
    }
    class Program
    {
        static void Main(string[] args)
        {
            var system = ActorSystem.Create("ArticleManagerSystem");

            StaticActor.DataReaderHandlerActor = system.ActorOf<DataReaderHandlerActor>("ArticleReceiveActor");


            if (HttpListener.IsSupported)
            {
                var httpListner = new HttpListener();

                httpListner.Prefixes.Add("http://localhost:3550/");
                httpListner.Start();

                while (httpListner.IsListening)
                {
                    try
                    {
                        var context = httpListner.GetContext();
                        var request = context.Request;
                        var response = context.Response;

                        response.StatusCode = 405; // method not supported

                        if (request.HttpMethod == "POST")
                        {
                            if (request.HasEntityBody)
                            {
                                var stream = request.InputStream;
                                var dt = new DataTable();
                                dt.Load(DataSerializer.Deserialize(stream));

                                StaticActor.DataReaderHandlerActor.Tell(new ProcessDataCmd(dt));

                                response.StatusCode = 200;

                                response.Headers.Add("Access-Control-Allow-Origin", "*");
                                response.Headers.Add("Access-Control-Allow-Methods", "POST");
                            }

                            response.Close();
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Error: {0}", e.Message);
                        httpListner.Stop();
                    }
                }
            }
        }
    }
}
