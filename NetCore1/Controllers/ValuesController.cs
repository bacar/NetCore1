using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace NetCore1.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        // GET api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            Console.WriteLine("Get");
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            Console.WriteLine($"Get - {id}");
            return "value";
        }

        // POST api/values
        [HttpPost]
        [RequestSizeLimit(10_000_000_000)]
        public void Post()
        {
            Console.WriteLine($"Post!");
            FileStream fs = new FileStream("/tmp/out", FileMode.Create);
            //Request.Body.CopyTo(fs);

            var ins = Request.Body;

            // artifically slow stream copy
            byte[] buffer = new byte[3276800];
            int read;
            int total = 0;
            int j = 0;
            while ((read = ins.Read(buffer, 0, buffer.Length)) > 0)
            {
                ++j;
                total += read;
                fs.Write(buffer, 0, read);
                fs.Flush();
                Console.WriteLine($"{j:00000} - Wrote {total} bytes!");
            }
            fs.Close();
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
