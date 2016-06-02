using System;
using System.Collections.Generic;
using System.Text;
using Oracle.ManagedDataAccess.Client;
using System.Security.Cryptography;
using System.Data;
using System.Globalization;

namespace CareMatch
{
    public class Database
    {
        private OracleConnection con;
        private OracleCommand command;
        private OracleDataReader reader;

        private Gebruiker gebruiker;
        private Agenda.AgendaPunt agendaPunt;

        private DateTime vandaag;
        private string tempString;

        public Database()
        {
            vandaag = new DateTime();

            string constr = "Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=fhictora01.fhict.local)(PORT=1521)))"
                          + "(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=fhictora)));"
                          + "User ID=DBI327544; PASSWORD=CareMatch;";

            con = new OracleConnection(constr);
        }


        #region Hulpvragen Queries
        public void HulpvraagToevoegen(Hulpvragen.Hulpvraag hulpvraag, Gebruiker gebruiker)
        {
            string AutoBenodigd;
            con.Open();
            if (hulpvraag.Urgent)
            {
                tempString = "Y";
            }
            else
            {
                tempString = "N";
            }
            if (hulpvraag.Auto)
            {
                AutoBenodigd = "Y";
            }
            else
            {
                AutoBenodigd = "N";
            }
            using (command = new OracleCommand(@"INSERT INTO Hulpvraag(GebruikerID, HulpvraagInhoud, Urgent, DatumTijd, Duur, Frequentie, Titel, Locatie, AutoBenodigd)" +
                                                              "VALUES(:gebruikerid, :hulpvraaginhoud, :temp, :datumtijd, :duur, :frequentie, :titel, :locatie, :auto)", con))
            {
                command.Parameters.Add(new OracleParameter(":gebruikerid", OracleDbType.Int32)).Value = gebruiker.GebruikersID;
                command.Parameters.Add(new OracleParameter(":hulpvraaginhoud", OracleDbType.Varchar2)).Value = hulpvraag.HulpvraagInhoud;
                command.Parameters.Add(new OracleParameter(":temp", OracleDbType.Varchar2)).Value = tempString; //urgent
                command.Parameters.Add(new OracleParameter(":datumtijd", OracleDbType.Varchar2)).Value = hulpvraag.DatumTijd;
                command.Parameters.Add(new OracleParameter(":duur", OracleDbType.Varchar2)).Value = hulpvraag.Duur;
                command.Parameters.Add(new OracleParameter(":frequentie", OracleDbType.Varchar2)).Value = hulpvraag.Frequentie;
                command.Parameters.Add(new OracleParameter(":titel", OracleDbType.Varchar2)).Value = hulpvraag.Titel;
                command.Parameters.Add(new OracleParameter(":locatie", OracleDbType.Varchar2)).Value = hulpvraag.Locatie;
                command.Parameters.Add(new OracleParameter(":auto", OracleDbType.Varchar2)).Value = AutoBenodigd;
                command.ExecuteNonQuery();
            }
            con.Close();
        }
        public void HulpvraagRapporteer(Hulpvragen.Hulpvraag hulpvraag)
        {
            con.Open();
            command = new OracleCommand(@"UPDATE Hulpvraag SET Flagged ='Y' WHERE HulpvraagID = :hulpvraagid", con);
            command.Parameters.Add(new OracleParameter(":hulpvraagid", OracleDbType.Int32)).Value = hulpvraag.HulpvraagID;
            reader = command.ExecuteReader();
            con.Close();
        }
        //Mee bezig.
        public void HulpvraagVerwijderen(int hulpvraagID)
        {
            con.Open();

            command = new OracleCommand("DELETE FROM Hulpvraag WHERE HulpvraagID =:id", con);
            command.Parameters.Add(new OracleParameter(":id", OracleDbType.Int32)).Value = hulpvraagID;
            command.ExecuteNonQuery();
            con.Close();
        }
        public void HulpvraagAanpassen(Gebruiker gebruiker, Hulpvragen.Hulpvraag hulpvraag)
        {
            con.Open();
            if (hulpvraag.Urgent)
            {
                tempString = "Y";
            }
            else
            {
                tempString = "N";
            }

            command = new OracleCommand(@"UPDATE Hulpvraag SET Reactie =:reactie, LaatstGereageerdDoor=:gebruikersnaam, VrijwilligerID=(SELECT GebruikerID FROM Gebruiker WHERE GebruikerID =:gebruikerid AND LOWER(ROL)='vrijwilliger'), Hulpvraaginhoud=:hulpvraaginhoud, Urgent=:temp WHERE HulpvraagID=:hulpvraagid", con);
            command.Parameters.Add(new OracleParameter(":reactie", OracleDbType.Varchar2)).Value = hulpvraag.Reactie;
            command.Parameters.Add(new OracleParameter(":gebruikersnaam", OracleDbType.Varchar2)).Value = gebruiker.Gebruikersnaam;
            command.Parameters.Add(new OracleParameter(":gebruikerid", OracleDbType.Int32)).Value = gebruiker.GebruikersID;
            command.Parameters.Add(new OracleParameter(":hulpvraaginhoud", OracleDbType.Varchar2)).Value = hulpvraag.HulpvraagInhoud;
            command.Parameters.Add(new OracleParameter(":temp", OracleDbType.Varchar2)).Value = tempString;
            command.Parameters.Add(new OracleParameter(":hulpvraagid", OracleDbType.Int32)).Value = hulpvraag.HulpvraagID;
            command.ExecuteNonQuery();
            con.Close();
        }
        public List<Hulpvragen.Hulpvraag> HulpvragenOverzicht(Gebruiker gebruiker, string filter)
        {
            List<Hulpvragen.Hulpvraag> hulpvraagList = new List<Hulpvragen.Hulpvraag>();

            con.Open();
            if ((filter == "Alle hulpvragen" || filter == "") && gebruiker.Rol.ToLower() == "vrijwilliger")
            {
                //Standaard alle hulpvragen laten zien voor vrijwilligers. - Gerapporteerde hulpvragen niet laten zien. - Gesloten hulpvragen ook niet(waar beoordeling is ingevuld)
                command = new OracleCommand("SELECT Hulpvraag.HulpvraagID, (SELECT Gebruikersnaam FROM Gebruiker WHERE Hulpvraag.GebruikerID = Gebruiker.GebruikerID) as hulpbeh, (SELECT Gebruikersnaam FROM Gebruiker WHERE Hulpvraag.VrijwilligerID = Gebruiker.GebruikerID) as vrijwilliger, Hulpvraag.HulpvraagInhoud,  Hulpvraag.DatumTijd, Hulpvraag.Urgent, Hulpvraag.Frequentie, Hulpvraag.Titel, Hulpvraag.Reactie, Hulpvraag.LaatstGereageerdDoor, Hulpvraag.Duur FROM Hulpvraag WHERE Flagged != 'Y' AND Beoordeling IS NULL", con);

            }
            else if (filter == "Eigen hulpvragen" && gebruiker.Rol.ToLower() == "vrijwilliger")
            {
                //overzicht eigen toegekende hulpvragen voor vrijwilligers
                command = new OracleCommand("SELECT Hulpvraag.HulpvraagID, (SELECT Gebruikersnaam FROM Gebruiker WHERE Hulpvraag.GebruikerID = Gebruiker.GebruikerID) as hulpbeh, (SELECT Gebruikersnaam FROM Gebruiker WHERE Hulpvraag.VrijwilligerID = Gebruiker.GebruikerID) as vrijwilliger, Hulpvraag.HulpvraagInhoud,  Hulpvraag.DatumTijd, Hulpvraag.Urgent, Hulpvraag.Frequentie,  Hulpvraag.Titel, Hulpvraag.Reactie, Hulpvraag.Duur, Hulpvraag.LaatstGereageerdDoor FROM Hulpvraag WHERE VrijwilligerID=:gebruikerid AND Beoordeling IS NULL", con);
                command.Parameters.Add(new OracleParameter(":gebruikerid", OracleDbType.Int32)).Value = gebruiker.GebruikersID;
            }
            else if (filter == "Nieuwe reacties" && gebruiker.Rol.ToLower() == "vrijwilliger")
            {
                //Eigen hulpvragen weergeven waarop een nieuwe reactie is gegeven.
                command = new OracleCommand("SELECT Hulpvraag.HulpvraagID, (SELECT Gebruikersnaam FROM Gebruiker WHERE Hulpvraag.GebruikerID = Gebruiker.GebruikerID) as hulpbeh, (SELECT Gebruikersnaam FROM Gebruiker WHERE Hulpvraag.VrijwilligerID = Gebruiker.GebruikerID) as vrijwilliger, Hulpvraag.LaatstGereageerdDoor, Hulpvraag.HulpvraagInhoud,  Hulpvraag.DatumTijd, Hulpvraag.Urgent, Hulpvraag.Frequentie,  Hulpvraag.Titel, Hulpvraag.Reactie, Hulpvraag.LaatstGereageerdDoor, Hulpvraag.Duur FROM Hulpvraag WHERE VrijwilligerID=:gebruikerid AND LaatstGereageerdDoor !=:gebruikersnaam AND Beoordeling IS NULL", con);
                command.Parameters.Add(new OracleParameter(":gebruikerid", OracleDbType.Int32)).Value = gebruiker.GebruikersID;
                command.Parameters.Add(new OracleParameter(":gebruikersnaam", OracleDbType.Varchar2)).Value = gebruiker.Gebruikersnaam;
            }
            else if (filter == "Nieuwe reacties" && gebruiker.Rol.ToLower() == "hulpbehoevende")
            {
                //Hulpbehoevende hulpvragen weergeven waarop een nieuwe reactie is 
                command = new OracleCommand("SELECT Hulpvraag.HulpvraagID, (SELECT Gebruikersnaam FROM Gebruiker WHERE Hulpvraag.GebruikerID = Gebruiker.GebruikerID) as hulpbeh, (SELECT Gebruikersnaam FROM Gebruiker WHERE Hulpvraag.VrijwilligerID = Gebruiker.GebruikerID) as vrijwilliger, Hulpvraag.LaatstGereageerdDoor, Hulpvraag.HulpvraagInhoud,  Hulpvraag.DatumTijd, Hulpvraag.Urgent, Hulpvraag.Frequentie,  Hulpvraag.Titel, Hulpvraag.Reactie, Hulpvraag.LaatstGereageerdDoor, Hulpvraag.Duur FROM Hulpvraag WHERE GebruikerID=:gebruikerid AND LaatstGereageerdDoor !=:gebruikersnaam AND LaatstGereageerdDoor != 'Geen Reacties'", con); // GebruikerID=:gebruikerid AND LaatstGereageerdDoor !=:gebruikersnaam  ERRORRR
                command.Parameters.Add(new OracleParameter(":gebruikerid", OracleDbType.Int32)).Value = gebruiker.GebruikersID;
                command.Parameters.Add(new OracleParameter(":gebruikersnaam", OracleDbType.Varchar2)).Value = gebruiker.Gebruikersnaam;
            }
            else if (filter == "Beoordelingen" && gebruiker.Rol.ToLower() == "vrijwilliger")
            {
                //overzicht eigen toegekende hulpvragen voor vrijwilligers
                command = new OracleCommand("SELECT Hulpvraag.HulpvraagID, (SELECT Gebruikersnaam FROM Gebruiker WHERE Hulpvraag.GebruikerID = Gebruiker.GebruikerID) as hulpbeh, (SELECT Gebruikersnaam FROM Gebruiker WHERE Hulpvraag.VrijwilligerID = Gebruiker.GebruikerID) as vrijwilliger, Hulpvraag.HulpvraagInhoud,  Hulpvraag.DatumTijd, Hulpvraag.Urgent, Hulpvraag.Frequentie,  Hulpvraag.Titel, Hulpvraag.Reactie, Hulpvraag.Duur, Hulpvraag.LaatstGereageerdDoor FROM Hulpvraag WHERE VrijwilligerID=:gebruikerid AND Beoordeling IS NOT NULL", con);
                command.Parameters.Add(new OracleParameter(":gebruikerid", OracleDbType.Int32)).Value = gebruiker.GebruikersID;
            }
            else if (filter.ToLower() == "ongepaste hulpvragen" && gebruiker.Rol.ToLower() == "beheerder")
            {
                //throw new NotImplementedException();
                command = new OracleCommand("SELECT Hulpvraag.HulpvraagID, (SELECT Gebruikersnaam FROM Gebruiker WHERE Hulpvraag.GebruikerID = Gebruiker.GebruikerID) as hulpbeh, (SELECT Gebruikersnaam FROM Gebruiker WHERE Hulpvraag.VrijwilligerID = Gebruiker.GebruikerID) as vrijwilliger, Hulpvraag.HulpvraagInhoud,  Hulpvraag.DatumTijd, Hulpvraag.Urgent, Hulpvraag.Frequentie, Hulpvraag.Titel, Hulpvraag.Reactie, Hulpvraag.LaatstGereageerdDoor, Hulpvraag.Duur FROM Hulpvraag WHERE Hulpvraag.Flagged = 'Y'", con);
                //parameters erbij?
                //misschien niet nodig omdat je deze nergens kunt invullen?
            }

            else
            {
                //Overzicht eigen hulpvragen voor hulpbehoevende.
                command = new OracleCommand("SELECT Hulpvraag.HulpvraagID, (SELECT Gebruikersnaam FROM Gebruiker WHERE Hulpvraag.GebruikerID = Gebruiker.GebruikerID) as hulpbeh, (SELECT Gebruikersnaam FROM Gebruiker WHERE Hulpvraag.VrijwilligerID = Gebruiker.GebruikerID) as vrijwilliger, Hulpvraag.HulpvraagInhoud,  Hulpvraag.DatumTijd, Hulpvraag.Urgent, Hulpvraag.Frequentie,  Hulpvraag.Titel, Hulpvraag.Reactie, Hulpvraag.Duur, Hulpvraag.LaatstGereageerdDoor FROM Hulpvraag WHERE (SELECT Gebruikersnaam FROM Gebruiker WHERE Hulpvraag.GebruikerID = Gebruiker.GebruikerID)=:gebruikersnaam AND Beoordeling IS NULL", con);
                command.Parameters.Add(new OracleParameter(":gebruikersnaam", OracleDbType.Varchar2)).Value = gebruiker.Gebruikersnaam;
            }
            reader = command.ExecuteReader();
            while (reader.Read())
            {
                Hulpvragen.Hulpvraag hulpvraag = new Hulpvragen.Hulpvraag();

                hulpvraag.HulpvraagID = Convert.ToInt32(reader["HulpvraagID"]);
                hulpvraag.Titel = reader["Titel"].ToString();
                hulpvraag.Hulpbehoevende = reader["hulpbeh"].ToString();
                hulpvraag.Vrijwilliger = reader["vrijwilliger"].ToString();
                hulpvraag.HulpvraagInhoud = reader["HulpvraagInhoud"].ToString();
                hulpvraag.Frequentie = reader["Frequentie"].ToString();
                hulpvraag.Reactie = reader["Reactie"].ToString();
                hulpvraag.LaatstGereageerdDoor = reader["LaatstGereageerdDoor"].ToString();
                hulpvraag.Duur = reader["Duur"].ToString();
                if (!(reader["DatumTijd"] is DBNull))
                {
                    hulpvraag.DatumTijd = Convert.ToDateTime(reader["DatumTijd"]);
                }
                if (reader["Urgent"].ToString() == "Y")
                {
                    hulpvraag.Urgent = true;
                }
                else
                {
                    hulpvraag.Urgent = false;
                }

                hulpvraagList.Add(hulpvraag);
            }
            con.Close();

            return hulpvraagList;
        }
        public string HulpvraagProfielFoto(Gebruiker gebruiker, Hulpvragen.Hulpvraag hulpvraag, string rol)
        {
            con.Open();
            if (rol == "hulpbehoevende")
            {
                command = new OracleCommand("SELECT Foto FROM Gebruiker WHERE Gebruikersnaam=:hulpbehoevende", con);
                command.Parameters.Add(new OracleParameter(":hulpbehoevende", OracleDbType.Varchar2)).Value = hulpvraag.Hulpbehoevende;
            }
            else if (rol == "vrijwilliger")
            {
                command = new OracleCommand("SELECT Foto FROM Gebruiker WHERE Gebruikersnaam=:vrijwilliger", con);
                command.Parameters.Add(new OracleParameter("vrijwilliger", hulpvraag.Vrijwilliger));
            }
            reader = command.ExecuteReader();
            while (reader.Read())
            {
                if (rol == "hulpbehoevende")
                {
                    if (reader["Foto"].ToString() != "")
                    {
                        tempString = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\DropBox\CareMatch\" + hulpvraag.Hulpbehoevende + "\\" + reader["Foto"].ToString();
                    }
                }
                else if (rol == "vrijwilliger")
                {
                    if (reader["Foto"].ToString() != "")
                    {
                        tempString = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\DropBox\CareMatch\" + hulpvraag.Vrijwilliger + "\\" + reader["Foto"].ToString();
                    }
                }
            }
            con.Close();
            return tempString;
        }
        public void HulpvraagBeoordelingToevoegen(Hulpvragen.Hulpvraag hulpvraag)
        {
            con.Open();
            command = new OracleCommand(@"UPDATE Hulpvraag SET Beoordeling =:beoordeling, Cijfer =:cijfer WHERE HulpvraagID=:hulpvraagid", con);
            command.Parameters.Add(new OracleParameter(":beoordeling", OracleDbType.Varchar2)).Value = hulpvraag.Beoordeling;
            command.Parameters.Add(new OracleParameter(":cijfer", OracleDbType.Int32)).Value = hulpvraag.Cijfer;
            command.Parameters.Add(new OracleParameter(":hulpvraagid", OracleDbType.Int32)).Value = hulpvraag.HulpvraagID;
            command.ExecuteNonQuery();
            con.Close();
        }
        #endregion
        #region Agenda Queries        
        public void AgendaOverzicht(Gebruiker gebruiker, string filter, string datum)
        {
            try
            {
                con.Open();
            }
            catch
            {
                //Soms is de connectie niet goed afgesloten en komt er een foutmelding: CON already Open. 
                //Als dat zo is, gewoon doorgaan met code. dus hoeft niet afgevangen te worden.
            }
            command = new OracleCommand("SELECT * FROM Agenda WHERE EigenaarID =(SELECT GebruikerID FROM Gebruiker WHERE Gebruikersnaam =:filter) AND AfspraakDatum =:datum", con);
            command.Parameters.Add(new OracleParameter(":filter", OracleDbType.Varchar2)).Value = filter;
            command.Parameters.Add(new OracleParameter(":datum", OracleDbType.Varchar2)).Value = datum;
            reader = command.ExecuteReader();

            while (reader.Read())
            {
                agendaPunt = new Agenda.AgendaPunt();

                agendaPunt.AfspraakID = Convert.ToInt32(reader["AFSPRAAKID"]);
                agendaPunt.Titel = reader["Titel"].ToString();
                agendaPunt.AfspraakMet = reader["AfspraakMet"].ToString();
                agendaPunt.Beschrijving = reader["Omschrijving"].ToString();
                agendaPunt.AgendaEigenaar = Convert.ToInt32(reader["EigenaarID"]);
                agendaPunt.DatumTijdStart = Convert.ToInt32(reader["StartTijd"]);
                agendaPunt.DatumTijdEind = Convert.ToInt32(reader["EindTijd"]);
                agendaPunt.AfspraakDatum = Convert.ToDateTime(reader["AfspraakDatum"]);

                gebruiker.AgendaPuntToevoegen(agendaPunt);
            }
            con.Close();
        }
        public void AgendaPuntToevoegen(Agenda.AgendaPunt agendaPunt, Gebruiker gebruiker, string datum)
        {
            con.Open();
            command = new OracleCommand("INSERT INTO Agenda(EigenaarID, Omschrijving, StartTijd, EindTijd, Titel, AfspraakMet, AfspraakDatum)" +
                                                "VALUES(:gebruikersid,:beschrijving ,:starttijd ,:eindtijd ,:titel ,:afspraakmet, :datum)", con);

            command.Parameters.Add(new OracleParameter(":gebruikersid", OracleDbType.Int32)).Value = gebruiker.GebruikersID;
            command.Parameters.Add(new OracleParameter(":beschrijving", OracleDbType.Varchar2)).Value = agendaPunt.Beschrijving;
            command.Parameters.Add(new OracleParameter(":starttijd", OracleDbType.Varchar2)).Value = agendaPunt.DatumTijdStart;
            command.Parameters.Add(new OracleParameter(":eindtijd", OracleDbType.Varchar2)).Value = agendaPunt.DatumTijdEind;
            command.Parameters.Add(new OracleParameter(":titel", OracleDbType.Varchar2)).Value = agendaPunt.Titel;
            command.Parameters.Add(new OracleParameter(":afspraakmet", OracleDbType.Varchar2)).Value = agendaPunt.AfspraakMet;
            command.Parameters.Add(new OracleParameter(":datum", OracleDbType.Varchar2)).Value = datum;
            command.ExecuteNonQuery();
            con.Close();
        }
        public void AgendaAanpassen(Gebruiker gebruiker, Agenda.AgendaPunt agendaPunt, string datum)
        {
            con.Open();
            command = new OracleCommand("UPDATE Agenda SET Omschrijving=:beschrijving, StartTijd=:starttijd, EindTijd=:eindtijd, Titel=:titel, AfspraakMet =:afspraakmet, AfspraakDatum =:datum WHERE AfspraakID =:afspraakid", con);
            command.Parameters.Add(new OracleParameter(":beschrijving", OracleDbType.Varchar2)).Value = agendaPunt.Beschrijving;
            command.Parameters.Add(new OracleParameter(":starttijd", OracleDbType.Varchar2)).Value = agendaPunt.DatumTijdStart;
            command.Parameters.Add(new OracleParameter(":eindtijd", OracleDbType.Varchar2)).Value = agendaPunt.DatumTijdEind;
            command.Parameters.Add(new OracleParameter(":titel", OracleDbType.Varchar2)).Value = agendaPunt.Titel;
            command.Parameters.Add(new OracleParameter(":afspraakmet", OracleDbType.Varchar2)).Value = agendaPunt.AfspraakMet;
            command.Parameters.Add(new OracleParameter(":datum", OracleDbType.Varchar2)).Value = datum;
            command.Parameters.Add(new OracleParameter(":afspraakid", OracleDbType.Int32)).Value = agendaPunt.AfspraakID;
            command.ExecuteNonQuery();
            con.Close();

        }
        public void AgendaPuntVerwijderen()
        {

        }
        #endregion
        #region Chat Queries

