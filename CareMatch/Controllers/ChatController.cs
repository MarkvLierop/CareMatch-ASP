using CareMatch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CareMatch.Controllers
{
    public class ChatController : Controller
    {
        CareMatch.Models.Database database = new CareMatch.Models.Database();
        
        // GET: Chat

        // laat de het lege chatvenser zien, vanaf hier kun je chats starten ect.
        public ActionResult Index(string id, string ingelogd)
        {
                database.ChatZetOnline((Session["Gebruiker"] as Models.Gebruiker).GebruikersID);
                ViewBag.gebruiker = Session["Gebruiker"] as CareMatch.Models.Gebruiker;
                if ((Session["Gebruiker"] as Models.Gebruiker).Rol.ToLower() == "vrijwilliger")
                {
                    ViewBag.Gebruikers = database.HulpbehoevendeLijst();
                    ViewBag.Bestaand = database.BestaandeChatlijstHulpbehoevende((Session["Gebruiker"] as Models.Gebruiker).GebruikersID);
                }
                else
                {
                    ViewBag.Gebruikers = database.VrijwilligersLijst();
                ViewBag.Bestaand = database.BestaandeChatlijstVrijwilligers((Session["Gebruiker"] as Models.Gebruiker).GebruikersID);
            }

                if (!string.IsNullOrEmpty(id))
                {
                    ViewBag.Chat = database.ChatLaden(id, (Session["Gebruiker"] as Models.Gebruiker).Gebruikersnaam, database.ChatpartnerID(id), (Session["Gebruiker"] as Models.Gebruiker).GebruikersID);
                    ViewBag.Partner = id;
                }
                else
                {
                    ViewBag.Chat = new List<Models.Chatbericht>();
                    ViewBag.Partner = string.Empty;            
                }

            return View();
        }

        // nutteloos?
        public ActionResult videotest()
        {
            return View();
        }

        // laat de chat zien
        public ActionResult ChatBekijken(string partner)
        {
            return RedirectToAction("Index", new { id = partner.ToString() });
        }

        // als een chatpartner is gekozen of een barcode is gescanned, laat de chat zien
        [HttpPost]
        public ActionResult Index(string id, string bericht, string ingelogd)
        {
            database.ChatZetOnline((Session["Gebruiker"] as Models.Gebruiker).GebruikersID);
            ViewBag.gebruiker = Session["Gebruiker"] as CareMatch.Models.Gebruiker;
            if ((Session["Gebruiker"] as Models.Gebruiker).Rol.ToLower() == "vrijwilliger")
            {
                ViewBag.Gebruikers = database.HulpbehoevendeLijst();
            }
            else
            {
                ViewBag.Gebruikers = database.VrijwilligersLijst();
            }

            if (!string.IsNullOrEmpty(bericht))
            {
                database.ChatInvoegen(1, bericht, database.ChatpartnerID(id), (Session["Gebruiker"] as Models.Gebruiker).GebruikersID, DateTime.Now.ToString("dd / MMM HH: mm"));
            }

            if (!string.IsNullOrEmpty(id))
            {
                ViewBag.Chat = database.ChatLaden(id, (Session["Gebruiker"] as Models.Gebruiker).Gebruikersnaam, database.ChatpartnerID(id), (Session["Gebruiker"] as Models.Gebruiker).GebruikersID);
                ViewBag.Partner = id;
            }
            else
            {
                ViewBag.Chat = new List<Models.Chatbericht>();
                ViewBag.Partner = string.Empty;
            }

            return RedirectToAction("ChatBekijken", new { partner = id });
        }
    }   
    }
