using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using Amazon.DynamoDBv2.DataModel;
using BabyBook.Models;
using Microsoft.Ajax.Utilities;

namespace BabyBook.Controllers
{
	public class BabiesController : ApiController
	{
		private readonly IDynamoDBContext _context;
		private readonly AuthController _authController;
		private readonly DataHelpers _dataHelpers;

		public BabiesController() : this(new BabyContext(), new AuthController())
		{
		}

		private BabiesController(IDynamoDBContext context, AuthController authController)
		{
			_context = context;
			_authController = authController;
			_dataHelpers = new DataHelpers(_context);
		}

		// GET api/<controller>
		/// <summary>
		/// Get Babies
		/// </summary>
		/// <remarks>
		/// Admin Users get all babies.
		/// Basic Users get only babies for the user.
		/// </remarks>
		/// <returns>Where does this go?</returns>
		/// <example>Where is the example?</example>
		/// <exception cref="HttpResponseException"></exception>
		public async Task<List<Dictionary<string, object>>> Get()
		{
			var currentUser = await _authController.GetVerifiedUser(Request.Headers.Authorization);

			if (currentUser == null)
			{
				throw new HttpResponseException(HttpStatusCode.Unauthorized);
			}

			var responseDictionaryList = new List<Dictionary<string, object>>();

			if (currentUser.Role == BabyMemoryConstants.AdminUserRole)
			{
				var allBabies = _context.Scan<Baby>();
				responseDictionaryList.AddRange(allBabies.Select(b => ResponseDictionary(b)));
				return responseDictionaryList;
			}

			var userBabies = _dataHelpers.BabiesForUserAndRole(currentUser);
			
			responseDictionaryList.AddRange(userBabies.Select(userBaby => ResponseDictionary(userBaby)));

			return responseDictionaryList;
		}

		// GET api/<controller>/5
		/// <summary>
		/// Get Baby By Id
		/// </summary>
		/// <remarks>
		/// Admin users can get any baby
		/// Basic users can only get babies for the authorized user
		/// </remarks>
		/// <param name="id"></param>
		/// <returns></returns>
		/// <exception cref="HttpResponseException"></exception>
		public async Task<Dictionary<string, object>> Get(string id)
		{
			var currentUser = await _authController.GetVerifiedUser(Request.Headers.Authorization);

			if (currentUser == null)
			{
				throw new HttpResponseException(HttpStatusCode.Unauthorized);
			}

			if (currentUser.Role == BabyMemoryConstants.AdminUserRole)
			{
				var baby = _context.Load<Baby>(id);
				return ResponseDictionary(baby);
			}

			var userBabies = _dataHelpers.BabiesForUserAndRole(currentUser);

			if (!(userBabies.Any(x => x.Id == id)))
			{
				throw new HttpResponseException(HttpStatusCode.Unauthorized);
			}

			//var usersBaby = _context.Load<Baby>(id);
			return ResponseDictionary(userBabies.FirstOrDefault(x => x.Id == id));
		}

		// POST api/<controller>
		/// <summary>
		/// Create baby
		/// </summary>
		/// <remarks>
		/// Creates baby, adds baby to authorized user, adds memory of babys birth
		/// Admin has the same abilites as a basic user.
		/// There is no special behavior for admin users.
		/// </remarks>
		/// <param name="baby"></param>
		/// <returns></returns>
		public async Task<IHttpActionResult> Post([FromBody]Baby baby)
		{
			var currentUser = await _authController.GetVerifiedUser(Request.Headers.Authorization);
			
			if (currentUser == null)
			{
				throw new HttpResponseException(HttpStatusCode.Unauthorized);
			}

			if (currentUser.Role != BabyMemoryConstants.AdminUserRole || !baby.UserId.IsNullOrWhiteSpace())
			{
				baby.UserId = currentUser.Id;
			}
			else
			{
				var userOfBaby = _context.Load<User>(baby.UserId);

				if (userOfBaby == null)
				{
					throw new HttpResponseException(HttpStatusCode.BadRequest);
				}
				else
				{
					baby.UserId = userOfBaby.Id;
				}

			}

			baby.Id = Guid.NewGuid().ToString("N");
			_context.Save<Baby>(baby);

			var memory = new Memory
			{
				Id = Guid.NewGuid().ToString("N"),
				BabyId = baby.Id,
				Date = baby.DateOfBirth,
				Description = BabyMemoryConstants.BirthDescription
			};
			_context.Save<Memory>(memory);

			return Created(Url.Route("DefaultApi", new { controller = "Babies" }), ResponseDictionary(_context.Load<Baby>(baby.Id)));
		}

