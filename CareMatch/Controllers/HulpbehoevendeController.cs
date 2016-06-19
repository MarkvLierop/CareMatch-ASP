using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CareMatch.Models;
using System.IO;

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
        public ActionResult Profiel(string wachtwoord, string hwachtwoord, string info, string auto, HttpPostedFileBase pasfoto)
        {
            if (Request.Form.Count > 0)
            {
                ((Gebruiker)Session["Gebruiker"]).GebruikerInfo = info;

                // Pasfoto uploaden
                if (pasfoto != null)
                {
                    if(pasfoto.ContentLength > 0)
                    {
                        // Bestandsinfo opvragen
                        System.IO.FileInfo Dfile = new System.IO.FileInfo(pasfoto.FileName);

                        // Linkt naar CareMatch-ASP\CareMatch\Bestanden\pasfoto
                        var path = Path.Combine(Server.MapPath("~/Fotos"), (Session["Gebruiker"] as Gebruiker).Gebruikersnaam + Dfile.Extension);
                        pasfoto.SaveAs(path);
                        ((Gebruiker)Session["Gebruiker"]).Pasfoto = (Session["Gebruiker"] as Gebruiker).Gebruikersnaam + Dfile.Extension;
                    }
                }
                if (wachtwoord != hwachtwoord)
                {
                    ViewBag.foutmelding = "Wachtwoorden zijn niet gelijk aan elkaar.";
                    return View();
                }
                if (!string.IsNullOrEmpty(wachtwoord))
                {
                    ((Gebruiker)Session["Gebruiker"]).Wachtwoord = wachtwoord;
                    carematch.database.GebruikerProfielAanpassen(Session["Gebruiker"] as Gebruiker, true, true);
                }
                if (!string.IsNullOrEmpty(info))
                {
                    carematch.database.GebruikerProfielAanpassen(Session["Gebruiker"] as Gebruiker, false, true);
                }
                return RedirectToAction("Index", "Hulpbehoevende");
            }

            return View();
        }
        // geeft een overzicht van alle eigen hulpvragen van de gebruiker
        public ActionResult HulpvragenOverzicht()
        {
            Gebruiker gebruiker = Session["Gebruiker"] as Gebruiker;
            ViewData["hulpvragen"] = carematch.database.HulpvragenOverzicht(gebruiker, Request.QueryString["filter"]);
            return View();
        }

        // laat een hulpvraag in detail zien
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

                hulpvraag.EindDatum = DateTime.Parse(Convert.ToString(hulpvraag.EindDatum - hulpvraag.StartDatum));
            }

            ViewData["Hulpvraag"] = selectedhulpvraag;

            if (selectedhulpvraag.Vrijwilliger != "")
            {
                ViewData["Vrijwilliger"] = carematch.database.GebruikerInfoOpvragen(selectedhulpvraag.Vrijwilliger);
            }
            return View();
        }

        // wijzigt een hulpvraag
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
                carematch.database.HulpvraagAanpassen(Session["Gebruiker"] as Gebruiker, hulpvraag);

                return RedirectToAction("HulpvragenOverzicht");
            }

            return View();
        }

        /// <summary>
        /// voegt een beoordeling toe aan een hulpvraag
        /// </summary>
        /// <param name="id">hulpvraag id</param>
        /// <param name="Cijfer">beoordeling cijfer</param>
        /// <param name="Beoordeling">beoordeling bericht</param>
        /// <returns></returns>
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

        // maakt een nieuwe hulpvraag aan
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

        // geeft de pagina van waar je je barcode kunt scannen
        public ActionResult ChatBarcode()
        {
            return View();
        }

        // als de barcode is gescand begin een chat, in de chat controller.
        [HttpPost]
        public ActionResult ChatBarcode(string partner)
        {
            return RedirectToAction("ChatBekijken", "Chat", new { partner = partner });
        }
    }
}