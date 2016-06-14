﻿using System;
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

        public ActionResult GebruikerAccepteren(int id)
        {
            Gebruiker gebruiker = Session["Gebruiker"] as Gebruiker;
            if (gebruiker.Rol.ToLower() == "beheerder")
            {
                carematch.database.DataUpdateBeheerApproved(id);
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
        public void Download(string gebruiker, string file)
        {
            Response.ContentType = "APPLICATION/OCTET-STREAM";
            System.IO.FileInfo Dfile = new System.IO.FileInfo(@"C:\"+gebruiker+@"\"+file);
            String Header = "Attachment; Filename="+Dfile.FullName;
            Response.AppendHeader("Content-Disposition", Header);
            Response.WriteFile(Dfile.FullName);
            Response.End();
        }
    }
}