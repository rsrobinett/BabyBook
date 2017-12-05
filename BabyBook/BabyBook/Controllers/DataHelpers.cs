using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Http;
using Amazon.DynamoDBv2.DataModel;
using BabyBook.Models;

namespace BabyBook.Controllers
{
	public class DataHelpers
	{
		private readonly IDynamoDBContext _context;

		public DataHelpers(IDynamoDBContext context)
		{
			_context = context;
		}
		public List<Baby> BabiesForUserAndRole(User currentUser)
		{
			return currentUser.Role == BabyMemoryConstants.AdminUserRole
				? _context.Scan<Baby>().ToList()
				: _context.Query<Baby>(currentUser.Id,
						new DynamoDBOperationConfig { IndexName = "UserIdIndex" })
					.ToList();
		}

		public void DeleteMemoresForBaby(string id)
		{
			var memories = _context.Query<Memory>(id, new DynamoDBOperationConfig { IndexName = "BabyIdIndex" });
			var writeMemoryBatch = _context.CreateBatchWrite<Memory>();
			writeMemoryBatch.AddDeleteItems(memories);
			writeMemoryBatch.Execute();
		}


		public List<Memory> GetMemoriesForNonAdminRole(User currentUser, List<KeyValuePair<string, string>> parameters)
		{
			if (!parameters.Any())
			{
				return GetMemoriesForUser(currentUser);
			}

			foreach (var queryString in parameters)
			{
				if (queryString.Key != "baby") continue;

				var memories = GetMemoriesForUser(currentUser).Where(b => b.BabyId == queryString.Value).ToList();
				
				return memories;
			}

			throw new HttpResponseException(HttpStatusCode.BadRequest);
		}

		public List<Memory> GetMemoriesForAdminRole(List<KeyValuePair<string, string>> parameters)
		{
			var memories = new List<Memory>();

			if (!parameters.Any())
			{
				var scan = _context.Scan<Memory>();
				if (scan != null)
				{
					memories.AddRange(scan);
				}
			}

			foreach (var queryString in parameters)
			{
				if (queryString.Key != "baby") continue;
				var results = _context.Query<Memory>(queryString.Value, new DynamoDBOperationConfig { IndexName = "BabyIdIndex" });

				if (results != null)
				{
					memories.AddRange(results);
				}
			}

			return memories;
		}

		public List<Memory> GetMemoriesForUser(User user)
		{
			var memories = new List<Memory>();

			var userBabies = BabiesForUserAndRole(user);

			foreach (var baby in userBabies)
			{
				var results = _context.Query<Memory>(baby.Id, new DynamoDBOperationConfig { IndexName = "BabyIdIndex" });

				if (results != null)
				{
					memories.AddRange(results);
				}
			}

			return memories;
		}
	}
}