using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CareMatch.Agenda;
using System.IO;

namespace CareMatch.Models
{
   
    public class Gebruiker
    {

        //Fields
        private List<AgendaPunt> agendaPuntList;

        //Properties        
        public int GebruikersID { get; set; }
        public string Pasfoto { get; set; }
        public string Gebruikersnaam { get; set; }
        public string Wachtwoord { get; set; }
        public string Voornaam { get; set; }
        public string Achternaam { get; set; }
        public string GebruikerInfo { get; set; }
        public string Rol { get; set; }
        public string VOG { get; set; }
        public DateTime GeboorteDatum { get; set; }
        public bool? Auto { get; set; }
        public bool Approved { get; set; }
        
        //Constructor
        public Gebruiker()
        {
            agendaPuntList = new List<AgendaPunt>();
        }
        //Methoden
        public void AgendaPuntToevoegen(AgendaPunt agendaPunt)
        {
            agendaPuntList.Add(agendaPunt);
        }
        public List<AgendaPunt> GetAgendaPunten()
        {
            return agendaPuntList;
        }
        public string GetLocalDropBox()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\DropBox\CareMatch\" + this.Gebruikersnaam;
        }
    }
}
