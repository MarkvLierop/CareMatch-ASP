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

        // zorgt voor de index pagina
        public ActionResult Index(string gebruikersnaam, string wachtwoord)
        {
            Gebruiker gebruiker = new Gebruiker();
            gebruiker = carematch.database.GebruikerLogin(gebruikersnaam, wachtwoord);

            // Kijken of het form gesubmit is.
            if (Request.Form.Count > 0)
            {
                if (gebruiker == null)
                {
                    ViewBag.foutmelding = "Gebruikersnaam of Wachtwoord is incorrect";
                }
                else if (gebruiker.Rol.ToLower() == "beheerder")
                {
                    Session["Gebruiker"] = gebruiker;
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
            }

            return View();
        }

        // geeft de registratieform
        public ActionResult Registreren()
        {
            return View();
        }

        // registreerd een nieuwe gebruiker
        [HttpPost]
        public ActionResult Registreren(string optionsRadios, string Gebruikersnaam, string Wachtwoord, string Voornaam, string Tussenvoegsel, string Achternaam, string Geslacht, string Geboortedatum, string pasfoto, string VOG, string Bevestig)
        {
            DateTime geboortedatum1 = Convert.ToDateTime(Geboortedatum);
            
            if (Request.Form.Count > 0 && Wachtwoord == Bevestig)
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
                ViewBag.foutmelding = "Wachtwoorden komen niet overeen.";
                return View();
            }
        }

        // logd de gebruiker uit en zet de session op NULL
        public ActionResult Uitloggen()
        {
            Session["Gebruiker"] = null;
            return RedirectToAction("Index");
        }
    }
}
