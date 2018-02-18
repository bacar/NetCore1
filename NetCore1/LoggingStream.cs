using System;
using System.IO;
using System.Threading.Tasks;

namespace NetCore1
{
    public class LoggingStream 
        : Stream
    {
        private long received;
        private DateTime lastLogged = DateTime.Now;

        public override bool CanRead => false;

        public override bool CanSeek => false;

        public override bool CanWrite => true;

        public override long Position { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public override long Length => throw new NotImplementedException();

        private static void Log(string message)
        {
            Console.WriteLine($"{DateTimeOffset.Now:O}: {message}");
        }

        public override void Flush()
        {
            //throw new NotImplementedException();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
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
            received += count;
            var now = DateTime.UtcNow;
            if (now - lastLogged > TimeSpan.FromMilliseconds(500))
            {
                Log($"Received {received/1024/1024} MB");
                lastLogged = now;
            }
        }

        public override System.Threading.Tasks.Task WriteAsync(byte[] buffer, int offset, int count, System.Threading.CancellationToken cancellationToken)
        {
            Write(buffer, offset, count);
            return Task.FromResult(0);
        }
    }
}
