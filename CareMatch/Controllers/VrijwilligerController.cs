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

        public ActionResult ChatBarcode()
        {
            return View();
        }

        public ActionResult AgendaPuntToevoegen(DateTime datum, DateTime van, DateTime tot, string titel, string inhoud)
        {
            if (Request.Form.Count > 0)
            {
                Agenda.AgendaPunt agenda = new Agenda.AgendaPunt();
                agenda.AfspraakDatum = datum;
                agenda.DatumTijdEind = tot;
                agenda.DatumTijdStart = van;
                //carematch.database.AgendaPuntToevoegen();
            }
            return View();
        }

        public ActionResult AgendaOverzicht()
        {
            return View();
        }
        public ActionResult Profiel(string wachtwoord, string pasfoto, string info, bool? auto)
        {
            if (Request.Form.Count >0)
            {
                if (!string.IsNullOrEmpty(wachtwoord))
                {
                    ((Gebruiker)Session["Gebruiker"]).Wachtwoord = wachtwoord;
                    ((Gebruiker)Session["Gebruiker"]).Auto = auto;
                    ((Gebruiker)Session["Gebruiker"]).GebruikerInfo = info;
                    carematch.database.GebruikerProfielAanpassen(Session["Gebruiker"] as Gebruiker, true, false);
                }
                if (!string.IsNullOrEmpty(pasfoto))
                {
                    ((Gebruiker)Session["Gebruiker"]).Pasfoto = pasfoto;
                    ((Gebruiker)Session["Gebruiker"]).Auto = auto;
                    ((Gebruiker)Session["Gebruiker"]).GebruikerInfo = info;
                    carematch.database.GebruikerProfielAanpassen(Session["Gebruiker"] as Gebruiker, false, true);
                }
                else if (!string.IsNullOrEmpty(info))
                {
                    ((Gebruiker)Session["Gebruiker"]).Auto = auto;
                    ((Gebruiker)Session["Gebruiker"]).GebruikerInfo = info;
                    carematch.database.GebruikerProfielAanpassen(Session["Gebruiker"] as Gebruiker, false, false);
                }
            }
            return View();
        }
        public ActionResult HulpvragenOverzicht()
        {
            Gebruiker gebruiker = Session["Gebruiker"] as Gebruiker;
            ViewData["hulpvragen"] = carematch.database.HulpvragenOverzicht(gebruiker, Request.QueryString["filter"]);
            return View();
        }
        public ActionResult Hulpvraag(int id)
        {
            Gebruiker gebruiker = Session["Gebruiker"] as Gebruiker;
            Hulpvraag selectedhulpvraag = null;
            List<Hulpvraag> hulpvragen = carematch.database.HulpvragenOverzicht(gebruiker, "");
            foreach (Hulpvraag hulpvraag in hulpvragen)
            {
                if (hulpvraag.HulpvraagID == id)
                {
                    selectedhulpvraag = hulpvraag;
                }
            }
            ViewData["Hulpvraag"] = selectedhulpvraag;
            return View();
        }
        public ActionResult HulpvraagAannemen(int id)
        {
            Gebruiker gebruiker = Session["Gebruiker"] as Gebruiker;
                carematch.database.HulpvraagAannemen(id, gebruiker.GebruikersID);
                return RedirectToAction("HulpvragenOverzicht", "Vrijwilliger");
        }

        [HttpPost]
        public ActionResult ChatBarcode(string partner)
        {
            return RedirectToAction("ChatBekijken", "Chat", new { partner = partner});
        }
    }
}

