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
            if((Session["Gebruiker"] as Models.Gebruiker).Rol.ToLower() == "vrijwilliger")
            {
                ViewBag.Gebruikers = database.HulpbehoevendeLijst();
            }
           else
            {
                ViewBag.Gebruikers = database.VrijwilligersLijst();
            }
            if(id != null)
            {
                ViewBag.Chat = database.ChatLaden(id, (Session["Gebruiker"] as Models.Gebruiker).Gebruikersnaam, 160, 161);
                ViewBag.Parner = id;
            }
            else
            {
                ViewBag.Chat = "";
                ViewBag.Parner = "" ;
            }
            return View();
        }

        [HttpPost]
        public ActionResult BerichtVerzenden(string bericht, string partner)
        {
            database.ChatInvoegen(1, bericht, database.ChatpartnerID(partner),  (Session["Gebruiker"] as Models.Gebruiker).GebruikersID, DateTime.Now.ToString("dd / MMM HH: mm"));
            
            return RedirectToAction("Index", "Chat", database.ChatpartnerID(partner));
        }

        public ActionResult ChatBekijken(string id)
        {
            return RedirectToAction("Index", "Chat", id);
        }
    }
}