﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Amazon.DynamoDBv2.DataModel;
using BabyBook.Models;

namespace BabyBook.Controllers
{
	public class MemoriesController : ApiController
	{
		private readonly IDynamoDBContext _context;
		private readonly AuthController _authController;
		private readonly DataHelpers _dataHelpers;

		public MemoriesController() : this(new BabyContext(), new AuthController())
		{
		}

		private MemoriesController(IDynamoDBContext context, AuthController authController)
		{
			_context = context;
			_authController = authController;
			_dataHelpers = new DataHelpers(_context);
		}


		// GET api/<controller>
		/// <summary>
		/// Get Memories
		/// </summary>
		/// <remarks>
		/// Lists all memories, possibly just memories for user logged in for baby
		/// </remarks>
		public async Task<List<Dictionary<string, object>>> Get()
		{
			var currentUser = await _authController.GetVerifiedUser(Request.Headers.Authorization);

			if (currentUser == null)
			{
				throw new HttpResponseException(HttpStatusCode.Unauthorized);
			}

			var parameters = Request.GetQueryNameValuePairs().ToList();
			
			List<Memory> memories;

			switch (currentUser.Role)
			{
				case "admin":
					memories =  _dataHelpers.GetMemoriesForAdminRole(parameters);
					break;
				default:
					memories = _dataHelpers.GetMemoriesForNonAdminRole(currentUser, parameters);
					break;
			}

			return memories.Select(memory => ResponseDictionary(memory)).ToList();
		}

		// GET api/<controller>/5
		/// <summary>
		/// Get memory by id
		/// </summary>
		/// <remarks>
		/// Admin User can get any user
		/// Basic user can only get itself
		/// </remarks>
		/// <param name="id"></param>
		/// <returns></returns>
		public async Task<Dictionary<string, object>> Get(string id)
		{
			var currentUser = await _authController.GetVerifiedUser(Request.Headers.Authorization);

			if (currentUser == null)
			{
				throw new HttpResponseException(HttpStatusCode.Unauthorized);
			}

			var currentMemory = _context.Load<Memory>(id);

			if (currentMemory == null)
			{
				throw new HttpResponseException(HttpStatusCode.BadRequest);
			}

			var userMemories = new List<Memory>();

			userMemories.AddRange(currentUser.Role == BabyMemoryConstants.AdminUserRole
				? _context.Scan<Memory>()
				: _dataHelpers.GetMemoriesForUser(currentUser));
			
			if (!userMemories.Exists(m => m.Id == currentMemory.Id))
			{
				throw new HttpResponseException(HttpStatusCode.Unauthorized);
			}

			return ResponseDictionary(currentMemory);
		}


		// POST api/<controller>
		/// <summary>
		/// Add new memory
		/// </summary>
		/// <param name="memory"></param>
		/// <returns></returns>
		public async Task<IHttpActionResult> Post([FromBody]Memory memory)
		{
			var currentUser = await _authController.GetVerifiedUser(Request.Headers.Authorization);

			if (currentUser == null)
			{
				throw new HttpResponseException(HttpStatusCode.Unauthorized);
			}

			var userBabies = _dataHelpers.BabiesForUserAndRole(currentUser);

			if (!(userBabies.Exists(x => x.Id == memory.BabyId)))
			{
				throw new HttpResponseException(HttpStatusCode.Unauthorized);
			}

			memory.Id = Guid.NewGuid().ToString("N");
			_context.Save<Memory>(memory);
			return Created(Url.Route("DefaultApi", new { controller = "Memories" }), ResponseDictionary(memory));
		}

		// PUT api/<controller>/5
		/// <summary>
		///</summary>
		/// <remarks>
		/// Update memory date or description.
		/// Admin can update any memory
		/// Basic user can only update memories associated with the authorized user. 
		/// BabyId cannot be updated
		/// </remarks>
		/// <param name="id"></param>
		/// <param name="memory"></param>
		/// <returns></returns>
		/// <exception cref="HttpResponseException"></exception>
		public async Task<Dictionary<string,object>> Put(string id, [FromBody]Memory memory)
		{
			var currentUser = await _authController.GetVerifiedUser(Request.Headers.Authorization);

			if (currentUser == null)
			{
				throw new HttpResponseException(HttpStatusCode.Unauthorized);
			}

			var userMemories = _dataHelpers.GetMemoriesForUser(currentUser);
			var currentMemory = _context.Load<Memory>(id);

			if (currentMemory == null)
			{
				throw new HttpResponseException(HttpStatusCode.Unauthorized);
			}

			if (!userMemories.Exists(m => m.Id == currentMemory.Id))
			{
				throw new HttpResponseException(HttpStatusCode.Unauthorized);
			}
			
			memory.Id = id;
			memory.BabyId = currentMemory.BabyId;
			_context.Save<Memory>(memory);

			return ResponseDictionary(memory);
		}

		// DELETE api/<controller>/5
		/// <summary>Delete Memory</summary>
		/// <remarks>
		/// Admin User can delete any user
		/// Basic User can only delete the authorized user
		/// </remarks>
		/// <example>where does this go</example>
		/// <param name="id"></param>
		public async Task<IHttpActionResult> Delete(string id)
		{
			var currentUser = await _authController.GetVerifiedUser(Request.Headers.Authorization);

			if (currentUser == null)
			{
				throw new HttpResponseException(HttpStatusCode.Unauthorized);
			}

			var currentMemory = _context.Load<Memory>(id);

			if (currentMemory == null)
			{
				throw new HttpResponseException(HttpStatusCode.Unauthorized);
			}

			var userMemories = new List<Memory>();

			userMemories.AddRange(currentUser.Role == BabyMemoryConstants.AdminUserRole
				? _context.Scan<Memory>()
				: _dataHelpers.GetMemoriesForUser(currentUser));
			
			if (!userMemories.Exists(m => m.Id == currentMemory.Id))
			{
				throw new HttpResponseException(HttpStatusCode.Unauthorized);
			}

			_context.Delete<Memory>(id);

			return StatusCode(HttpStatusCode.NoContent);
		}





		private Dictionary<string, object> ResponseDictionary(Memory memory, User user = null)
		{
			Dictionary<string, object> metadata = new Dictionary<string, object>();

			if (memory != null)
			{
				metadata.Add("memory", memory);
				metadata.Add("url", Url.Route("DefaultApi", new { controller = "memories", id = memory.Id}));
			}

			var baby = _context.Load<Baby>(memory.BabyId);

			if (baby != null)
			{
				metadata.Add("baby_url", Url.Route("DefaultApi", new {controller = "babies", id = baby.Id}));

			}

			if (user != null)
			{
				metadata.Add("user_url", Url.Route("DefaultApi", new {controller = "user", id = user.Id}));
			}

			return metadata;
		}

	}
}
