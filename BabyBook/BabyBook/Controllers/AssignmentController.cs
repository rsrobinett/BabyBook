using System.Web.Mvc;

namespace BabyBook.Controllers
{
    public class AssignmentController : Controller
    {
	    [AllowAnonymous]
		public ActionResult Index()
	    {
		    ViewBag.Title = "Assignment";

		    return View();
	    }
	}
}