        public int ChatCheckGelezen(int ontvangerid, int verzenderid)
        {
            int count = 0;
            try { con.Open(); } catch { };
            command = new OracleCommand("SELECT COUNT(*) FROM CHAT WHERE ONTVANGERID =:verzenderid AND VERZENDERID = :ontvangerid AND GELEZEN = 'N' ", con);
            command.Parameters.Add(new OracleParameter("verzenderid", OracleDbType.Int32)).Value = verzenderid;
            command.Parameters.Add(new OracleParameter("onvtvangerid", OracleDbType.Int32)).Value = ontvangerid;
            reader = command.ExecuteReader();

            while (reader.Read())
            {
                count = Convert.ToInt32(reader["COUNT(*)"]);
            }

            con.Close();
            return count;
        }

        public bool ChatNieuwBericht(Gebruiker gebruiker)
        {
            bool nieuwBericht = false;
            try { con.Open(); } catch { };
            command = new OracleCommand("SELECT Gelezen FROM Chat WHERE OntvangerID =:gebruikerID ", con);
            command.Parameters.Add(new OracleParameter("gebruikerid", OracleDbType.Int32)).Value = gebruiker.GebruikersID;
            reader = command.ExecuteReader();

            while (reader.Read())
            {
                if (Convert.ToChar(reader["Gelezen"]) == 'N')
                {
                    nieuwBericht = true;
                }
            }
            con.Close();
            return nieuwBericht;
        }

