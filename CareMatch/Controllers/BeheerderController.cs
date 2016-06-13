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

        public ActionResult Hulpvraag(int id)
        {
            Hulpvraag gekozenHulpvraag = null;
            string filter = "ongepaste hulpvragen";
            foreach (Hulpvraag hulpvraag in carematch.database.HulpvragenOverzicht(Session["gebruiker"] as Gebruiker, filter))
            {
                if(hulpvraag.HulpvraagID == id)
                {
                    gekozenHulpvraag = hulpvraag;
                }
            }
            if(gekozenHulpvraag != null)
            {
                ViewBag.Hulpvraag = gekozenHulpvraag;
            }
            
            return View();
        }

        public ActionResult AccountOverzicht(string id)
        {
            //zorg dat id "Alles", "Naam & Wachtwoord","Niet goedgekeurde gebruikers" of "Vrijwilligers zonder VOG" is anders werkt het niet, want goede database...
            ViewBag.UserList = carematch.database.GebruikerBeheer(id);
            ViewBag.CurrentView = id;
            return View();
        }
        public ActionResult UserAccount(int id)
        {
            Gebruiker user = null;
            foreach(Gebruiker gebruiker in carematch.database.GebruikerBeheer("Alles"))
            {
                if(gebruiker.GebruikersID == id)
                {
                    user = gebruiker;
                }
            }
            ViewBag.ShowUser = user;
            return View();
        }

        public ActionResult HulpvraagVerwijderen(int id)
        {
            Gebruiker gebruiker = Session["Gebruiker"] as Gebruiker;
            if(gebruiker.Rol == "beheerder")
            {
                carematch.database.HulpvraagVerwijderen(id);
            }
            return RedirectToAction("HulpvragenOverzicht", "Beheerder");
        }


    }
}