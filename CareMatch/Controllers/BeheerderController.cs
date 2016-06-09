using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CareMatch.Models;

namespace CareMatch.Controllers
{
    public class BeheerderController : Controller
    {
        CareMatch1 carematch = new CareMatch1();
        // GET: Beheerder
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult HulpvragenOverzicht()
        //laat een lijst met alle ongepaste hulpvragen zien
        {
            string filter = "ongepaste hulpvragen";
            ViewBag.hulpvraaglist = carematch.database.HulpvragenOverzicht(Session["gebruiker"] as Gebruiker, filter);
            return View();
        }

        public ActionResult Hulpvraag()
        {
            return View();
        }

        public ActionResult AccountOverzicht()
        {
            return View();
        }
        public ActionResult Chat()
        {
            return View();
        }


    }
}