        //Bericht is gelezen
        public void ChatBerichtGelezen(int berichtid)
        {
            try { con.Open(); } catch { };
            command = new OracleCommand("UPDATE CHAT SET GELEZEN =:STATUS WHERE CHATID =:berichtid", con);
            command.Parameters.Add(new OracleParameter("STATUS", OracleDbType.Char)).Value = "Y";
            command.Parameters.Add(new OracleParameter("berichtid", OracleDbType.Int32)).Value = berichtid;
            command.ExecuteNonQuery();
            con.Close();
        }

        //Geeft de onlinestatus van je chatpartner
        public string ChatPartnerStatus(int id)
        {
            string status = "";

            try { con.Open(); } catch { };
            command = new OracleCommand("SELECT onlinestatus FROM gebruiker WHERE gebruikerid = :id", con);
            command.Parameters.Add(new OracleParameter("id", OracleDbType.Int32)).Value = id;
            reader = command.ExecuteReader();

            while (reader.Read())
            {
                status = reader["onlinestatus"].ToString();
            }

            if (status == "Y")
            {
                con.Close();
                return "Online";
            }

            else
            {
                con.Close();
                return "Offline";
            }
        }

        //Zet de gebruiker online
        public void ChatZetOnline(int gebruikerID)
        {
            try { con.Open(); } catch { };
            command = new OracleCommand("UPDATE Gebruiker SET ONLINESTATUS =:STATUS WHERE GebruikerID =:gebruikerid", con);
            command.Parameters.Add(new OracleParameter("STATUS", OracleDbType.Char)).Value = "Y";
            command.Parameters.Add(new OracleParameter("gebruikerid", OracleDbType.Int32)).Value = gebruikerID;
            command.ExecuteNonQuery();
            con.Close();
        }

