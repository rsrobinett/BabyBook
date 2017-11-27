using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Mvc;
using Amazon.DynamoDBv2.DataModel;
using BabyBook.Models;
using Newtonsoft.Json;
using Owin;

namespace BabyBook.Controllers
{
    public class UserController : ApiController
    {
	    private IDynamoDBContext _context;
	    private AuthController _authController;

	    public UserController() : this (new BabyContext(), new AuthController())
	    {
		    
	    }

	    private UserController(IDynamoDBContext context, AuthController authController)
	    {
		    _context = context;
		    _authController = authController;
	    }

	
		// Get: api/user/5
	    public async Task<User> Get(string id)
	    {
		    var currentUser = await _authController.GetVerifiedUser(Request.Headers.Authorization);
		    
			var user = _context.Load<User>(id);
		    
			if(currentUser.Id != id && currentUser.Role != BabyMemoryConstants.AdminUserRole)
		    {
				throw new HttpResponseException(HttpStatusCode.BadRequest);
			}

		    return user;
		}
		/*
	    public IEnumerable<User> Get(string id)
	    {
		    return _context.Query<User>(email, 
				new DynamoDBOperationConfig { IndexName = "UserEmailIndex" });

		}
		*/


	    // GET: api/User
		public async Task<IEnumerable<User>> Get()
        {
	        var currentUser = await _authController.GetVerifiedUser(Request.Headers.Authorization);

	        if (currentUser.Role != BabyMemoryConstants.AdminUserRole)
	        {
				throw new HttpResponseException(HttpStatusCode.BadRequest);
			}

			var parameters = Request.GetQueryNameValuePairs();

	        if (!parameters.Any())
	        {
		        return _context.Scan<User>();
	        }

	        var users = new List<User>();

	        foreach (var queryString in parameters)
	        {
		        if (queryString.Key == "email")
		        {
			        var results = _context.Query<User>(queryString.Value, new DynamoDBOperationConfig { IndexName = "UserEmailIndex" });

			        if (results != null)
			        {
				        users.AddRange(results);
			        }
		        }
	        }

	        return users;
		}

		
        // POST: api/User
        public Task<User> Post([FromBody]User user)
        {
	        user.Id = Guid.NewGuid().ToString("N");
			_context.Save<User>(user);
	        return Get(user.Id);
        }

        // PUT: api/User/5
        public void Put(string id, [FromBody]string value)
        {
        }

        // DELETE: api/User/5
        public void Delete(string id)
        {
        }
	}
}
