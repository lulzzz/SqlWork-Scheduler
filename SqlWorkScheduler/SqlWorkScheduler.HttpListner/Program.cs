using ProtoBuf.Data;
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

                        response.StatusCode = 405; // method not supported

                        if (request.HttpMethod == "POST")
                        {

                            if (request.HasEntityBody)
                            {
                                var stream = request.InputStream;

                                string filePath = string.Format("./received/{0}.txt", Guid.NewGuid());
                                var fileStream = File.Open(filePath, FileMode.OpenOrCreate, FileAccess.Write);

                                var dt = new DataTable();
                                // Deserializing to a protobuf data reader and loading that into a datatable
                                using (IDataReader reader = DataSerializer.Deserialize(stream))
                                {
                                    dt.Load(reader);
                                    //Console.WriteLine(reader["OrderId"] + " " + reader["CustomerId"]);
                                    stream.CopyTo(fileStream);
                                    fileStream.Close();
                                    stream.Close();
                                }

                                if(dt != null)
                                {
                                    foreach (var key in dt.Columns)
                                    {
                                        Console.Write(key + ",");
                                    }
                                    Console.WriteLine();
                                }

                                response.StatusCode = 200;

                                response.Headers.Add("Access-Control-Allow-Origin", "*");
                                response.Headers.Add("Access-Control-Allow-Methods", "POST, GET");
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
