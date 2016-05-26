using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CareMatch.Models
{
    public class Vrijwilligers
    {
        public Int32 ID;
        public string Naam;
        public Vrijwilligers(Int32 ID, string naam)
        {
            this.ID = ID;
            this.Naam = naam;
        }
        public override string ToString()
        {
            return Naam;
        }
    }
}