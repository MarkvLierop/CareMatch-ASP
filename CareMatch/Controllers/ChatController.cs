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
        public ActionResult Index()
        {
            CareMatch.Models.Gebruiker gebruiker = new CareMatch.Models.Gebruiker();
            Session["Gebruiker"] = gebruiker;
            ViewBag.gebruiker = Session["Gebruiker"];



            ViewBag.ditiskut = database.ChatLaden("Hulpbehoevende", "Vrijwilliger", 160, 161);
            return View();
        }

        public void test(string bericht)
        {

        }
    }
}