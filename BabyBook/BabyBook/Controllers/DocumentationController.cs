using System.Web.Http.Validation;
using System.Web.Mvc;

namespace BabyBook.Controllers
{
    public class DocumentationController : Controller
    {
	    public ActionResult Index()
	    {
		    ViewBag.Title = "Documentation";
		    return View();
	    }
	}
}
