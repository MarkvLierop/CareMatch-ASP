using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareMatch.Models
{
    public class Hulpvraag
    {
        public int HulpvraagID { get; set; }
        public string Cijfer { get; set; }
        public DateTime? StartDatum { get; set; }
        public DateTime? EindDatum { get; set; }
        public string HulpbehoevendeFoto { get; set; }
        public string Locatie { get; set; }
        public string Hulpbehoevende { get; set; }
        public string Vrijwilliger { get; set; }
        public string Titel { get; set;}
        public string HulpvraagInhoud { get; set; }
        public string Beoordeling { get; set; } //hulpvraag is afgerond als er een beoordeling gegeven is.
        public bool Auto { get; set; }
        public bool Urgent { get; set; }
        public string Plaatsnaam { get; set; }

        public Hulpvraag()
        {

        }
        public override string ToString()
        {
            return this.HulpvraagID.ToString();
        }
    }
}