		// PUT api/<controller>/5
		/// <summary>
		/// Update baby
		/// </summary>
		/// <remarks>
		/// Associated User or Admin user can update baby, but not the id.  
		/// </remarks>
		/// <param name="id"></param>
		/// <param name="baby"></param>
		/// <exception cref="HttpResponseException"></exception>
		public async Task<Dictionary<string,object>> Put(string id, [FromBody]Baby baby)
		{
			var currentUser = await _authController.GetVerifiedUser(Request.Headers.Authorization);

			if (currentUser == null)
			{
				throw new HttpResponseException(HttpStatusCode.Unauthorized);
			}

			var userBabies = _dataHelpers.BabiesForUserAndRole(currentUser);
			var currentBaby = userBabies.FirstOrDefault(x => x.Id == id);

			if (currentBaby == null)
			{
				throw new HttpResponseException(HttpStatusCode.BadRequest);
			}

			if (baby.DateOfBirth != currentBaby.DateOfBirth)
			{
				//todo: add functionality to update birth memory if dob changes. 	
			}

			if (currentUser.Role != BabyMemoryConstants.AdminUserRole || baby.UserId.IsNullOrWhiteSpace())
			{
				baby.UserId = currentBaby.UserId;
			}
			
			baby.Id = id;
			
			_context.Save<Baby>(baby);

			return ResponseDictionary(baby);
		}

		// DELETE api/<controller>/5
		/// <summary>
		/// Deletes baby
		/// </summary>
		/// <remarks>
		/// Deletes baby, removes baby from the user and deletes associated memories.
		/// Admin can delete any baby
		/// Basic user can only delete baby associated with the authorized user
		/// </remarks>
		/// <param name="id"></param>
		/// <exception cref="HttpResponseException"></exception>
		public async Task<IHttpActionResult> Delete(string id)
		{
			var currentUser = await _authController.GetVerifiedUser(Request.Headers.Authorization);

			if (currentUser == null)
			{
				throw new HttpResponseException(HttpStatusCode.Unauthorized);
			}

			var babies = _dataHelpers.BabiesForUserAndRole(currentUser);

			if (!(babies.Any(x => x.Id == id)))
			{
				throw new HttpResponseException(HttpStatusCode.Unauthorized);
			}

			var existingBaby = _context.Load<Baby>(id);
			if (existingBaby == null)
			{
				return StatusCode(HttpStatusCode.NoContent);
			}

			//delete memories
			_dataHelpers.DeleteMemoresForBaby(id);

			_context.Delete<Baby>(existingBaby.Id);

			return StatusCode(HttpStatusCode.NoContent);
		}
		
		private Dictionary<string, object> ResponseDictionary(Baby baby)
		{
			Dictionary<string, object> metadata = new Dictionary<string, object>();

			metadata.Add("baby", baby);
			metadata.Add("url", Url.Route("DefaultApi", new { controller = "babies", id=baby.Id }));

			var user = _context.Load<User>(baby.UserId);

			if (user != null)
			{
				metadata.Add("user_url", Url.Route("DefaultApi", new { controller = "user", id = user.Id}));
			}

			var memories = _context.Query<Memory>(baby.Id, new DynamoDBOperationConfig { IndexName = "BabyIdIndex" });

			if (memories != null && memories.Any())
			{
				metadata.Add("memory_url", Url.Route("DefaultApi", new { controller = "memories", baby = baby.Id }));
				
			}
			
			return metadata;
		}
	}
}