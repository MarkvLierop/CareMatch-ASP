using CareMatch.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CareMatch.Controllers
{
    public class CareMatchController : Controller
    {
        public Hulpbehoevenden gebruikerHulpbehoevende = null;
        public Vrijwilligers gebruikerVrijwilliger = null;
        public CareMatch.Models.CareMatch carematch = new CareMatch.Models.CareMatch();
        // GET: CareMatch
        public ActionResult index()
        {
            return View();
        }
        public ActionResult vrijwilliger()
        {
            ViewBag.Data = carematch;
            IEnumerable<SelectListItem> items = carematch.hulpbehoevenden
              .Select(c => new SelectListItem
              {
                  Value = Convert.ToString(c.ID),
                  Text = c.Naam
              });
            ViewBag.CategoryID = items;
            return View();
        }
        public ActionResult hulpbehoevende()
        {
            ViewBag.Data = carematch;
            return View();
        }
        public ActionResult videochat(int ID, string functie, string gebruiker)
        {
            Vrijwilligers vrijwilligerSelected = null;
            Hulpbehoevenden hulpbehoevendeSelected = null;

            if(functie == "hulpbehoevende")
            {
                foreach (Vrijwilligers vrijwilliger in carematch.vrijwilligers)
                {
                    if (vrijwilliger.ID == ID)
                    {
                        vrijwilligerSelected = vrijwilliger;
                      
                    }
                }
                foreach (Hulpbehoevenden hulpbehoevende in carematch.hulpbehoevenden)
                {
                    if (hulpbehoevende.Naam == gebruiker)
                    {
                        gebruikerHulpbehoevende = hulpbehoevende;
                        
                    }
                }
            }
            else
            {
                foreach (Hulpbehoevenden hulpbehoevende in carematch.hulpbehoevenden)
                {
                    if (hulpbehoevende.ID == ID)
                    {
                        hulpbehoevendeSelected = hulpbehoevende;
                        
                    }
                }
                foreach (Vrijwilligers vrijwilliger in carematch.vrijwilligers)
                {
                    if (vrijwilliger.Naam == gebruiker)
                    {
                        gebruikerVrijwilliger = vrijwilliger;
                       
                    }
                }
            }


            if (hulpbehoevendeSelected != null)
            {
                ViewData["partner"]= hulpbehoevendeSelected;
                ViewData["gebruiker"] = gebruikerVrijwilliger;
                ViewData["functie"] = functie;
                return View();
            }
            else
            {
                ViewData["partner"] = vrijwilligerSelected;
                ViewData["gebruiker"] = gebruikerHulpbehoevende;
                ViewData["functie"] = functie;
                return View();
            }
        }
    }
}