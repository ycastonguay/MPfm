using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace mp4m.org.Controllers
{
    public class RootController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Message = "Welcome to ASP.NET MVC!";            

            return View();
        }

        public ActionResult PageNotFound()
        {
            return View();
        }

        public ActionResult InternalError()
        {
            return View();
        }

        public ActionResult Screenshots()
        {
            return View();
        }

        public ActionResult Download()
        {
            return View();
        }

        public ActionResult Support()
        {
            return View();
        }

        public ActionResult Features()
        {
            return View();
        }

        public ActionResult License()
        {
            return View();
        }
    }
}
