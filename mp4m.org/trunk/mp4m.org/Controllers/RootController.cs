// Copyright © 2011-2013 Yanick Castonguay
//
// This file is part of MPfm.
//
// MPfm is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// MPfm is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with MPfm. If not, see <http://www.gnu.org/licenses/>.

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

        public ActionResult Developers()
        {
            return View();
        }
    }
}
