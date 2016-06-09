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
            ViewData["hulpvragen"] = carematch.database.HulpvragenOverzicht(gebruiker, "");
            return View();
        }
        public ActionResult HulpvraagIndienen(string Urgent, string Auto, string Datum, string Tijd, string Plaatsnaam,
                                                string StraatEnHuisnummer, string KOmschrijving, string Omschrijving)
        {
            if (!string.IsNullOrEmpty(Omschrijving))
            {
                Hulpvraag hulpvraag = new Hulpvraag();
                //hulpvraag.Urgent = Urgent;
                //hulpvraag.Auto = Auto;
                hulpvraag.DatumTijd = Convert.ToDateTime(Datum + Tijd);
                hulpvraag.Locatie = StraatEnHuisnummer;
                hulpvraag.Titel = KOmschrijving;
                hulpvraag.HulpvraagInhoud = Omschrijving;
                hulpvraag.Hulpbehoevende = (Session["Gebruiker"] as Gebruiker).Gebruikersnaam;

                carematch.database.HulpvraagToevoegen(hulpvraag, Session["Gebruiker"] as Gebruiker);

                return RedirectToAction("Index", "Hulpbehoevende");
            }
            return View();
        }
    }
}