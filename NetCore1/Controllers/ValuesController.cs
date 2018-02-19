using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace StreamingServer.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        Random r = new Random();

        // GET api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            Console.WriteLine("Get");
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{size}")]
        public void Get(int size)
        {
            const int BUF_SIZE = 100;
            byte[] bytes = new byte[BUF_SIZE];
            Console.WriteLine($"Get - {size}");

            Response.ContentType = "text/plain";
            Stream str = Response.Body;
            StreamWriter sw = new StreamWriter(str);
            int done = 0;
            while(done<size)
            {
                Console.WriteLine($"Written {done}...");
                r.NextBytes(bytes);
                String b64 = Convert.ToBase64String(bytes);
                sw.WriteLine(b64);
                str.Flush();
                done += b64.Length;
                Thread.Sleep(10);
            }
            sw.Close();
        }

        // POST api/values
        [HttpPost]
        [HttpPut]
        [RequestSizeLimit(10_000_000_000)]
        public async Task Post()
        {
            Console.WriteLine(
                String.Join("\n", Request.Headers.Select(x => $"{x.Key} = {x.Value}")));

            Console.WriteLine($"Post!");
            //FileStream fs = new FileStream("/tmp/out", FileMode.Create);
            //Request.Body.CopyTo(fs);

            var ins = Request.Body;

            using(var loggingStream = new LoggingStream())
            {
                await ins.CopyToAsync(loggingStream);
            }

            Console.WriteLine($"Post done!");
        }
    }
}
