using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using Amazon.DynamoDBv2.DataModel;
using BabyBook.Models;

namespace BabyBook.Controllers
{
	public class BabiesController : ApiController
	{
		private readonly IDynamoDBContext _context;
		private readonly AuthController _authController;

		public BabiesController() : this(new BabyContext(), new AuthController())
		{
		}

		private BabiesController(IDynamoDBContext context, AuthController authController)
		{
			_context = context;
			_authController = authController;
		}
		
		// GET api/<controller>
		public async Task<IEnumerable<Baby>> Get()
		{
			var currentUser = await _authController.GetVerifiedUser(Request.Headers.Authorization);

			if (currentUser == null || currentUser.Role != BabyMemoryConstants.AdminUserRole)
			{
				throw new HttpResponseException(HttpStatusCode.Unauthorized);
			}

			return _context.Scan<Baby>();
		}

		// GET api/<controller>/5
		public async Task<Dictionary<string, object>> Get(string id)
		{
			var currentUser = await _authController.GetVerifiedUser(Request.Headers.Authorization);

			if (currentUser == null || currentUser.BabyIds.All(x => x != id) ||
			    currentUser.Role != BabyMemoryConstants.AdminUserRole)
			{
				throw new HttpResponseException(HttpStatusCode.Unauthorized);
			}

			var baby = _context.Load<Baby>(id);

			return ResponseDictionary(baby);
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
		/// <summary>
		/// Creates baby, adds baby to authorized user, adds memory of babys birth
		/// </summary>
		/// <remarks>
		/// There is no special behavior for admin users.
		/// </remarks>
		/// <param name="baby"></param>
		/// <returns></returns>
		public async Task<Dictionary<string, object>> Post([FromBody]Baby baby)
		{
			var currentUser = await _authController.GetVerifiedUser(Request.Headers.Authorization);
			
			if (currentUser == null)
			{
				throw new HttpResponseException(HttpStatusCode.Unauthorized);
			}

			baby.Id = Guid.NewGuid().ToString("N");
			_context.Save<Baby>(baby);

			currentUser.BabyIds.Add(baby.Id);
			_context.Save<User>(currentUser);

			var memory = new Memory
			{
				Id = Guid.NewGuid().ToString("N"),
				BabyId = baby.Id,
				Date = baby.DateOfBirth,
				Description = "Born"
			};
			_context.Save<Memory>(memory);

			return ResponseDictionary(_context.Load<Baby>(baby.Id),_context.Load<User>(currentUser.Id),
				_context.Load<Memory>(memory.Id));
		}

		// PUT api/<controller>/5
		/// <summary>
		/// Updates baby.
		/// </summary>
		/// <remarks>
		/// Associated User or Admin user can update baby.  
		/// </remarks>
		/// <param name="id"></param>
		/// <param name="baby"></param>
		/// <exception cref="HttpResponseException"></exception>
		public async void Put(string id, [FromBody]Baby baby)
		{
			var currentUser = await _authController.GetVerifiedUser(Request.Headers.Authorization);

			if (currentUser == null || currentUser.BabyIds.All(x => x != id) ||
			    currentUser.Role != BabyMemoryConstants.AdminUserRole)
			{
				throw new HttpResponseException(HttpStatusCode.Unauthorized);
			}

			var existingBaby = _context.Load<Baby>(id);
			if (existingBaby == null)
			{
				throw new HttpResponseException(HttpStatusCode.BadRequest);
			}
			baby.Id = id;
			_context.Save<Baby>(baby);
		}

		// DELETE api/<controller>/5
		/// <summary>
		/// Deletes baby, removes baby from the user and deletes associated memories.
		/// </summary>
		/// <remarks>
		/// Only the user associated with the baby can delete the baby.  
		/// Admin role cannot delete a baby that is not associated with the user.  
		/// </remarks>
		/// <param name="id"></param>
		/// <exception cref="HttpResponseException"></exception>
		public async Task<IHttpActionResult> Delete(string id)
		{
			var currentUser = await _authController.GetVerifiedUser(Request.Headers.Authorization);

			if (currentUser == null || currentUser.BabyIds.All(x => x != id))
			{
				throw new HttpResponseException(HttpStatusCode.Unauthorized);
			}

			var existingBaby = _context.Load<Baby>(id);
			if (existingBaby == null)
			{
				return StatusCode(HttpStatusCode.NoContent);
			}

			var results = _context.Query<Memory>(id, new DynamoDBOperationConfig { IndexName = "BabyIdIndex" });

			//var asyncResults = _context.QueryAsync<Memory>(id, new DynamoDBOperationConfig { IndexName = "BabyIdIndex" });
			
			foreach (var result in results)
			{
				_context.Delete<Memory>(result.Id);
			}

			currentUser.BabyIds.Remove(existingBaby.Id);

			_context.Delete<Baby>(existingBaby.Id);

			return StatusCode(HttpStatusCode.NoContent);
		}

		private Dictionary<string, object> ResponseDictionary(Baby baby, User user = null, Memory memory = null)
		{
			Dictionary<string, object> metadata = new Dictionary<string, object>();

			metadata.Add("baby", baby);
			metadata.Add("url", Url.Route("DefaultApi", new { controller = "babies" }));

			if (user != null)
			{
				metadata.Add("url", Url.Route("DefaultApi", new { controller = "user", action = user.Id}));
			}

			if (memory != null)
			{
				metadata.Add("url", Url.Route("DefaultApi", new { controller = "memories", action = memory.Id }));
			}
			return metadata;
		}
	}
}