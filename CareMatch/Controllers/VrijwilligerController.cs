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

        /// <summary>
        /// voegt een afspraak toe aan de agenda van de gebruiker
        /// </summary>
        /// <param name="datum">de datum van de afspraak</param>
        /// <param name="van">begintijd van de afspraak</param>
        /// <param name="tot">de eindtijd van de afspraak</param>
        /// <param name="titel">de naam van de afspraak</param>
        /// <param name="inhoud">de beschrijving van de afspraak</param>
        /// <returns></returns>
        public ActionResult AgendaPuntToevoegen(DateTime? datum, DateTime? van, DateTime? tot, string titel, string inhoud)
        {
            if (Request.Form.Count > 0)
            {
                Agenda.AgendaPunt agenda = new Agenda.AgendaPunt();
                agenda.AfspraakDatum = datum;
                agenda.DatumTijdEind = tot;
                agenda.DatumTijdStart = van;
                agenda.Beschrijving = inhoud;
                agenda.Titel = titel;
                carematch.database.AgendaPuntToevoegen(agenda,Session["Gebruiker"] as Gebruiker);
            }

            return View();
        }

        /// <summary>
        /// laat het agenda overzicht van de ingelogde gebruiker zien
        /// </summary>
        /// <returns></returns>
        public ActionResult AgendaOverzicht()
        {
            ViewData["AgendaList"] = carematch.database.AgendaOverzicht(Session["Gebruiker"] as Gebruiker);
            return View();
        }

        /// <summary>
        /// laat het profiel zien en past het aan als de bijhorende velden zijn ingevult
        /// </summary>
        /// <param name="wachtwoord"></param>
        /// <param name="pasfoto"></param>
        /// <param name="info"></param>
        /// <param name="auto"></param>
        /// <returns></returns>
        public ActionResult Profiel(string wachtwoord, string pasfoto, string info, bool? auto)
        {
            if (Request.Form.Count > 0)
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

        /// <summary>
        /// laat alle onaangenomen hulpvragen zien
        /// </summary>
        /// <returns></returns>
        public ActionResult HulpvragenOverzicht()
        {
            Gebruiker gebruiker = Session["Gebruiker"] as Gebruiker;
            ViewData["hulpvragen"] = carematch.database.HulpvragenOverzicht(gebruiker, Request.QueryString["filter"]);
            return View();
        }

        /// <summary>
        /// geeft alle info van een geselecteerde hulpvraag
        /// </summary>
        /// <param name="id">de id van de hulpvraag die laten zien moet worden</param>
        /// <returns></returns>
        public ActionResult Hulpvraag(int id)
        {
            Gebruiker gebruiker = Session["Gebruiker"] as Gebruiker;
            Hulpvraag selectedhulpvraag = null;
            List<Hulpvraag> hulpvragen = carematch.database.HulpvragenOverzicht(gebruiker, string.Empty);
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

        /// <summary>
        /// koppelt een hulpvraag met een vrijwilliger zodat de hulpvraag is aangenomen
        /// </summary>
        /// <param name="id">de id van de hulpvraag die aangenomen moet worden</param>
        /// <returns></returns>
        public ActionResult HulpvraagAannemen(int id)
        {
            Gebruiker gebruiker = Session["Gebruiker"] as Gebruiker;
                carematch.database.HulpvraagAannemen(id, gebruiker.GebruikersID);
                return RedirectToAction("HulpvragenOverzicht", "Vrijwilliger");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="Beoordeling"></param>
        /// <returns></returns>
        public ActionResult Beoordelingreactie(int id, string Beoordeling)
        {
            if (!string.IsNullOrEmpty(Beoordeling))
            {
                Hulpvraag hulpvraag = new Hulpvraag();
                hulpvraag.HulpvraagID = id;
                hulpvraag.ReactieBeoordeling = Beoordeling;

                carematch.database.HulpvraagReactieBeoordelingToevoegen(hulpvraag);
                return RedirectToAction("HulpvragenOverzicht");
            }
            else
            {
                return View();
            }
        }

        public ActionResult Rapporteer(int id)
        {
            Hulpvraag hulpvraag = new Hulpvraag();
            hulpvraag.HulpvraagID = id;
            carematch.database.HulpvraagRapporteer(hulpvraag);
            return RedirectToAction("HulpvragenOverzicht");
        }

        [HttpPost]
        public ActionResult ChatBarcode(string partner)
        {
            return RedirectToAction("ChatBekijken", "Chat", new { partner = partner });
        }
    }
}
