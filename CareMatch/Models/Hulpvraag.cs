using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAREMATCH.Hulpvragen
{
    class Hulpvraag
    {
        public DateTime DatumTijd { get; set; }
        public int HulpvraagID { get; set; }
        public int Cijfer { get; set; }
        public string Duur { get; set; }
        public string HulpbehoevendeFoto { get; set; }
        public string Locatie { get; set; }
        public string Hulpbehoevende { get; set; }
        public string Vrijwilliger { get; set; }
        public string Titel { get; set;}
        public string HulpvraagInhoud { get; set; }
        public string Frequentie { get; set; }
        public string Reactie { get; set; }
        public string Beoordeling { get; set; } //hulpvraag is afgerond als er een beoordeling gegeven is.
        public string LaatstGereageerdDoor { get; set; }
        public bool Auto { get; set; }
        public bool Urgent { get; set; }

        public Hulpvraag()
        {

        }
        public override string ToString()
        {
            return this.HulpvraagID.ToString();
        }
    }
}
