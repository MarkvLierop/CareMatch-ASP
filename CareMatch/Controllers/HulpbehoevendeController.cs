using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CareMatch.Models;

namespace CareMatch.Controllers
{
    public class HulpbehoevendeController : Controller
    {
        CareMatch1 carematch = new CareMatch1();

        // GET: Hulpbehoevende
        public ActionResult Index()
        {
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

                hulpvraag.EindDatum = DateTime.Parse(Convert.ToString(hulpvraag.EindDatum - hulpvraag.StartDatum));
            }

            ViewData["Hulpvraag"] = selectedhulpvraag;
            ViewData["Vrijwilliger"] = carematch.database.GebruikerInfoOpvragen(selectedhulpvraag.Vrijwilliger);
            return View();
        }

        public ActionResult HulpvraagWijzigen(int id, string Urgent, string Auto, DateTime? Datum, TimeSpan? Duur, TimeSpan? Tijd, string Plaatsnaam, string StraatEnHuisnummer, string KOmschrijving, string Omschrijving)
        {
            if (!string.IsNullOrEmpty(Omschrijving))
            {
                Hulpvraag hulpvraag = new Hulpvraag();
                if (!string.IsNullOrEmpty(Urgent))
                {
                    hulpvraag.Urgent = true;
                }
                else
                {
                    hulpvraag.Urgent = false;
                }

                if (!string.IsNullOrEmpty(Auto))
                {
                    hulpvraag.Auto = true;
                }
                else
                {
                    hulpvraag.Auto = false;
                }

                hulpvraag.HulpvraagID = id;
                hulpvraag.StartDatum = Datum + Tijd;
                hulpvraag.EindDatum = hulpvraag.StartDatum + Duur;
                hulpvraag.Locatie = StraatEnHuisnummer;
                hulpvraag.Titel = KOmschrijving;
                hulpvraag.HulpvraagInhoud = Omschrijving;
                hulpvraag.Hulpbehoevende = (Session["Gebruiker"] as Gebruiker).Gebruikersnaam;
                hulpvraag.Plaatsnaam = Plaatsnaam;
                carematch.database.HulpvraagAanpassen((Session["Gebruiker"] as Gebruiker), hulpvraag);

                return RedirectToAction("HulpvragenOverzicht");
            }

            return View();
        }

        public ActionResult HulpvraagBeoordelen(int id, int Cijfer, string Beoordeling)
        {
            if (!string.IsNullOrEmpty(Beoordeling))
            {
                Hulpvraag hulpvraag = new Hulpvraag();
                hulpvraag.HulpvraagID = id;
                hulpvraag.Cijfer = Cijfer.ToString();
                hulpvraag.Beoordeling = Beoordeling;

                carematch.database.HulpvraagBeoordelingToevoegen(hulpvraag);
                return RedirectToAction("HulpvragenOverzicht");
            }
            else
            {
                return View();
            }
            
        }
        public ActionResult HulpvraagIndienen(string Urgent, string Auto, DateTime? Datum, TimeSpan? Duur, TimeSpan? Tijd, string Plaatsnaam, string StraatEnHuisnummer, string KOmschrijving, string Omschrijving)
        {
            Hulpvraag hulpvraag = new Hulpvraag();
            if (!string.IsNullOrEmpty(Omschrijving))
            {
                if (!string.IsNullOrEmpty(Urgent))
                {
                    hulpvraag.Urgent = true;
                }
                else
                {
                    hulpvraag.Urgent = false;
                }

                if (!string.IsNullOrEmpty(Auto))
                {
                    hulpvraag.Auto = true;
                }
                else
                {
                    hulpvraag.Auto = false;
                }

                hulpvraag.StartDatum = Datum + Tijd;
                hulpvraag.EindDatum = hulpvraag.StartDatum + Duur;
                hulpvraag.Plaatsnaam = Plaatsnaam;
                hulpvraag.Locatie = StraatEnHuisnummer;
                hulpvraag.Titel = KOmschrijving;
                hulpvraag.HulpvraagInhoud = Omschrijving;
                hulpvraag.Hulpbehoevende = (Session["Gebruiker"] as Gebruiker).Gebruikersnaam;

                carematch.database.HulpvraagToevoegen(hulpvraag, Session["Gebruiker"] as Gebruiker);

                return RedirectToAction("HulpvragenOverzicht");
            }

            return View();
        }
        public ActionResult ChatBarcode()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ChatBarcode(string partner)
        {
            return RedirectToAction("ChatBekijken", "Chat", new { partner = partner });
        }
    }
}