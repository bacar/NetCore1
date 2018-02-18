using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;

namespace ChunkedSender
{
    class Program
    {
        private const string URL = "http://localhost:5000/api/values";
        private const int SIZE_TO_SEND = 1024 * 1024 * 1024;

        static void Main(string[] args)
        {
            impl3();
        }

        public static void Log(string message)
        {
            Console.WriteLine($"{DateTimeOffset.Now:O}: {message}");
        }

        static void impl3()
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:5000/");

            using (var ms = new RandomByteStream(SIZE_TO_SEND))
            {
                var content = new StreamContent(ms);
                Log("About to post!");
                var response = client.PostAsync("/api/values", content).Result;
                Log("Post complete!");

                Log(response.StatusCode.ToString());
                Log(response.ReasonPhrase);

                foreach (var header in response.Headers)
                {
                    Log($"'{header.Key}' = '{string.Join(';', header.Value as string[])}'");
                }

                Console.WriteLine("Press enter!");
                Console.ReadLine();
            }
        }

        static void impl2()
        {
            HttpClient c = new HttpClient();
            var content = new PushStreamContent((Stream stream, HttpContent cnt, TransportContext txctx) =>
            {
                Console.WriteLine(stream.GetType());
                Random r = new Random();
                byte[] buffer = new byte[1_000_000];
                long j = 0;
                while (j < SIZE_TO_SEND)
                {
                    j += buffer.Length;
                    r.NextBytes(buffer);
                    stream.Write(buffer, 0, buffer.Length);
                    stream.Flush();
                    Console.WriteLine($"Wrote {j} bytes!");
                    System.Threading.Thread.Sleep(10);
                }
                stream.Flush();
                stream.Close();

            });
            c.PostAsync(URL, content).Wait();
        }


        static void impl1()
        {
            var wr = WebRequest.Create(URL) as HttpWebRequest;

            wr.ServicePoint.Expect100Continue = false;
            wr.Method = WebRequestMethods.Http.Put;
            wr.AllowWriteStreamBuffering = false;
           // wr.KeepAlive = true;

            wr.SendChunked = true;



            //wr.ContentType = "application/x-www-form-urlencoded";

            var wrs = wr.GetRequestStream();
            Console.WriteLine(wrs.GetType());

            Random r = new Random();
            byte[] buffer = new byte[1_000_000];


            long j = 0;
            while (j< SIZE_TO_SEND)
            {
                j+=buffer.Length;
                r.NextBytes(buffer);
                wrs.Write(buffer, 0, buffer.Length);
                wrs.Flush();
                Console.WriteLine($"Wrote {j} bytes!");
                //System.Threading.Thread.Sleep(10);
            }

            //fs.CopyTo(wrs);

            wrs.Close();

            Console.WriteLine("Press enter!");
            Console.ReadLine();

            var response = wr.GetResponse();

            var hdrs = response.Headers;

            for (int i = 0; i < hdrs.Count; ++i)
            {
                Console.WriteLine($"\n{hdrs.Keys[i]} = {hdrs[i]}");
            }
        }
    }
}
