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

        // laat een lijst met alle ongepaste hulpvragen zien
        {
            Gebruiker gebruiker = Session["Gebruiker"] as Gebruiker;
            ViewData["hulpvragen"] = carematch.database.HulpvragenOverzicht(gebruiker, "");
            return View();
        }

        public ActionResult Hulpvraag(int id)
        {
            Hulpvraag gekozenHulpvraag = null;
            foreach (Hulpvraag hulpvraag in carematch.database.HulpvragenOverzicht(Session["gebruiker"] as Gebruiker, ""))
            {
                if (hulpvraag.HulpvraagID == id)
                {
                    gekozenHulpvraag = hulpvraag;
                }
            }

            if (gekozenHulpvraag != null)
            {
                ViewBag.Hulpvraag = gekozenHulpvraag;
            }
            
            return View();
        }

        public ActionResult AccountOverzicht(int id = 0)
        {
            switch (id)
            {
                case 3:
                    ViewBag.GebruikerList = carematch.database.GebruikerBeheer("Niet goedgekeurde gebruikers");
                    ViewBag.filter = "Niet goedgekeurde gebruikers";
                    break;
                case 4:
                    ViewBag.GebruikerList = carematch.database.GebruikerBeheer("Vrijwilligers zonder VOG");
                    ViewBag.filter = "Vrijwilligers zonder VOG";
                    break;
                case 1:
                    ViewBag.GebruikerList = carematch.database.GebruikerBeheer("Vrijwilligers");
                    ViewBag.filter = "Vrijwilligers";
                    break;
                case 2:
                    ViewBag.GebruikerList = carematch.database.GebruikerBeheer("Hulpbehoevenden");
                    ViewBag.filter = "Hulpbehoevenden";
                    break;
                default:
                    ViewBag.GebruikerList = carematch.database.GebruikerBeheer("Alles");
                    ViewBag.filter = "Alles";
                    break;
            }

            return View();
        }

        public ActionResult GebruikerAccount(int id)
        {
            List<Gebruiker> gebruikerList = carematch.database.GebruikerBeheer("Alles");
            Gebruiker returnGebruiker = null;
            foreach (Gebruiker gebruiker in gebruikerList)
            {
                if (gebruiker.GebruikersID == id)
                {
                    returnGebruiker = gebruiker;
                }
            }

            if (returnGebruiker != null)
            {
                ViewBag.gebruikerBool = true;
                ViewBag.Gebruiker = returnGebruiker;
                return View();
            }

            ViewBag.gebruikerBool = false;
            return View();
        }

        public ActionResult GebruikerSetBeheerder(int accountID)
        {
            Gebruiker gebruiker = Session["Gebruiker"] as Gebruiker;
            if (gebruiker.Rol.ToLower() == "beheerder")
            {   

                carematch.database.DataUpdateBeheerRol(accountID);
            }

            return RedirectToAction("AccountOverzicht", "Beheerder");
        }

        public ActionResult GebruikerAccepteren(int accountID)
        {
            Gebruiker gebruiker = Session["Gebruiker"] as Gebruiker;
            if (gebruiker.Rol.ToLower() == "beheerder")
            {
                carematch.database.DataUpdateBeheerApproved(accountID);
            }

            return RedirectToAction("AccountOverzicht", "Beheerder");
        }

        public ActionResult GebruikerVerwijderen(int id)
        {
            Gebruiker gebruiker = Session["Gebruiker"] as Gebruiker;
            if (gebruiker.Rol.ToLower() == "beheerder")
            {
                carematch.database.VerwijderGebruiker(id);
            }

            return RedirectToAction("AccountOverzicht", "Beheerder");
        }

        public ActionResult HulpvraagVerwijderen(int id)
        {
            Gebruiker gebruiker = Session["Gebruiker"] as Gebruiker;
            if (gebruiker.Rol.ToLower() == "beheerder")
            {
                carematch.database.HulpvraagVerwijderen(id);
            }

            return RedirectToAction("HulpvragenOverzicht", "Beheerder");
        }
    }
}