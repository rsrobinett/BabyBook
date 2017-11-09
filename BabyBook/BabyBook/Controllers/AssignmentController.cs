using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;

namespace BabyBook.Controllers
{
    public class AssignmentController : Controller
    {
	    public ActionResult Index()
	    {
		    ViewBag.Title = "Assignment";

		    return View();
	    }
	}
}
