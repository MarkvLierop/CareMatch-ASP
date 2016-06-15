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
        public ActionResult Index(string id, string ingelogd)
        {
            if (Session["Gebruiker"] == null)
            {
                return RedirectToAction("Index", "Login", new { area = "" });
            }

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
                    ViewBag.Partner = "";            
                }

            return View();
        }

        public ActionResult videotest()
        {
            return View();
        }

        public ActionResult ChatBekijken(string partner)
        {
            return RedirectToAction("Index", new { id = partner.ToString() });
        }

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
                ViewBag.Partner = "";
            }

            return RedirectToAction("ChatBekijken", new { partner = id });
        }
    }   
    }
