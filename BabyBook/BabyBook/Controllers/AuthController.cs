using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
//using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using Amazon.DynamoDBv2.DataModel;
using BabyBook.Models;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Mvc;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Plus.v1;
using Google.Apis.Services;
using Newtonsoft.Json;

namespace BabyBook.Controllers
{
	public class AuthController : Controller
	{
		private IDynamoDBContext _context;

		public AuthController() : this (new BabyContext())
		{

		}

		private AuthController(IDynamoDBContext context)
		{
			_context = context;
		}

		public async Task<User> GetVerifiedUser(AuthenticationHeaderValue token)
		{
			var email = await GetVerifiedEmail(token);

			var users = _context.Query<User>(email.ToLower(), new DynamoDBOperationConfig {IndexName = "UserEmailIndex"}).ToList();

			return users.Count > 0 ? users[0] : null;
		}

		public async Task<string> GetVerifiedEmail(AuthenticationHeaderValue token)
		{
			string plusData;
			try
			{
				plusData = await GetAsyncPlusDataTask(token);
			}
			catch (Exception e)
			{
				//log this somewhere
				throw new HttpResponseException(HttpStatusCode.Unauthorized);
			}
			

			dynamic plusJsonData = JsonConvert.DeserializeObject(plusData);
			var email = plusJsonData.emails[0].value;

			if (email == null)
			{
				throw new HttpResponseException(HttpStatusCode.Unauthorized);
			}
			return email;
		}

		private async Task<string> GetAsyncPlusDataTask(AuthenticationHeaderValue token)
		{
			var uri = "https://www.googleapis.com/plus/v1/people/me";

			/*
			
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
			//request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

			request.Headers = new WebHeaderCollection() {{"Authorization", token.Scheme + " " + token.Parameter } };

			using (HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync())
			using (Stream stream = response.GetResponseStream())
			using (StreamReader reader = new StreamReader(stream))
			{
				return await reader.ReadToEndAsync();
			}
			*/
			
			HttpClient hc = new HttpClient();
			hc.DefaultRequestHeaders.Authorization = token;
			Task<Stream> result = hc.GetStreamAsync(uri);

			Stream vs = await result;
			StreamReader am = new StreamReader(vs);

			return await am.ReadToEndAsync();
			
		}

		public async Task<User> AuthAsync(CancellationToken cancellationToken, string accessToken)
		{
			/*
			var flow = new AppFlowMetadata().Flow;
			var clientSecrets = new ClientSecrets
			{
				ClientId = ConfigurationManager.AppSettings["client_id"],
				ClientSecret = ConfigurationManager.AppSettings["secret"]
			};
			Task<UserCredential> credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
				clientSecrets,
				new[] { "email" },
				"user",
				CancellationToken.None,
				
				);
			//UserCredential credential = new UserCredential(flow, "me", tokenResponse);
			//var result = await new AuthorizationCodeMvcApp(this, new AppFlowMetadata()).
			//	AuthorizeAsync(cancellationToken);

			if (await credential == null) return null;

			var service = new PlusService(new BaseClientService.Initializer
			{
				HttpClientInitializer = credential.Result,
				ApplicationName = "Baby Memory"
			});
			
			
			var profile = await service.People.Get("me").ExecuteAsync(cancellationToken);

			if (profile?.Emails == null) return null;

			var users = _context.Query<User>(profile.Emails[0].Value,
				new DynamoDBOperationConfig { IndexName = "UserEmailIndex" }).ToList();

			//var uc = new UserController();
			
			//var users = uc.Get(profile.Emails[0].Value).ToList();

			return users.Count() == 1 ? users.First() : null;
		*/
			return null;
		}
		

	}
}