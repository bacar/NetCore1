using System;
using System.IO;
using System.Net;

namespace ChunkedSender
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            var wr = WebRequest.Create("http://localhost:5000/api/values");
            wr.Method = WebRequestMethods.Http.Post;

            ((HttpWebRequest)wr).SendChunked = true;

            var fs = new FileStream("/tmp/largefile", FileMode.Open);
            var wrs = wr.GetRequestStream();

            byte[] buffer = new byte[3276800];
            int read;
            int total = 0;
            int j = 0;
            while ((read = fs.Read(buffer, 0, buffer.Length)) > 0)
            {
                ++j;
                total += read;
                wrs.Write(buffer, 0, read);
                wrs.Flush();
                Console.WriteLine($"{j:00000} - Wrote {total} bytes!");
                System.Threading.Thread.Sleep(10);
            }

            //fs.CopyTo(wrs);

            fs.Close();
            wrs.Close();


            Console.WriteLine("Bye!");
        }
    }
}
