﻿using System;
using System.Collections.Generic;
using System.Web.Http;
using Amazon.DynamoDBv2.DataModel;
using BabyBook.Models;

namespace BabyBook.Controllers
{
	public class BabiesController : ApiController
	{
		private readonly IDynamoDBContext _context;

		public BabiesController() : this(new BabyContext())
		{
		}

		private BabiesController(IDynamoDBContext context)
		{
			_context = context;
		}
		
		// GET api/<controller>
		public IEnumerable<Baby> Get()
		{
			return _context.Scan<Baby>();
		}

		// GET api/<controller>/5
		public Baby Get(string id)
		{
			 return _context.Load<Baby>(id);
		}

		/*
		public List<Baby> Get([FromBody] List<string> idList)
		{
			var batchGet = _context.CreateBatchGet<Baby>();
			foreach (var id in idList)
			{
				batchGet.AddKey(id);
			}
			batchGet.Execute();
			return batchGet.Results;
		}
		*/

		// POST api/<controller>
		public Baby Post([FromBody]Baby baby)
		{
			baby.Id = Guid.NewGuid().ToString("N");
			_context.Save<Baby>(baby);

			var memory = new Memory
			{
				Id = Guid.NewGuid().ToString("N"),
				BabyId = baby.Id,
				Date = baby.DateOfBirth,
				Description = "Born"
			};
			_context.Save<Memory>(memory);

			return Get(baby.Id);
		}

		// PUT api/<controller>/5
		public void Put(string id, [FromBody]Baby baby)
		{
			var existingBaby = Get(id);
			if (existingBaby == null)
			{
				return;
			}
			baby.Id = id;
			_context.Save<Baby>(baby);
		}

		// DELETE api/<controller>/5
		public void Delete(string id)
		{
			var existingBaby = Get(id);
			if (existingBaby == null)
			{
				return;
			}

			var results = _context.Query<Memory>(id, new DynamoDBOperationConfig { IndexName = "BabyIdIndex" });

			foreach (var result in results)
			{
				_context.Delete<Memory>(result.Id);
			}

			_context.Delete<Baby>(existingBaby.Id);
		}
	}
}