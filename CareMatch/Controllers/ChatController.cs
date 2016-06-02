using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CAREMATCH;

namespace CareMatch.Controllers
{
    public class ChatController : Controller
    {
        CareMatch1 carematch = new CareMatch1();
        // GET: Chat
        public ActionResult Index()
        {
            return View();
        }
    }
}