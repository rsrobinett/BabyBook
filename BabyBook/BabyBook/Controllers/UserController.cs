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
    public class UserController : ApiController
    {
	    private IDynamoDBContext _context;

	    public UserController() : this (new BabyContext())
	    {
		    
	    }

	    private UserController(IDynamoDBContext context)
	    {
		    _context = context;
	    }

		/*
	    // GET: api/User/5
	    public User Get(string id)
	    {
		    return _context.Load<User>(id);
	    }
		*/

		// Get: api/user/email/
		public IEnumerable<User> Get(string email)
	    {
		    return _context.Query<User>(email, 
				new DynamoDBOperationConfig { IndexName = "UserEmailIndex" });

		}

		/*
	    // GET: api/User
		public IEnumerable<User> Get()
        {
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
        public User Post([FromBody]User user)
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
        }*/
	}
}
