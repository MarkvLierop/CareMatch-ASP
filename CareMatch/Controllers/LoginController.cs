using CareMatch.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CareMatch.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        CareMatch1 carematch = new CareMatch1();
        public ActionResult Index(string gebruikersnaam, string wachtwoord)
        {
            Gebruiker gebruiker = new Gebruiker();
            gebruiker = carematch.database.GebruikerLogin(gebruikersnaam, wachtwoord);

            if (gebruiker == null)
            {
                ViewBag.foutmelding = "Gebruikersnaam of Wachtwoord is incorrect";
            }
            else if (gebruiker.Rol.ToLower() == "beheerder")
            {
                return RedirectToAction("Index", "Beheerder", new { area = string.Empty });
            }
            else if (gebruiker.Rol.ToLower() == "vrijwilliger")
            {
                if (gebruiker.Approved)
                {
                    Session["Gebruiker"] = gebruiker;
                    return RedirectToAction("Index", "Vrijwilliger", new { area = string.Empty });
                }
                else
                {
                    ViewBag.foutmelding = "Account is nog niet goedgekeurd";
                }
            }
            else if (gebruiker.Rol.ToLower() == "hulpbehoevende")
            {
                Session["Gebruiker"] = gebruiker;
                return RedirectToAction("Index", "Hulpbehoevende", new { area = string.Empty });
            }

            return View();
        }

        //commit
        public ActionResult Registreren()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Registreren(string optionsRadios, string Gebruikersnaam, string Wachtwoord, string Voornaam, string Tussenvoegsel, string Achternaam, string Geslacht, string Geboortedatum, string pasfoto, string VOG)
        {
            DateTime geboortedatum1 = Convert.ToDateTime(Geboortedatum);
            
            if (!string.IsNullOrEmpty(Voornaam))
            {
                bool success = carematch.database.GebruikerAccountToevoegen(Gebruikersnaam, Wachtwoord, optionsRadios, pasfoto, VOG, Voornaam, Tussenvoegsel, Achternaam, Geslacht, geboortedatum1);
                if (success)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    return View();
                }
            }
            else
            {
                return View();
            }
        }

        public ActionResult Uitloggen()
        {
            Session["Gebruiker"] = null;
            return RedirectToAction("Index");
        }
    }
}
