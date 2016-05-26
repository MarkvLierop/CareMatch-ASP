using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAREMATCH.Agenda
{
    public class AgendaPunt
    {
        //Properties
        public int AfspraakID { get; set; }
        public int AgendaEigenaar { get; set; }
        public int DatumTijdStart { get; set; }
        public int DatumTijdEind { get; set; }
        public string AfspraakMet { get; set; }
        public string Titel { get; set; }
        public string Beschrijving { get; set; }
        public Point Locatie { get; set; }
        public DateTime AfspraakDatum { get; set; }
        
        //Constructor
        public AgendaPunt()
        { 
        }
    }
}
