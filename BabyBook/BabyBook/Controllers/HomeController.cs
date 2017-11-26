using System.Threading;
using System.Web.Mvc;
using Google.Apis.Auth.OAuth2.Mvc;
using Google.Apis.Services;
using System.Threading.Tasks;
using Google.Apis.Plus.v1;

namespace BabyBook.Controllers
{
	public class HomeController : Controller
	{
		public ActionResult Index()
		{
			ViewBag.Title = "Home Page";

			return View();
		}
		public ActionResult TermsAndConditions()
		{
			ViewBag.Title = "Terms And Conditions";

			return View();
		}

		public ActionResult PrivacyPolicy()
		{
			ViewBag.Title = "Privacy Policy";

			return View();
		}

		public async Task<ActionResult> IndexAsync(CancellationToken cancellationToken)
		{
			ViewBag.Title = "Authorization Verification";

			var result = await new AuthorizationCodeMvcApp(this, new AppFlowMetadata()).
				AuthorizeAsync(cancellationToken);

			if (result.Credential != null)
			{
				var service = new PlusService(new BaseClientService.Initializer
				{
					HttpClientInitializer = result.Credential,
					ApplicationName = "Baby Memory"
				});

				var profile = await service.People.Get("me").ExecuteAsync();

				if (profile != null && profile.Emails != null)
				{
					ViewBag.Message = "User Email Is: " + profile.Emails[0].Value;
				}
				else
				{
					ViewBag.Message = "Invalid user";
				}
				return View();
			}
			else
			{
				return new RedirectResult(result.RedirectUri);
			}
		}
	}
}