        //Zet de gebruiker Offline
        public void ChatZetOffline(int gebruikerID)
        {
            try { con.Open(); } catch { };
            command = new OracleCommand("UPDATE Gebruiker SET ONLINESTATUS =:STATUS WHERE GebruikerID =:gebruikerid", con);
            command.Parameters.Add(new OracleParameter("STATUS", OracleDbType.Char)).Value = "N";
            command.Parameters.Add(new OracleParameter("gebruikerid", OracleDbType.Int32)).Value = gebruikerID;
            command.ExecuteNonQuery();
            con.Close();
        }

        //Geeft een lijst van alle vrijwilligers
        public List<string> VrijwilligersLijst()
        {
            List<string> vrijwilligerlijst;
            vrijwilligerlijst = new List<string>();

            try { con.Open(); } catch { };
            command = new OracleCommand("SELECT Gebruikersnaam FROM gebruiker WHERE rol = 'Vrijwilliger' ORDER BY Gebruikersnaam ASC", con);
            reader = command.ExecuteReader();

            while (reader.Read())
            {
                vrijwilligerlijst.Add(reader["Gebruikersnaam"].ToString());
            }
            con.Close();

            return vrijwilligerlijst;
        }

        //Geeft een lijst van alle hulpbehoevende
        public List<string> HulpbehoevendeLijst()
        {
            List<string> hulpbehoevendelijst;
            hulpbehoevendelijst = new List<string>();

            try { con.Open(); } catch { };
            command = new OracleCommand("SELECT Gebruikersnaam FROM gebruiker WHERE rol = 'Hulpbehoevende'  ORDER BY Gebruikersnaam ASC", con);
            reader = command.ExecuteReader();

            while (reader.Read())
            {
                hulpbehoevendelijst.Add(reader["Gebruikersnaam"].ToString());
            }
            con.Close();

            return hulpbehoevendelijst;
        }

