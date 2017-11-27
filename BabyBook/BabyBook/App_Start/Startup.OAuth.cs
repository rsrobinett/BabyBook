using System;
using System.Configuration;
using System.Security.Claims;
using BabyBook.Controllers;
using Google.Apis.Auth.OAuth2.Responses;
using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.DataHandler;
using Microsoft.Owin.Security.DataHandler.Encoder;
using Microsoft.Owin.Security.Google;
using Microsoft.Owin.Security.Infrastructure;
using Microsoft.Owin.Security.Jwt;
using Microsoft.Owin.Security.OAuth;
using Owin;

namespace BabyBook
{
	public partial class Startup
	{

		public void ConfigureOAuth(IAppBuilder app)
		{

			app.UseOAuthBearerTokens(new OAuthAuthorizationServerOptions
			{
				ApplicationCanDisplayErrors = true,
				TokenEndpointPath = new PathString(ConfigurationManager.AppSettings["access_token_url"]),
				AuthorizeEndpointPath = new PathString(ConfigurationManager.AppSettings["auth_url"]),
					//AccessTokenProvider = new AuthenticationTokenProvider()
			});
			
			
			//var google = new GoogleOAuth2AuthenticationOptions()
			//{
			//	//AccessType = "offline",     // Request a refresh token.
			//	ClientId = ConfigurationManager.AppSettings["client_id"],
			//	ClientSecret = ConfigurationManager.AppSettings["secret"],
			//	//Provider = new GoogleOAuth2AuthenticationProvider()
			//	//{
			//	//	OnAuthenticated = async context =>
			//	//	{
			//	//		//var userId = context.Id;
			//	//		//context.Identity.AddClaim(new Claim("GoogleUserId", userId));

			//	//		var tokenResponse = new TokenResponse()
			//	//		{
			//	//			AccessToken = context.AccessToken,
			//	//			RefreshToken = context.RefreshToken,
			//	//			ExpiresInSeconds = (long)context.ExpiresIn.Value.TotalSeconds,
			//	//			Issued = DateTime.Now,
			//	//		};

			//	//		//await dataStore.StoreAsync(userId, tokenResponse);
			//	//	},
			//	//},
			//};

			//foreach (var scope in MyRequestedScopes.Scopes)
			//{
				//google.Scope.Add("email");
			//}
			//app.UseGoogleAuthentication(google);

			//google.Scope.Add("email");
			//var issuer = ConfigurationManager.AppSettings["issuer"];
			//var secret = TextEncodings.Base64Url.Decode(ConfigurationManager.AppSettings["secret"]);

			//			/*
			//			app.CreatePerOwinContext(() => new BabyContext());
			//			app.CreatePerOwinContext(() => new LocalClientContext());
			//			*/

			//			app.UseJwtBearerAuthentication(new JwtBearerAuthenticationOptions
			//			{
			//				AuthenticationMode = AuthenticationMode.Active,
			//				AllowedAudiences = new[] { "Any" },
			//				IssuerSecurityTokenProviders = new IIssuerSecurityTokenProvider[]
			//				{
			//					new SymmetricKeyIssuerSecurityTokenProvider(issuer, secret)
			//					//new X509CertificateSecurityTokenProvider(issuer, certificate);
			//				}
			//			});

			//			app.UseOAuthAuthorizationServer(new OAuthAuthorizationServerOptions
			//			{
			//#if DEBUG
			//				AllowInsecureHttp = true,
			//#endif
			//				TokenEndpointPath = new PathString("/oauth2/token"),
			//				AccessTokenExpireTimeSpan = TimeSpan.FromMinutes(30),
			//				Provider = new CustomOAuthProvider(),
			//				AccessTokenFormat = new CustomJwtFormat(issuer),


			//				//OAuthGrantResourceOwnerCredentialsContext
			//			});

		}
	}
}