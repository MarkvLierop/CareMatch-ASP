using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CareMatch.Models;

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
            Gebruiker gebruiker = Session["Gebruiker"] as Gebruiker;
            ViewData["hulpvragen"] = carematch.database.HulpvragenOverzicht(gebruiker, "");
            return View();
        }
        public ActionResult HulpvraagIndienen()
        {
            return View();
        }
    }
}