        //Geeft het ID van je chat partner
        public int ChatpartnerID(string naam)
        {
            int id = 0;
            try { con.Open(); } catch { };
            command = new OracleCommand("SELECT GebruikerID FROM gebruiker WHERE gebruikersnaam = :naam", con);
            command.Parameters.Add(new OracleParameter("naam", naam));
            reader = command.ExecuteReader();

            while (reader.Read())
            {
                id = Convert.ToInt32(reader["GEBRUIKERID"].ToString());
            }
            con.Close();
            return id;
        }

        //Voegt een chatbericht toe aan de database
        public void ChatInvoegen(int chatid, string inhoud, int ontvangerID, int verzenderID, string datum)
        {
            int Chatcount = 0;

            try { con.Open(); } catch { };
            command = new OracleCommand("SELECT COUNT(CHATID) as ChatIDCount FROM Chat", con);
            reader = command.ExecuteReader();

            while (reader.Read())
            {
                Chatcount = Convert.ToInt32(reader["ChatIDCount"]);
            }

            if (Chatcount > 0)
            {
                command = new OracleCommand("INSERT INTO Chat(OntvangerID, VerzenderID, BerichtInhoud, Datumtijd) VALUES(:ontvangerid, :verzenderid, :inhoud, TO_TIMESTAMP(:datum,'DD-MON HH24.MI'))", con);
                command.Parameters.Add(new OracleParameter("ontvangerid", OracleDbType.Int32)).Value = ontvangerID;
                command.Parameters.Add(new OracleParameter("verzenderid", OracleDbType.Int32)).Value = verzenderID;
                command.Parameters.Add(new OracleParameter("inhoud", OracleDbType.Varchar2)).Value = inhoud;
                command.Parameters.Add(new OracleParameter("datum", OracleDbType.Varchar2)).Value = datum;

                command.ExecuteNonQuery();
                con.Close();
            }

            else if (Chatcount <= 0)
            {
                command = new OracleCommand("INSERT INTO Chat(OntvangerID, VerzenderID, BerichtInhoud, Datumtijd) VALUES(:ontvangerID, :verzenderid, :inhoud, TO_TIMESTAMP(:datum,'DD-MON HH24.MI'))", con);
                command.Parameters.Add(new OracleParameter("ontvangerid", OracleDbType.Int32)).Value = ontvangerID;
                command.Parameters.Add(new OracleParameter("verzenderid", OracleDbType.Int32)).Value = verzenderID;
                command.Parameters.Add(new OracleParameter("inhoud", OracleDbType.Varchar2)).Value = inhoud;
                command.Parameters.Add(new OracleParameter("datum", OracleDbType.Varchar2)).Value = datum;

                command.ExecuteNonQuery();
                con.Close();
            }

            con.Close();
        }

        //Geeft de lijst van vrijwilligers waar je een open chat mee hebt
        public List<string> BestaandeChatlijstVrijwilligers(int id)
        {
            List<string> vrijwilligerlijst;
            vrijwilligerlijst = new List<string>();

            try { con.Open(); } catch { };
            command = new OracleCommand("SELECT Gebruikersnaam FROM gebruiker WHERE rol = 'Vrijwilliger' AND (GEBRUIKERID IN (SELECT ONTVANGERID FROM CHAT WHERE VERZENDERID = :id OR ONTVANGERID = :id) OR GEBRUIKERID IN (SELECT VERZENDERID FROM CHAT WHERE VERZENDERID = :id OR ONTVANGERID = :id)) ORDER BY Gebruikersnaam ASC ", con);
            command.Parameters.Add(new OracleParameter("id", OracleDbType.Int32)).Value = id;
            reader = command.ExecuteReader();


            while (reader.Read())
            {
                vrijwilligerlijst.Add(reader["Gebruikersnaam"].ToString());
            }
            con.Close();

            return vrijwilligerlijst;
        }

