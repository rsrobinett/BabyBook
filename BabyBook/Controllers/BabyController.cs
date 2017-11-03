using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BabyBook.Controllers
{
    [Produces("application/json")]
    [Route("api/Baby")]
    public class BabyController : Controller
    {
        // GET: api/Baby
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/Baby/5
        [HttpGet("{id}", Name = "GetBaby")]
        public string Get(int id)
        {
            return "value";
        }
        
        // POST: api/Baby
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }
        
        // PUT: api/Baby/5
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
