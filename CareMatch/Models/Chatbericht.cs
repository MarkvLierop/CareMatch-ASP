using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

namespace CareMatch.Models
{
    public class Chatbericht
    {
        // Properties
        public int ID { get; }

        public string Inhoud { get; }

        public string VerzenderNaam { get; }

        public DateTime datumtijd { get; }

        // Properties


        // Constructor
        public Chatbericht(string inhoud, string verzender, int id, DateTime datum)
        {
            Inhoud = inhoud.Replace(System.Environment.NewLine, string.Empty);
            VerzenderNaam = verzender;
            ID = id;
            datumtijd = datum;
        }

        public override string ToString()
        {
            return VerzenderNaam + ": " + Inhoud;
        }
    }
}
