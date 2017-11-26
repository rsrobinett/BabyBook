using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Mvc;
using Google.Apis.Util.Store;
using Microsoft.Owin.Security.DataHandler.Encoder;

namespace BabyBook
{
	public class AppFlowMetadata : FlowMetadata
	{
		private static readonly IAuthorizationCodeFlow flow =
			new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
			{
				ClientSecrets = new ClientSecrets
				{
					ClientId = ConfigurationManager.AppSettings["client_id"],
					ClientSecret = ConfigurationManager.AppSettings["secret"]
				},
				Scopes = new[] {"email"},
				//DataStore = new FileDataStore("Plus.Api.Auth.Store")
				
			});

		/*
		public override string GetUserId(Controller controller)
		{
			// In this sample we use the session to store the user identifiers.
			// That's not the best practice, because you should have a logic to identify
			// a user. You might want to use "OpenID Connect".
			// You can read more about the protocol in the following link:
			// https://developers.google.com/accounts/docs/OAuth2Login.

			/*
			var user = controller.Session["user"];
			if (user == null)
			{
				user = Guid.NewGuid();
				controller.Session["user"] = user;
			}
			return user.ToString();
			
		
		}
		*/
		public override string GetUserId(Controller controller)
		{
			//state should never be stored
			//throw new NotImplementedException();
			//return Guid.NewGuid().ToString();

			var user = controller.Session["user"];
			if (user == null)
			{
				user = Guid.NewGuid();
				controller.Session["user"] = user;
			}
			return user.ToString();
		}

		public override IAuthorizationCodeFlow Flow
		{
			get { return flow; }
		}
	}
}