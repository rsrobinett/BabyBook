using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BabyBook.Controllers
{
    [Produces("application/json")]
    [Route("api/Memory")]
    public class MemoryController : Controller
    {
        // GET: api/Memory
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/Memory/5
        [HttpGet("{id}", Name = "GetMemory")]
        public string Get(int id)
        {
            return "value";
        }
        
        // POST: api/Memory
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }
        
        // PUT: api/Memory/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }
        
        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
