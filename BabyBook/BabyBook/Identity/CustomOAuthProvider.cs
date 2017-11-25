using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using BabyBook.Controllers;
using BabyBook.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;

namespace BooksAPI.Identity
{
	public class CustomOAuthProvider : OAuthAuthorizationServerProvider
	{
		public override Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
		{
			context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { "*" });

			/*
			var user = context.OwinContext.Get<BooksContext>().Users.FirstOrDefault(u => u.UserName == context.UserName);
			if (!context.OwinContext.Get<BookUserManager>().CheckPassword(user, context.Password))
			{
				context.SetError("invalid_grant", "The user name or password is incorrect");
				context.Rejected();
				return Task.FromResult<object>(null);
			}
			*/

			IEnumerable<User> user = new UserController().Get(context.UserName);
			
			var ticket = new AuthenticationTicket(SetClaimsIdentity(context, user.First()), new AuthenticationProperties());
			context.Validated(ticket);

			return Task.FromResult<object>(null);
		}

		public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
		{
			context.Validated();
			return Task.FromResult<object>(null);
		}

		private static ClaimsIdentity SetClaimsIdentity(OAuthGrantResourceOwnerCredentialsContext context, User user)
		{
			var identity = new ClaimsIdentity("JWT");
			identity.AddClaim(new Claim(ClaimTypes.Name, context.UserName));
			identity.AddClaim(new Claim("sub", context.UserName));

			/*
			var userRoles = context.OwinContext.Get<BookUserManager>().GetRoles(user.Id);
			foreach (var role in userRoles)
			{
				identity.AddClaim(new Claim(ClaimTypes.Role, role));
			}
			*/
			return identity;
		}
	}
}