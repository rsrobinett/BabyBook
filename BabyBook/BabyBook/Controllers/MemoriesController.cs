using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Amazon.DynamoDBv2.DataModel;
using BabyBook.Models;

namespace BabyBook.Controllers
{
	public class MemoriesController : ApiController
	{
		private readonly IDynamoDBContext _context;
		public MemoriesController() : this(new BabyContext())
		{
		}

		private MemoriesController(IDynamoDBContext context)
		{
			_context = context;
		}

		// GET api/<controller>
		public IEnumerable<Memory> Get()
		{
			return _context.Scan<Memory>();
		}

		// GET api/<controller>/5
		public Memory Get(string id)
		{
			return _context.Load<Memory>(id);
		}

		// POST api/<controller>
		public Memory Post([FromBody]Memory memory)
		{
			memory.Id = Guid.NewGuid().ToString("N");
			_context.Save(memory);
			return Get(memory.Id);
		}

		// PUT api/<controller>/5
		public void Put(string id, [FromBody]Memory memory)
		{
			var existingBaby = Get(id);
			if (existingBaby == null)
			{
				return;
			}
			memory.Id = id;
			_context.Save(memory);
		}

		// DELETE api/<controller>/5
		public void Delete(string id)
		{
			var existingMemory = Get(id);
			if (existingMemory == null)
			{
				return;
			}
			_context.Delete(id);
		}
	}
}
