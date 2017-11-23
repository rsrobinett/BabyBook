using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

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
	}
}
