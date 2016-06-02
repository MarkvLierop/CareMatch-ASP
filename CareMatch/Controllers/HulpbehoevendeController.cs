using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CAREMATCH;

namespace CareMatch.Controllers
{
    public class HulpbehoevendeController : Controller
    {
        CareMatch1 carematch = new CareMatch1();
        // GET: Hulpbehoevende
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult HulpvragenOverzicht()
        {
            return View();
        }
    }
}