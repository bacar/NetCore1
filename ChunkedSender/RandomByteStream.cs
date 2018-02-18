using System;
using System.IO;

namespace ChunkedSender
{

    public class RandomByteStream
        : Stream
    {
        private readonly Random r = new Random();
        private DateTime lastLogged = DateTime.Now;

        private long sent;
        public RandomByteStream(long length)
        {
            Length = length;
        }

        public override bool CanRead => true;

        public override bool CanSeek => false;

        public override bool CanWrite => false;

        public override long Length { get; }

        public override long Position { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        private static void Log(string message)
        {
            Console.WriteLine($"{DateTimeOffset.Now:O}: {message}");
        }


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

                var now = DateTime.UtcNow;
                if (now - lastLogged > TimeSpan.FromMilliseconds(500))
                {
                    Log($"Sending {toSend} bytes {sent / 1024 / 1024}MiB/{Length / 1024 / 1024}MiB");
                    lastLogged = now;
                }
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