        //Geeft de lijst van hulpbehoevende waar je een open chat mee hebt
        public List<string> BestaandeChatlijstHulpbehoevende(int id)
        {
            List<string> hulpbehoevendelijst;
            hulpbehoevendelijst = new List<string>();

            try { con.Open(); } catch { };
            command = new OracleCommand("SELECT Gebruikersnaam FROM gebruiker WHERE rol = 'Hulpbehoevende' AND (GEBRUIKERID IN (SELECT ONTVANGERID FROM CHAT WHERE VERZENDERID = :id OR ONTVANGERID = :id) OR GEBRUIKERID IN (SELECT VERZENDERID FROM CHAT WHERE VERZENDERID = :id OR ONTVANGERID = :id)) ORDER BY Gebruikersnaam ASC", con);
            command.Parameters.Add(new OracleParameter("id", OracleDbType.Int32)).Value = id;
            reader = command.ExecuteReader();

            while (reader.Read())
            {
                hulpbehoevendelijst.Add(reader["Gebruikersnaam"].ToString());
            }
            con.Close();

            return hulpbehoevendelijst;
        }

        //Geeft een lijst met chatberichten die bestaan tussen perssoon a en persoon b
        public List<Chatbericht> ChatLaden(string partnerNaam, string gebruikerNaam, int partnerID, int gebruikerID)
        {
            List<Chatbericht> berichtenlijst = new List<Chatbericht>();
            try { con.Open(); } catch { };
            command = new OracleCommand("SELECT CHATID, BERICHTINHOUD, DATUMTIJD, VERZENDERID FROM CHAT WHERE (VERZENDERID = :gebruikerID AND ONTVANGERID =  :partnerID) OR (VERZENDERID = :partnerID AND ONTVANGERID = :gebruikerID) ORDER BY CHATID ASC", con);
            command.Parameters.Add(new OracleParameter("partnerID", OracleDbType.Int32)).Value = partnerID;
            command.Parameters.Add(new OracleParameter("gebruikerID", OracleDbType.Int32)).Value = gebruikerID;
            reader = command.ExecuteReader();

            while (reader.Read())
            {
                int verzender = Convert.ToInt32(reader["VERZENDERID"]);

                if (verzender == gebruikerID)
                {
                    Chatbericht bericht = new Chatbericht(reader["BERICHTINHOUD"].ToString(), gebruikerNaam, Convert.ToInt32(reader["CHATID"]), Convert.ToDateTime(reader["DATUMTIJD"]));
                    berichtenlijst.Add(bericht);
                }

                else if (verzender == partnerID)
                {
                    Chatbericht bericht = new Chatbericht(reader["BERICHTINHOUD"].ToString(), partnerNaam, Convert.ToInt32(reader["CHATID"]), Convert.ToDateTime(reader["DATUMTIJD"]));
                    berichtenlijst.Add(bericht);
                }
            }

            con.Close();
            return berichtenlijst;
        }

        //Geeft het hoogste chat id
        public int ControlleerMaxChatID()
        {
            int id = 0;
            try { con.Open(); } catch { };
            command = new OracleCommand("SELECT MAX(CHATID) as MAXID FROM CHAT", con);
            reader = command.ExecuteReader();
            while (reader.Read())
            {
                try
                {
                    id = Convert.ToInt32(reader["MAXID"]);
                }
                catch (InvalidCastException)
                {
                    id = 0;
                }
            }
            con.Close();

            return id;
        }

        #endregion
        #region Beheerder Queries
        //Agenda Queries
        public OracleDataAdapter AgendaBeheer(string query)
        {
            con.Open();
            if (query == "Alles")
            {
                tempString = "SELECT * FROM AGENDA";
            }
            else if (query == "Afspraak")
            {
                tempString = "SELECT TITEL, OMSCHRIJVING FROM AGENDA";
            }
            OracleDataAdapter reader = new OracleDataAdapter(tempString, con);
            con.Close();
            return reader;

        }
        //Chat en Reactie Queries
        public OracleDataAdapter ChatBeheer(string query)
        {
            con.Open();
            if (query == "Alles")
            {
                tempString = "SELECT * FROM GEBRUIKER";
            }
            OracleDataAdapter reader = new OracleDataAdapter(tempString, con);
            con.Close();
            return reader;
        }
        //Gebruiker Queries
        public OracleDataAdapter GebruikerBeheer(string query)
        {
            con.Open();
            if (query == "Alles")
            {
                tempString = "SELECT * FROM GEBRUIKER";
            }
            else if (query == "Naam & Wachtwoord")
            {
                tempString = "SELECT GEBRUIKERSNAAM, WACHTWOORD FROM GEBRUIKER";
            }
            else if (query == "Niet goedgekeurde gebruikers")
            {
                tempString = "SELECT * FROM GEBRUIKER WHERE APPROVED = 'N'";
            }
            else if (query == "Vrijwilligers zonder VOG")
            {
                tempString = "SELECT * FROM GEBRUIKER WHERE ROL = 'vrijwilliger' AND VOG IS NULL";
            }

            OracleDataAdapter reader = new OracleDataAdapter(tempString, con);
            con.Close();
            return reader;
        }
        public OracleDataAdapter DataUpdateBeheerGebruiker(string datagrid)
        {
            con.Open();
            OracleCommand cmd = con.CreateCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "UPDATE GEBRUIKER SET GEBRUIKERSNAAM ='" + datagrid;
            cmd.ExecuteNonQuery();
            OracleDataAdapter reader = new OracleDataAdapter(tempString, con);
            con.Close();


            return reader;
        }

        public OracleDataAdapter DataUpdateBeheerApproved(string datagrid)
        {
            con.Open();
            OracleCommand cmd = con.CreateCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "UPDATE GEBRUIKER SET APPROVED ='" + datagrid;
            cmd.ExecuteNonQuery();
            OracleDataAdapter reader = new OracleDataAdapter(tempString, con);
            con.Close();

            return reader;
        }

        public OracleDataAdapter DataUpdateBeheerRol(string datagrid)
        {
            con.Open();
            OracleCommand cmd = con.CreateCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "UPDATE GEBRUIKER SET ROL ='" + datagrid;
            OracleDataAdapter reader = new OracleDataAdapter(tempString, con);
            cmd.ExecuteNonQuery();
            con.Close();

            return reader;
        }





