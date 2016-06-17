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

        // laat een lijst met alle ongepaste hulpvragen zien
        public ActionResult HulpvragenOverzicht()
        {
            Gebruiker gebruiker = Session["Gebruiker"] as Gebruiker;
            ViewData["hulpvragen"] = carematch.database.HulpvragenOverzicht(gebruiker, string.Empty);
            return View();
        }

        /// <summary>
        /// laat een hulpvraag zien
        /// </summary>
        /// <param name="id">de id van de te laten zien hulpvraag</param>
        /// <returns></returns>
        public ActionResult Hulpvraag(int id)
        {
            Hulpvraag gekozenHulpvraag = null;
            foreach (Hulpvraag hulpvraag in carematch.database.HulpvragenOverzicht(Session["gebruiker"] as Gebruiker, string.Empty))
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

        /// <summary>
        /// laat een overzicht van accounts zien met filter
        /// </summary>
        /// <param name="id">het filter: 0 = alles, 1 = alleen vrijwilligers, 2 = alleen hulpbehoevende, 3 = alleen ongekeurde gebruikers, 4 = vrijwilligers zonder VOG</param>
        /// <returns></returns>
        public ActionResult AccountOverzicht(int id = 0)
        {
            List<Gebruiker> gebruikerslist = new List<Gebruiker>();
            switch (id)
            {
                case 3:
                    gebruikerslist = carematch.database.GebruikerBeheer("Niet goedgekeurde gebruikers");
                    ViewBag.GebruikerList = gebruikerslist;
                    ViewBag.filter = "Niet goedgekeurde vrijwilligers";
                    break;
                case 4:
                    gebruikerslist = carematch.database.GebruikerBeheer("Vrijwilligers zonder VOG");
                    ViewBag.GebruikerList = gebruikerslist;
                    ViewBag.filter = "Vrijwilligers zonder VOG";
                    break;
                case 1:
                    gebruikerslist = carematch.database.GebruikerBeheer("Vrijwilligers");
                    ViewBag.GebruikerList = gebruikerslist;
                    ViewBag.filter = "Vrijwilligers";
                    break;
                case 2:
                    gebruikerslist = carematch.database.GebruikerBeheer("Hulpbehoevenden");
                    ViewBag.GebruikerList = gebruikerslist;
                    ViewBag.filter = "Hulpbehoevende";
                    break;
                default:
                    gebruikerslist = carematch.database.GebruikerBeheer("Alles");
                    ViewBag.GebruikerList = gebruikerslist;
                    ViewBag.filter = "Alle gebruikers";
                    break;
            }

            return View();
        }

        /// <summary>
        /// geeft een overzicht van een gebruikeraccount
        /// </summary>
        /// <param name="id">de id van de gebruiker</param>
        /// <returns></returns>
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

        /// <summary>
        /// roept de query in de database aan die een gebruiker in een vrijwilliger veranderd
        /// </summary>
        /// <param name="id">de id die bij de gebruiker hoort</param>
        /// <returns></returns>
        public ActionResult GebruikerSetBeheerder(int id)
        {
            Gebruiker gebruiker = Session["Gebruiker"] as Gebruiker;
            if (gebruiker.Rol.ToLower() == "beheerder")
            {   
                carematch.database.DataUpdateBeheerRol(id);
            }

            return RedirectToAction("AccountOverzicht", "Beheerder");
        }

        /// <summary>
        /// accepteerd een ongeaccepteerde gebruiker op basis van id 
        /// </summary>
        /// <param name="id">de id van de te accepteren gebruiker</param>
        /// <returns></returns>
        public ActionResult GebruikerAccepteren(int id)
        {
            carematch.database.DataUpdateBeheerApproved(id);

            return RedirectToAction("AccountOverzicht", "Beheerder");
        }

        /// <summary>
        /// verwijdert een gebruiker op basis van id
        /// </summary>
        /// <param name="id">id van de gebruiker</param>
        /// <returns></returns>
        public ActionResult GebruikerVerwijderen(int id)
        {
            Gebruiker gebruiker = Session["Gebruiker"] as Gebruiker;
            if (gebruiker.Rol.ToLower() == "beheerder")
            {
                carematch.database.VerwijderGebruiker(id);
            }

            return RedirectToAction("AccountOverzicht", "Beheerder");
        }
        
        /// <summary>
        /// verandert het wachtwoord van de gebruiker en laat daarna het accountovezicht zien
        /// </summary>
        /// <param name="gebruikerid">het id van de gebruiker</param>
        /// <param name="wachtwoord">het nieuwe wachtwoord voor de gebruiker</param>
        /// <returns></returns>
        [HttpPost]
        //public ActionResult AccountOverzicht(int gebruikerid)
        //{
        //    carematch.database.ResetWachtwoord(gebruikerid);
        //    return RedirectToAction("AccountOverzicht", "Beheerder");
        //}

        /// <summary>
        /// verwijdert een hulpvraag op basis van id
        /// </summary>
        /// <param name="id">de id van de hulpvraag</param>
        /// <returns></returns>
        public ActionResult HulpvraagVerwijderen(int id)
        {
            Gebruiker gebruiker = Session["Gebruiker"] as Gebruiker;
            if (gebruiker.Rol.ToLower() == "beheerder")
            {
                carematch.database.HulpvraagVerwijderen(id);
            }

            return RedirectToAction("HulpvragenOverzicht", "Beheerder");
        }

        /// <summary>
        /// een functie voor het downloaden van bestanden
        /// </summary>
        /// <param name="gebruiker">de gebruikerfolder waar het bestand in staat, is gelijk aan gebruikersnaam</param>
        /// <param name="file">het specefieke bestand dat gedownload moet worden</param>
        public void DownloadFile(string gebruiker, string file)
        {
            Response.ContentType = "APPLICATION/OCTET-STREAM";
            System.IO.FileInfo Dfile = new System.IO.FileInfo(@"C:\" + gebruiker + @"\" + file);
            string Header = "Attachment; Filename=" + Dfile.FullName;
            Response.AppendHeader("Content-Disposition", Header);
            Response.WriteFile(Dfile.FullName);
            Response.End();
        }

        /// <summary>
        /// verwijdert een beoordeling van een hulpvraag op basis van hulpvraag id
        /// </summary>
        /// <param name="id">de hulpvraag id</param>
        /// <returns></returns>
        public ActionResult BeoordelingVerwijderen(int id)
        {
            Hulpvraag hulpvraag = new Models.Hulpvraag();
            hulpvraag.HulpvraagID = id;

            carematch.database.BeoordelingVerwijderen(hulpvraag);
            return RedirectToAction("HulpvraagDeRapporteren", "Beheerder", new { id = id });
        }

        /// <summary>
        /// Als een hulpvraag gerapporteerd is derapporteer deze op basis van id
        /// </summary>
        /// <param name="id">de id van de hulpvraag</param>
        /// <returns></returns>
        public ActionResult HulpvraagDerapporteren(int id)
        {
            Hulpvraag hulpvraag = new Models.Hulpvraag();
            hulpvraag.HulpvraagID = id;

            carematch.database.HulpvraagDerapporteer(hulpvraag);

            return RedirectToAction("HulpvragenOverzicht", "Beheerder");
        }

        public ActionResult ResetWatchtwoord(int gebruikerid)
        {
            if ((Session["Gebruiker"] as Gebruiker).Rol == "beheerder")
            {
                carematch.database.ResetWachtwoord(gebruikerid);
            }
            return View();
        }
    }
}