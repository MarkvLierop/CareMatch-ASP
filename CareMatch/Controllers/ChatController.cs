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
        public ActionResult Index(string id)
        {
            ViewBag.gebruiker = Session["Gebruiker"] as CareMatch.Models.Gebruiker;
            if ((Session["Gebruiker"] as Models.Gebruiker).Rol.ToLower() == "vrijwilliger")
            {
                ViewBag.Gebruikers = database.HulpbehoevendeLijst();
            }
            else
            {
                ViewBag.Gebruikers = database.VrijwilligersLijst();
            }

            if (!string.IsNullOrEmpty(id))
            {
                ViewBag.Chat = database.ChatLaden(id, (Session["Gebruiker"] as Models.Gebruiker).Gebruikersnaam, database.ChatpartnerID(id), (Session["Gebruiker"] as Models.Gebruiker).GebruikersID);
                ViewBag.Parner = id;
            }
            else
            {
                ViewBag.Chat = new List<Models.Chatbericht>();
                ViewBag.Parner = "";
            }
            return View();
        }

        [HttpPost]
        public ActionResult BerichtVerzenden(string bericht, string partner)
        {
            database.ChatInvoegen(1, bericht, database.ChatpartnerID(partner), (Session["Gebruiker"] as Models.Gebruiker).GebruikersID, DateTime.Now.ToString("dd / MMM HH: mm"));

            return RedirectToAction("Index", "Chat", database.ChatpartnerID(partner));
        }

        public ActionResult ChatBekijken(string partner)
        {
            return RedirectToAction("Index", new { id = partner.ToString() });
        }
    }
           
    }