        //Hulpvraag Queries
        public OracleDataAdapter HulpvraagBeheer(string query)
        {
            con.Open();
            if (query == "Alles")
            {
                tempString = "SELECT * FROM HULPVRAAG";
            }
            else if (query == "Hulpvraag info")
            {
                tempString = "SELECT GEBRUIKERSNAAM, TITEL, FREQUENTIE FROM HULPVRAAG";
            }

            OracleDataAdapter reader = new OracleDataAdapter(tempString, con);
            con.Close();
            return reader;
        }
        #endregion
        #region Gebruiker Queries
        public Gebruiker GebruikerLogin(string naam, string wachtwoord)
        {
            try
            {
                con.Open();
                //Gebruikersnaam zoeken waar gebruikersnaam gelijk is aan de ingevoerde naam + w8woord
                command = new OracleCommand("SELECT * FROM gebruiker WHERE gebruikersnaam = :naam AND wachtwoord = :pw", con);
                command.Parameters.Add(new OracleParameter("naam", naam));
                command.Parameters.Add(new OracleParameter("pw", EncryptString(wachtwoord)));
                reader = command.ExecuteReader();

                while (reader.Read())
                {
                    //Nieuwe gebruiker aanmaken op basis van de rol
                    gebruiker = new Gebruiker();

                    if (reader["ROL"].ToString().ToLower() == "vrijwilliger")
                    {
                        //Kan niet vergelijken met string &char.Database approved column moet naar varchar2.alle gebruikers eerst verwijderen.
                        if (reader["Approved"].ToString() == "Y")
                        {
                            gebruiker.Approved = true;

                        }
                        else
                        {
                            gebruiker.Approved = false;
                        }

                    }
                    //Properties toekennen aan gebruiken.
                    gebruiker.Achternaam = reader["Achternaam"].ToString();
                    gebruiker.Voornaam = reader["Voornaam"].ToString();
                    gebruiker.Wachtwoord = reader["Wachtwoord"].ToString();
                    gebruiker.Gebruikersnaam = reader["Gebruikersnaam"].ToString();
                    gebruiker.GebruikersID = Convert.ToInt32(reader["GebruikerID"]);
                    gebruiker.GebruikerInfo = reader["GebruikerInfo"].ToString();
                    gebruiker.VOG = reader["vog"].ToString();
                    if (reader["HeeftAuto"].ToString() == "Y")
                    {
                        gebruiker.Auto = true;
                    }
                    else
                    {
                        gebruiker.Auto = false;
                    }
                    if (reader["Foto"].ToString() != "")
                    {
                        gebruiker.Pasfoto = gebruiker.GetLocalDropBox() + reader["Foto"].ToString();
                    }
                    gebruiker.Rol = reader["Rol"].ToString();
                }
            }
            catch
            {

            }
            finally
            {
                con.Close();
            }

            return gebruiker;
        }
        public bool GebruikerAccountToevoegen(string Gebruikersnaam, string Wachtwoord, string Rol, string filenameFoto, string filenameVOG, string voornaam, string tussenvoegsel, string achternaam, string geslacht, DateTime geboortedatum)
        {

            try
            {
                con.Open();
                //Hulpbehoevende hoeft geen VOG te inserten.
                if (Rol.ToLower() == "hulpbehoevende")
                {
                    command = new OracleCommand(@"INSERT INTO GEBRUIKER(GEBRUIKERSNAAM, WACHTWOORD, VOORNAAM, TUSSENVOEGSEL, ACHTERNAAM, FOTO, APPROVED, ROL, GEBOORTEDATUM)" +
                                                      "VALUES(:gebruikersnaam, :wachtwoord, :voornaam, :tussenvoegsel, :achternaam, :filenamefoto, :Approved, :Rol, :Geboortedatum)", con);
                    command.Parameters.Add(new OracleParameter(":gebruikersnaam", OracleDbType.Varchar2)).Value = Gebruikersnaam;
                    command.Parameters.Add(new OracleParameter(":wachtwoord", OracleDbType.Varchar2)).Value = EncryptString(Wachtwoord);
                    command.Parameters.Add(new OracleParameter(":voornaam", OracleDbType.Varchar2)).Value = voornaam;
                    command.Parameters.Add(new OracleParameter(":tussenvoegsel", OracleDbType.Varchar2)).Value = tussenvoegsel;
                    command.Parameters.Add(new OracleParameter(":achternaam", OracleDbType.Varchar2)).Value = achternaam;
                    command.Parameters.Add(new OracleParameter(":filenameFoto", OracleDbType.Varchar2)).Value = filenameFoto;
                    command.Parameters.Add(new OracleParameter(":Approved", OracleDbType.Varchar2)).Value = "Y";
                    command.Parameters.Add(new OracleParameter(":Rol", OracleDbType.Varchar2)).Value = Rol;
                    command.Parameters.Add(new OracleParameter(":Geboortedatum", OracleDbType.Date)).Value = geboortedatum;
                }
                //Vrijwilliger wel.
                else
                {
                    command = new OracleCommand(@"INSERT INTO GEBRUIKER(GEBRUIKERSNAAM, WACHTWOORD, VOORNAAM, TUSSENVOEGSEL, ACHTERNAAM, FOTO, APPROVED, ROL, VOG, GEBOORTEDATUM)" +
                                                        "VALUES(:gebruikersnaam, :wachtwoord, :voornaam, :tussenvoegsel, :achternaam, :filenamefoto, :Approved, :Rol, :filenameVOG, :Geboortedatum)", con);
                    command.Parameters.Add(new OracleParameter(":gebruikersnaam", OracleDbType.Varchar2)).Value = Gebruikersnaam;
                    command.Parameters.Add(new OracleParameter(":wachtwoord", OracleDbType.Varchar2)).Value = EncryptString(Wachtwoord);
                    command.Parameters.Add(new OracleParameter(":voornaam", OracleDbType.Varchar2)).Value = voornaam;
                    command.Parameters.Add(new OracleParameter(":tussenvoegsel", OracleDbType.Varchar2)).Value = tussenvoegsel;
                    command.Parameters.Add(new OracleParameter(":achternaam", OracleDbType.Varchar2)).Value = achternaam;
                    command.Parameters.Add(new OracleParameter(":filenameFoto", OracleDbType.Varchar2)).Value = filenameFoto;
                    command.Parameters.Add(new OracleParameter(":Approved", OracleDbType.Varchar2)).Value = "N";
                    command.Parameters.Add(new OracleParameter(":Rol", OracleDbType.Varchar2)).Value = Rol;
                    command.Parameters.Add(new OracleParameter(":filenameVOG", OracleDbType.Varchar2)).Value = filenameVOG;
                    command.Parameters.Add(new OracleParameter(":Geboortedatum", OracleDbType.Date)).Value = geboortedatum;
                }
                command.ExecuteNonQuery();
                return true;
            }
            catch (OracleException)
            {
                return false;
            }
            finally
            {
                con.Close();
            }



        }
        public bool GebruikerControlleerUsername(string Gebruikersnaam)
        {
            try
            {
                con.Open();
                command = new OracleCommand("SELECT Gebruikersnaam FROM GEBRUIKER WHERE Gebruikersnaam =:gebruikersnaam", con);
                command.Parameters.Add(new OracleParameter("Gebruikersnaam", Gebruikersnaam));
                reader = command.ExecuteReader();
                while (reader.Read())
                {
                    tempString = reader["Gebruikersnaam"].ToString();
                }
            }
            catch (OracleException)
            {
                return true;
            }


            con.Close();
            if (tempString == null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public void GebruikerProfielAanpassen(Gebruiker gebruiker, bool wachtwoordChanged, bool fotoChanged)
        {
            if (gebruiker.Auto)
            {
                tempString = "Y";
            }
            else
            {
                tempString = "N";
            }
            //Verschil maken tussen welke info veranderd is. Anders wordt er een encryptie 
            //over encryptie van het wachtwoord gedaan elke keer dat je iets aan het profiel aanpast
            if (fotoChanged)
            {
                command = new OracleCommand("UPDATE Gebruiker SET GebruikerInfo=:info, Foto=:pasfoto, Auto=:temp, Voornaam=:voornaam, Achternaam=:achternaam  WHERE GebruikerID =:gebruikerid", con);
                command.Parameters.Add(new OracleParameter("info", OracleDbType.Varchar2)).Value = gebruiker.GebruikerInfo;
                command.Parameters.Add(new OracleParameter("pasfoto", OracleDbType.Varchar2)).Value = gebruiker.Pasfoto;
                command.Parameters.Add(new OracleParameter("temp", OracleDbType.Char)).Value = Convert.ToChar(tempString);
                command.Parameters.Add(new OracleParameter("voornaam", OracleDbType.Varchar2)).Value = gebruiker.Voornaam;
                command.Parameters.Add(new OracleParameter("achternaam", OracleDbType.Varchar2)).Value = gebruiker.Achternaam;
                command.Parameters.Add(new OracleParameter("gebruikerid", OracleDbType.Int32)).Value = gebruiker.GebruikersID;
            }
            else if (wachtwoordChanged)
            {
                command = new OracleCommand("UPDATE Gebruiker SET Wachtwoord =:password, GebruikerInfo=:info, Auto=:temp, Voornaam=:voornaam, Achternaam=:achternaam WHERE GebruikerID =:gebruikerid", con);
                command.Parameters.Add(new OracleParameter("password", OracleDbType.Varchar2)).Value = EncryptString(gebruiker.Wachtwoord);
                command.Parameters.Add(new OracleParameter("info", OracleDbType.Varchar2)).Value = gebruiker.GebruikerInfo;
                command.Parameters.Add(new OracleParameter("temp", OracleDbType.Char)).Value = Convert.ToChar(tempString);
                command.Parameters.Add(new OracleParameter("voornaam", OracleDbType.Varchar2)).Value = gebruiker.Voornaam;
                command.Parameters.Add(new OracleParameter("achternaam", OracleDbType.Varchar2)).Value = gebruiker.Achternaam;
                command.Parameters.Add(new OracleParameter("gebruikerid", OracleDbType.Int32)).Value = gebruiker.GebruikersID;
            }
            else
            {
                command = new OracleCommand("UPDATE Gebruiker SET GebruikerInfo=:info, Auto=:temp, Voornaam=:voornaam, Achternaam=:achternaam WHERE GebruikerID =:gebruikerid", con);
                command.Parameters.Add(new OracleParameter("info", OracleDbType.Varchar2)).Value = gebruiker.GebruikerInfo;
                command.Parameters.Add(new OracleParameter("temp", OracleDbType.Char)).Value = Convert.ToChar(tempString);
                command.Parameters.Add(new OracleParameter("voornaam", OracleDbType.Varchar2)).Value = gebruiker.Voornaam;
                command.Parameters.Add(new OracleParameter("achternaam", OracleDbType.Varchar2)).Value = gebruiker.Achternaam;
                command.Parameters.Add(new OracleParameter("gebruikerid", OracleDbType.Int32)).Value = gebruiker.GebruikersID;
            }
            command.ExecuteNonQuery();
            con.Close();
        }
        public List<string> GebruikerProfielOpvragen(string gebruikersnaam, Gebruiker gebruiker)
        {
            List<string> ProfielInfo = new List<string>();
            con.Open();
            command = new OracleCommand("SELECT * FROM GEBRUIKER WHERE Gebruikersnaam =:gebruiker", con);
            command.Parameters.Add(new OracleParameter("gebruiker", OracleDbType.Varchar2)).Value = gebruikersnaam;
            reader = command.ExecuteReader();

            while (reader.Read())
            {
                tempString = reader["Auto"].ToString();
                ProfielInfo.Add(tempString);
                tempString = reader["GebruikerInfo"].ToString();
                ProfielInfo.Add(tempString);
                tempString = reader["Achternaam"].ToString();
                ProfielInfo.Add(tempString);
                tempString = reader["Voornaam"].ToString();
                ProfielInfo.Add(tempString);
                tempString = gebruiker.GetLocalDropBox() + reader["Foto"].ToString();
                ProfielInfo.Add(tempString);
            }
            con.Close();

            return ProfielInfo;
        }
        public List<string> GebruikerSelecteerVrijwilligers()
        {
            List<string> vrijwilligersList = new List<string>();
            con.Open();

            command = new OracleCommand("SELECT Gebruikersnaam FROM Gebruiker WHERE LOWER(ROL)='vrijwilliger' ", con);

            reader = command.ExecuteReader();
            while (reader.Read())
            {
                vrijwilligersList.Add(reader["Gebruikersnaam"].ToString());
            }
            return vrijwilligersList;
        }
        #endregion
        public string EncryptString(string toEncrypt)
        {
            SHA256Managed crypt = new SHA256Managed();
            System.Text.StringBuilder hash = new StringBuilder();
            byte[] crypto = crypt.ComputeHash(Encoding.UTF8.GetBytes(toEncrypt), 0, Encoding.UTF8.GetByteCount(toEncrypt));
            foreach (byte theByte in crypto)
            {
                hash.Append(theByte.ToString("x2"));
            }
            return hash.ToString();
        }
    }
}
