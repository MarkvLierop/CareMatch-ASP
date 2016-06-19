using CareMatch.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CareMatch.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        CareMatch1 carematch = new CareMatch1();
        string tpasfoto;
        string tvog;

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
        public ActionResult Registreren(string optionsRadios, string Gebruikersnaam, string Wachtwoord, string Voornaam, string Tussenvoegsel, string Achternaam, string Geslacht, string Geboortedatum, HttpPostedFileBase pasfoto, HttpPostedFileBase vog, string Bevestig)
        {
            DateTime geboortedatum1 = Convert.ToDateTime(Geboortedatum);
            if (Request.Form.Count > 0 && Wachtwoord == Bevestig)
            {
                if (pasfoto != null)
                {
                    if(pasfoto.ContentLength > 0)
                    {
                        // Bestandsinfo opvragen
                        System.IO.FileInfo Dfile = new System.IO.FileInfo(pasfoto.FileName);

                        // Linkt naar CareMatch-ASP\CareMatch\Bestanden\pasfoto
                        var path = Path.Combine(Server.MapPath("~/Fotos"), Gebruikersnaam + Dfile.Extension);
                        pasfoto.SaveAs(path);
                        tpasfoto = Dfile.Extension;
                    }
                }
                if (vog != null)
                {
                    if(vog.ContentLength > 0)
                    {
                        // Bestandsinfo opvragen
                        System.IO.FileInfo Dfile = new System.IO.FileInfo(vog.FileName);

                        // Linkt naar CareMatch-ASP\CareMatch\Bestanden\pasfoto
                        var path = Path.Combine(Server.MapPath("~/VOG"), Gebruikersnaam + Dfile.Extension);
                        vog.SaveAs(path);
                        tvog = Dfile.Extension;
                    }
                }

                bool success = carematch.database.GebruikerAccountToevoegen(Gebruikersnaam, Wachtwoord, optionsRadios, Gebruikersnaam + tpasfoto, Gebruikersnaam + tvog, Voornaam, Tussenvoegsel, Achternaam, Geslacht, geboortedatum1);

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
