using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace NetCore1.Controllers
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

            // artifically slow stream copy
            byte[] buffer = new byte[3276800];
            int read;
            int total = 0;
            int j = 0;
            while ((read = await ins.ReadAsync(buffer, 0, buffer.Length)) > 0)
            {
                ++j;
                total += read;
                //fs.Write(buffer, 0, read);
                //fs.Flush();
                Console.WriteLine($"{j:00000} - Wrote {total} bytes!");
            }
            //fs.Close();
            Console.WriteLine($"Post done!");
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
            Console.WriteLine("Put");
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
