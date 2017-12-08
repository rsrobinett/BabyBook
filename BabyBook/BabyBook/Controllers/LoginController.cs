using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Amazon.DynamoDBv2.DataModel;
using BabyBook.Models;
using Swashbuckle.Swagger.Annotations;

namespace BabyBook.Controllers
{
	/// <summary>
	/// 
	/// </summary>
	public class LoginController : ApiController
	{
		/// <summary>
		/// Controller to login for testing		
		/// </summary>
		public LoginController()
		{

		}

		/// <summary>
		/// Helper api to populate token header in Postman
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		public async Task<IHttpActionResult> Login()
		{
			return Ok(Request.Headers.Authorization);
		}
	}
}
