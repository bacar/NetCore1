using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;

namespace ChunkedSender
{
    class Program
    {
        public static void Log(string message)
        {
            Console.WriteLine($"{DateTimeOffset.Now:O}: {message}");
        }

        static void Main(string[] args)
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:5000/");

            using (var ms = new MyStream(1_000_000_000))
            {
                var content = new StreamContent(ms, 1_000);
                Log("About to post!");
                var response = client.PostAsync("/api/values", content).Result;
                Log("Post complete!");

                Log(response.StatusCode.ToString());
                Log(response.ReasonPhrase);

                foreach (var header in  response.Headers)
                {
                    Log($"'{header.Key}' = '{string.Join(';', header.Value as string[])}'");
                }

                Console.WriteLine("Press enter!");
                Console.ReadLine();
            }
        }

        public class MyStream
            : Stream
        {
            private readonly Random r = new Random();

            private long sent;
            public MyStream(long length)
            {
                Length = length;
            }

            public override bool CanRead => true;

            public override bool CanSeek => false;

            public override bool CanWrite => false;

            public override long Length { get; }

            public override long Position { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

            public override void Flush()
            {
                throw new NotImplementedException();
            }

            public override int Read(byte[] buffer, int offset, int count)
            {
                long remain = Length - sent;
                int toSend = (int)Math.Min(remain, count);

                if (toSend > 0)
                {
                    r.NextBytes(buffer);
                    sent += toSend;
                    Log($"Sending {toSend} bytes {sent}/{Length}");
                }
                else
                {
                    Log("Send complete");
                }

                return toSend;
            }

            public override long Seek(long offset, SeekOrigin origin)
            {
                throw new NotImplementedException();
            }

            public override void SetLength(long value)
            {
                throw new NotImplementedException();
            }

            public override void Write(byte[] buffer, int offset, int count)
            {
                throw new NotImplementedException();
            }
        }
    }
}
