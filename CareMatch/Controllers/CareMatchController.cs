using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CareMatch.Controllers
{
    public class CareMatchController : Controller
    {
        // GET: CareMatch
        public ActionResult index()
        {
            return View();
        }
        public ActionResult vrijwilliger()
        {
            return View();
        }
        public ActionResult hulpbehoevende()
        {
            return View();
        }
        public ActionResult videochat()
        {
            return View();
        }
    }
}