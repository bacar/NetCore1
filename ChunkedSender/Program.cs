﻿using System;
using System.IO;
using System.Net;
using System.Text;

namespace ChunkedSender
{
    class Program
    {
        static void Main(string[] args)
        {

            var wr = WebRequest.Create("http://localhost:5000/api/values") as HttpWebRequest;

            wr.ServicePoint.Expect100Continue = false;
            wr.Method = WebRequestMethods.Http.Post;
            wr.AllowWriteStreamBuffering = false;
           // wr.KeepAlive = true;

            wr.SendChunked = true;

            //wr.ContentType = "application/x-www-form-urlencoded";

            var wrs = wr.GetRequestStream();

            Random r = new Random();
            byte[] buffer = new byte[1_000_000];


            long j = 0;
            while (j<1_000_000)
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
