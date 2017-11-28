using System;
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
    /// <summary>
    /// 
    /// </summary>
    public class UserController : ApiController
    {
	    private readonly IDynamoDBContext _context;
	    private readonly AuthController _authController;
	    private BabiesController _babiesController;

	    /// <summary>
		/// Controller to maintain user identities. 		
		/// </summary>
		public UserController() : this (new BabyContext(), new AuthController(), new BabiesController())
	    {
		    
	    }

	    private UserController(IDynamoDBContext context, AuthController authController, BabiesController babiesController)
	    {
		    _context = context;
		    _authController = authController;
		    _babiesController = babiesController;
	    }


		// Get: api/user/5
		/// <summary>
		/// Get user by Id
		/// </summary>
		/// <remarks>
		/// For Admin User returns any user.
		/// For non-admin user, only returns info for the user.
		/// </remarks>
		/// <param name="id"></param>
		/// <returns>User</returns>
		/// <response code="401">Unauthorized: due to user not token not authorized or the request is not available to user role</response> 
		/// <exception cref="HttpResponseException"></exception>
		public async Task<IHttpActionResult> Get(string id)
	    {
		    var currentUser = await _authController.GetVerifiedUser(Request.Headers.Authorization);
		    if (currentUser is null)
		    {
				throw new HttpResponseException(HttpStatusCode.BadRequest);
			}
		    
			if(currentUser.Id != id && currentUser.Role != BabyMemoryConstants.AdminUserRole)
		    {
				throw new HttpResponseException(HttpStatusCode.BadRequest);
			}

		    var user = _context.Load<User>(id);

		    return Ok(ResponseDictionary(user));
	    }

		// GET: api/User
		/// <summary>
		/// Get Users
		/// </summary>
		/// <remarks>
		/// A parameter of email can be used to get users by email address
		/// Users with admin role can get any users
		/// Users without admin role can only get their own user by email address
		/// </remarks>
		/// <response code="401">Unauthorized: due to user not token not authorized or the request is not available to user role</response> 
		/// <returns>User</returns>
		/// <exception cref="HttpResponseException"></exception>
		public async Task<IEnumerable<User>> Get()
        {
			var currentUser = await _authController.GetVerifiedUser(Request.Headers.Authorization);
	        if (currentUser is null)
	        {
		        throw new HttpResponseException(HttpStatusCode.Unauthorized);
	        }

			var parameters = Request.GetQueryNameValuePairs();

	        if (!parameters.Any())
	        {

		        if (currentUser.Role != BabyMemoryConstants.AdminUserRole)
		        {
			        throw new HttpResponseException(HttpStatusCode.Unauthorized);
		        }

				return _context.Scan<User>();
	        }

	        var users = new List<User>();

	        foreach (var queryString in parameters)
	        {
		        if (queryString.Key.ToLower() == "email")
		        {

			        if ( currentUser.Email != queryString.Key.ToLower() || currentUser.Role != BabyMemoryConstants.AdminUserRole)
			        {
				        throw new HttpResponseException(HttpStatusCode.Unauthorized);
			        }

					var results = _context.Query<User>(queryString.Value.ToLower(), new DynamoDBOperationConfig { IndexName = "UserEmailIndex" });

			        if (results != null)
			        {
				        users.AddRange(results);
			        }
		        }
	        }

	        return users;
		}

		/// <summary>
		/// Create User
		/// </summary>
		/// <remarks>
		/// Admin user can add any user with any role.
		/// Non admin user can only add a user with their own verified email address. 
		/// No user can add an email address that already exists.
		/// 
		/// BabyIds is always blank (babies are added via baby conroller)
		///  and Id is auto generated.
		/// Role can be generated as admin when it is created.
		/// </remarks>
		/// <param name="user"></param>
		/// <response code="519">email address already used</response>
		/// <response code="401">Unauthorized: due to user not token not authorized or the request is not available to user role</response> 
		/// <response code="201">Created</response>
		/// <returns>User</returns>
		// POST: api/User
		public async Task<IHttpActionResult> Post([FromBody]User user)
        {
	        var currentUserEmail = await _authController.GetVerifiedEmail(Request.Headers.Authorization);
			if (currentUserEmail is null)
	        {
		        throw new HttpResponseException(HttpStatusCode.Unauthorized);
	        }

	        var currentUser = await _authController.GetVerifiedUser(Request.Headers.Authorization);

			user.Id = Guid.NewGuid().ToString("N");

	        if (!string.Equals(currentUserEmail, user.Email, StringComparison.CurrentCultureIgnoreCase) &&
	            currentUser?.Role != BabyMemoryConstants.AdminUserRole)
	        {
		        throw new HttpResponseException(HttpStatusCode.Unauthorized);
			}

	        if (currentUser?.Role != BabyMemoryConstants.AdminUserRole)
	        {
		        user.Email = user.Email.ToLower();
	        }

			user.BabyIds = new List<string>();

			var existingUsers = _context.Query<User>(user.Email.ToLower(), new DynamoDBOperationConfig { IndexName = "UserEmailIndex" }).ToList();

	        if (existingUsers.Count > 1)
	        {
		        throw new HttpResponseException(HttpStatusCode.Conflict);
			}

	        _context.Save<User>(user);

	        return Created(Url.Route("DefaultApi", new { controller = "User" }), ResponseDictionary(user));
		}

		/// <summary>
		/// Update user
		/// </summary>
		/// <remarks>
		/// Admin users can change email and role.  
		/// Non-admin users can only change the email on the account (note: user would then lose access unless they provide a token for the new email address).
		/// </remarks>
		/// <param name="id"></param>
		/// <param name="user"></param>
		///<response code = "401" > Unauthorized: due to user not token not authorized or the request is not available to user role</response>
		/// <reponse code = "200" exmaple ="{'exmaple json here'}"></reponse>
		/// <return>User</returns>
		// PUT: api/User/5
		public async Task<Dictionary<string, object>> Put(string id, [FromBody]User user)
        {
	        var currentUser = await _authController.GetVerifiedUser(Request.Headers.Authorization);
	        if (currentUser is null)
	        {
		        throw new HttpResponseException(HttpStatusCode.Unauthorized);
	        }

			if (currentUser.Id != id && currentUser.Role != BabyMemoryConstants.AdminUserRole)
	        {
		        throw new HttpResponseException(HttpStatusCode.BadRequest);
	        }

	        var usertoUpdate = _context.Load<User>(id);

	        if (currentUser.Role == BabyMemoryConstants.AdminUserRole)
	        {
		        usertoUpdate.Role = user.Role;
	        }

			usertoUpdate.Email = user.Email;
			
	        user.Id = id;
	        
	        _context.Save<User>(user);

	        return ResponseDictionary(user);
        }

		/// <summary>
		/// Delete User, Associated babies and associated memories. 
		/// </summary>
		/// <remarks>
		/// Admin User can delete any user.
		/// Non admin user an only delete its own user. 
		/// </remarks>
		/// <param name="id"></param>
		/// <response code="401">Unauthorized: due to user not token not authorized or the request is not available to user role</response> 
		/// <returns>null</returns>
		// DELETE: api/User/5
		public async Task<IHttpActionResult> Delete(string id)
        {
	        var currentUser = await _authController.GetVerifiedUser(Request.Headers.Authorization);
	        if (currentUser is null)
	        {
		        throw new HttpResponseException(HttpStatusCode.Unauthorized);
	        }

	        if (currentUser.Id != id && currentUser.Role != BabyMemoryConstants.AdminUserRole)
	        {
		        throw new HttpResponseException(HttpStatusCode.Unauthorized);
	        }

	        foreach (var babyId in currentUser.BabyIds)
	        {
		        await _babiesController.Delete(babyId);
	        }

			_context.Delete<User>(id);

	        return StatusCode(HttpStatusCode.NoContent); // HttpStatusCode.NoContent;
        }


	    private Dictionary<string, object> ResponseDictionary(User user)
	    {
		    Dictionary<string, object> metadata = new Dictionary<string, object>();

		    metadata.Add("user", user);
		    metadata.Add("url", Url.Route("DefaultApi", new { controller = "user"}));

		    var babyurls = new List<string>();
		    babyurls.AddRange(user.BabyIds?.Select(userBabyId =>
			    Url.Route("DefaultApi", new { controller = "babies", id = userBabyId })));
		    metadata.Add("baby_urls", babyurls);
		    return metadata;
	    }
	}
}
