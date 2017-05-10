using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SqlWorkScheduler.HttpReceiver
{
    class Program
    {
        static void Main(string[] args)
        {
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

                        response.StatusCode = 405;

                        if (request.HttpMethod == "POST")
                        {

                            if(request.HasEntityBody)
                            {
                                var stream = request.InputStream;

                                string filePath = string.Format("./{0}.txt", Guid.NewGuid());
                                var fileStream = File.Open(filePath, FileMode.OpenOrCreate, FileAccess.Write);

                                stream.CopyTo(fileStream);
                                fileStream.Flush();
                                fileStream.Close();
                                stream.Close();
                            }

                            response.StatusCode = 200;

                            response.Headers.Add("Access-Control-Allow-Origin", "*");
                            response.Headers.Add("Access-Control-Allow-Methods", "POST, GET");
                        }

                        response.Close();
                    }
                    catch(Exception e)
                    {
                        Console.WriteLine("Error: {0}", e.Message);
                        httpListner.Stop();
                    }
                }
            }
        }
    }
}
