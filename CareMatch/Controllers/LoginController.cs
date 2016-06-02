using CAREMATCH;
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
        Database database = new Database();
        public ActionResult Index(string gebruikersnaam, string wachtwoord)
        {
            if (string.IsNullOrEmpty(gebruikersnaam) && string.IsNullOrEmpty(wachtwoord))
            {
                ViewBag.foutmelding = "";
            }
            else if(database.GebruikerLogin(gebruikersnaam, wachtwoord) == null)
            {
                ViewBag.foutmelding = "Gebruikersnaam of Wachtwoord is incorrect";
            }
            else if(database.GebruikerLogin(gebruikersnaam, wachtwoord).Rol.ToLower() == "beheerder")
            {
                Session["Gebruiker"] = database.GebruikerLogin(gebruikersnaam, wachtwoord);
                return RedirectToAction("Index", "Beheerder", new { area = "" });
            }
            else if(database.GebruikerLogin(gebruikersnaam, wachtwoord).Rol.ToLower() == "vrijwilliger")
            {
                Session["Gebruiker"] = database.GebruikerLogin(gebruikersnaam, wachtwoord);
                return RedirectToAction("Index", "Vrijwilliger", new { area = "" });
            }
            else if(database.GebruikerLogin(gebruikersnaam, wachtwoord).Rol.ToLower() == "hulpbehoevende")
            {
                Session["Gebruiker"] = database.GebruikerLogin(gebruikersnaam, wachtwoord);
                return RedirectToAction("Index", "Hulpbehoevende", new { area = "" });
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
                bool success = database.GebruikerAccountToevoegen(Gebruikersnaam, Wachtwoord, optionsRadios, pasfoto, VOG, Voornaam, Tussenvoegsel, Achternaam, Geslacht, geboortedatum1);
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
