using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CareMatch.Models;

namespace CareMatch.Controllers
{
    public class VrijwilligerController : Controller
    {
        CareMatch1 carematch = new CareMatch1();
        // GET: Vrijwilliger
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
        public ActionResult Hulpvraag(int id)
        {
            Gebruiker gebruiker = Session["Gebruiker"] as Gebruiker;
            Hulpvraag selectedhulpvraag = null;
            List<Hulpvraag> hulpvragen = carematch.database.HulpvragenOverzicht(gebruiker, "");
            foreach(Hulpvraag hulpvraag in hulpvragen)
            {
                if(hulpvraag.HulpvraagID == id)
                {
                    selectedhulpvraag = hulpvraag;
                }
            }
            ViewData["Hulpvraag"] = selectedhulpvraag;
            return View();
        }
    }
}