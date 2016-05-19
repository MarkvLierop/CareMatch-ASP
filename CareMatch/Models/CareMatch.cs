using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CareMatch.Models
{
    public class CareMatch
    {
        public List<Hulpbehoevenden> hulpbehoevenden = new List<Hulpbehoevenden>();
        public List<Vrijwilligers> vrijwilligers = new List<Vrijwilligers>();

        public CareMatch()
        {
            hulpbehoevenden.Add(new Hulpbehoevenden(1, "Piet"));
            hulpbehoevenden.Add(new Hulpbehoevenden(2, "Hans"));
            hulpbehoevenden.Add(new Hulpbehoevenden(3, "Nellie"));
            hulpbehoevenden.Add(new Hulpbehoevenden(4, "Frans"));
            hulpbehoevenden.Add(new Hulpbehoevenden(5, "Mien"));

            vrijwilligers.Add(new Vrijwilligers(1, "Peter"));
            vrijwilligers.Add(new Vrijwilligers(2, "Anja"));
            vrijwilligers.Add(new Vrijwilligers(3, "Marja"));
            vrijwilligers.Add(new Vrijwilligers(4, "Mark"));
        }
       
    }
}