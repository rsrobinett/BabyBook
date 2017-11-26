using System;
using System.Configuration;
using BabyBook.Controllers;
using BabyBook.Identity;
using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.DataHandler.Encoder;
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
			
			var issuer = ConfigurationManager.AppSettings["issuer"];
			var secret = TextEncodings.Base64Url.Decode(ConfigurationManager.AppSettings["secret"]);

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