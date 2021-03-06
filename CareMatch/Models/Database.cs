﻿using System;
using System.Collections.Generic;
using System.Text;
using Oracle.ManagedDataAccess.Client;
using System.Security.Cryptography;
using System.Data;
using System.Globalization;

namespace CareMatch.Models
{
    public class Database
    {
        private OracleConnection con;
        private OracleCommand command;
        private OracleDataReader reader;

        private Gebruiker gebruiker;
        private Agenda.AgendaPunt agendaPunt;

        private string tempString;

        public Database()
        {
            string constr = "Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=fhictora01.fhict.local)(PORT=1521)))"
                          + "(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=fhictora)));"
                          + "User ID=DBI327544; PASSWORD=CareMatch;";

            con = new OracleConnection(constr);
        }


        #region Hulpvragen Queries
        public void HulpvraagToevoegen(Hulpvraag hulpvraag, Gebruiker gebruiker)
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

            using (command = new OracleCommand(@"INSERT INTO Hulpvraag(GebruikerID, Plaatsnaam, Omschrijving, Urgent, Titel, Locatie, Auto, Flagged, StartDatum, EindDatum) VALUES(:gebruikerid, :plaatsnaam, :hulpvraaginhoud, :temp, :titel, :locatie, :auto, 'N', :startdatum, :einddatum)", con))
            {
                command.Parameters.Add(new OracleParameter(":gebruikerid", OracleDbType.Int32)).Value = gebruiker.GebruikersID;
                command.Parameters.Add(new OracleParameter(":plaatsnaam", OracleDbType.Varchar2)).Value = hulpvraag.Plaatsnaam;
                command.Parameters.Add(new OracleParameter(":hulpvraaginhoud", OracleDbType.Varchar2)).Value = hulpvraag.HulpvraagInhoud;
                command.Parameters.Add(new OracleParameter(":temp", OracleDbType.Varchar2)).Value = tempString; // urgent
                command.Parameters.Add(new OracleParameter(":titel", OracleDbType.Varchar2)).Value = hulpvraag.Titel;
                command.Parameters.Add(new OracleParameter(":locatie", OracleDbType.Varchar2)).Value = hulpvraag.Locatie;
                command.Parameters.Add(new OracleParameter(":auto", OracleDbType.Char)).Value = AutoBenodigd;
                command.Parameters.Add(new OracleParameter(":startdatum", OracleDbType.Varchar2)).Value = hulpvraag.StartDatum;
                command.Parameters.Add(new OracleParameter(":einddatum", OracleDbType.Varchar2)).Value = hulpvraag.EindDatum;
                command.ExecuteNonQuery();
            }

            con.Close();
        }

        public void HulpvraagAannemen(int id, int vrijwilliger)
        {
            con.Open();
            command = new OracleCommand(@"UPDATE Hulpvraag SET VRIJWILLIGERID =:vrijwilligerid WHERE HULPVRAAGID =:hulpvraagid", con);
            command.Parameters.Add(new OracleParameter(":vrijwilligerid", OracleDbType.Int32)).Value = vrijwilliger;
            command.Parameters.Add(new OracleParameter(":hulpvraagid", OracleDbType.Int32)).Value = id;
            reader = command.ExecuteReader();
            con.Close();
        }

        public void HulpvraagRapporteer(Hulpvraag hulpvraag)
        {
            con.Open();
            command = new OracleCommand(@"UPDATE Hulpvraag SET Flagged ='Y' WHERE HulpvraagID = :hulpvraagid", con);
            command.Parameters.Add(new OracleParameter(":hulpvraagid", OracleDbType.Int32)).Value = hulpvraag.HulpvraagID;
            reader = command.ExecuteReader();
            con.Close();
        }
        public void HulpvraagDerapporteer(Hulpvraag hulpvraag)
        {
            con.Open();
            command = new OracleCommand(@"UPDATE Hulpvraag SET Flagged ='N' WHERE HulpvraagID = :hulpvraagid", con);
            command.Parameters.Add(new OracleParameter(":hulpvraagid", OracleDbType.Int32)).Value = hulpvraag.HulpvraagID;
            reader = command.ExecuteReader();
            con.Close();
        }

        public void BeoordelingVerwijderen(Hulpvraag hulpvraag)
        {
            con.Open();
            command = new OracleCommand(@"UPDATE Hulpvraag SET Cijfer = '', Beoordeling ='' WHERE HulpvraagID = :hulpvraagid", con);
            command.Parameters.Add(new OracleParameter(":hulpvraagid", OracleDbType.Int32)).Value = hulpvraag.HulpvraagID;
            reader = command.ExecuteReader();
            con.Close();
        }

        // Mee bezig.
        public void HulpvraagVerwijderen(int hulpvraagID)
        {
            con.Open();

            command = new OracleCommand("DELETE FROM Hulpvraag WHERE HulpvraagID =:id", con);
            command.Parameters.Add(new OracleParameter(":id", OracleDbType.Int32)).Value = hulpvraagID;
            command.ExecuteNonQuery();
            con.Close();
        }

        public void HulpvraagAanpassen(Gebruiker gebruiker, Hulpvraag hulpvraag)
        {
            con.Open();
            string auto;
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
                auto = "Y";
            }
            else
            {
                auto = "N";
            }

            command = new OracleCommand(@"UPDATE Hulpvraag SET Auto=:auto, Plaatsnaam=:plaatsnaam, Locatie=:locatie, Startdatum=:startdatum, Einddatum=:einddatum, Titel=:titel, Omschrijving=:hulpvraaginhoud, Urgent=:temp WHERE HulpvraagID=:hulpvraagid", con);
            command.Parameters.Add(new OracleParameter(":auto", OracleDbType.Varchar2)).Value = auto;
            command.Parameters.Add(new OracleParameter(":plaatsnaam", OracleDbType.Varchar2)).Value = hulpvraag.Plaatsnaam;
            command.Parameters.Add(new OracleParameter(":locatie", OracleDbType.Varchar2)).Value = hulpvraag.Locatie;
            command.Parameters.Add(new OracleParameter(":startdatum", OracleDbType.Varchar2)).Value = hulpvraag.StartDatum;
            command.Parameters.Add(new OracleParameter(":einddatum", OracleDbType.Varchar2)).Value = hulpvraag.EindDatum;
            command.Parameters.Add(new OracleParameter(":titel", OracleDbType.Varchar2)).Value = hulpvraag.Titel;
            command.Parameters.Add(new OracleParameter(":hulpvraaginhoud", OracleDbType.Varchar2)).Value = hulpvraag.HulpvraagInhoud;
            command.Parameters.Add(new OracleParameter(":temp", OracleDbType.Varchar2)).Value = tempString;
            command.Parameters.Add(new OracleParameter(":hulpvraagid", OracleDbType.Int32)).Value = hulpvraag.HulpvraagID;
            command.ExecuteNonQuery();
            con.Close();
        }

        public List<Hulpvraag> HulpvragenOverzicht(Gebruiker gebruiker, string filter)
        {
            List<Hulpvraag> hulpvraagList = new List<Hulpvraag>();
            con.Open();
            if ((string.IsNullOrEmpty(filter) && gebruiker.Rol.ToLower() == "vrijwilliger") || (filter == "Alle hulpvragen" || filter == string.Empty) && gebruiker.Rol.ToLower() == "vrijwilliger")
            {
                // Standaard alle hulpvragen laten zien voor vrijwilligers. - Gerapporteerde hulpvragen niet laten zien. - Gesloten hulpvragen ook niet(waar beoordeling is ingevuld)
                command = new OracleCommand("SELECT Hulpvraag.HulpvraagID, Hulpvraag.Locatie, Hulpvraag.Plaatsnaam, Hulpvraag.Auto, (SELECT Gebruikersnaam FROM Gebruiker WHERE Hulpvraag.GebruikerID = Gebruiker.GebruikerID) as hulpbeh, (SELECT Gebruikersnaam FROM Gebruiker WHERE Hulpvraag.VrijwilligerID = Gebruiker.GebruikerID) as vrijwilliger, Hulpvraag.Omschrijving,  Hulpvraag.startdatum, Hulpvraag.einddatum, Hulpvraag.Urgent, Hulpvraag.Titel, Hulpvraag.BEOORDELING, Hulpvraag.CIJFER, Hulpvraag.BEOORDELINGREACTIE FROM Hulpvraag WHERE Flagged != 'Y'", con);
            }
            else if ((string.IsNullOrEmpty(filter) && gebruiker.Rol.ToLower() == "vrijwilliger") || (filter == "Openstaande hulpvragen" || filter == string.Empty) && gebruiker.Rol.ToLower() == "vrijwilliger")
            {
                // Standaard alle hulpvragen laten zien voor vrijwilligers. - Gerapporteerde hulpvragen niet laten zien. - Gesloten hulpvragen ook niet(waar beoordeling is ingevuld)
                command = new OracleCommand("SELECT Hulpvraag.HulpvraagID, Hulpvraag.Locatie, Hulpvraag.Plaatsnaam, Hulpvraag.Auto, (SELECT Gebruikersnaam FROM Gebruiker WHERE Hulpvraag.GebruikerID = Gebruiker.GebruikerID) as hulpbeh, (SELECT Gebruikersnaam FROM Gebruiker WHERE Hulpvraag.VrijwilligerID = Gebruiker.GebruikerID) as vrijwilliger, Hulpvraag.Omschrijving,  Hulpvraag.startdatum, Hulpvraag.einddatum, Hulpvraag.Urgent, Hulpvraag.Titel, Hulpvraag.BEOORDELING, Hulpvraag.CIJFER, Hulpvraag.BEOORDELINGREACTIE FROM Hulpvraag WHERE Flagged != 'Y' AND VrijwilligerID IS NULL", con);
            }
            else if (!string.IsNullOrEmpty(filter) & gebruiker.Rol.ToLower() == "vrijwilliger" && filter == "Auto")
            {
                // Standaard alle hulpvragen laten zien voor vrijwilligers. - Gerapporteerde hulpvragen niet laten zien. - Gesloten hulpvragen ook niet(waar beoordeling is ingevuld)
                command = new OracleCommand("SELECT Hulpvraag.HulpvraagID, Hulpvraag.Locatie, Hulpvraag.Plaatsnaam, Hulpvraag.Auto, (SELECT Gebruikersnaam FROM Gebruiker WHERE Hulpvraag.GebruikerID = Gebruiker.GebruikerID) as hulpbeh, (SELECT Gebruikersnaam FROM Gebruiker WHERE Hulpvraag.VrijwilligerID = Gebruiker.GebruikerID) as vrijwilliger, Hulpvraag.Omschrijving,  Hulpvraag.startdatum, Hulpvraag.einddatum, Hulpvraag.Urgent, Hulpvraag.Titel, Hulpvraag.BEOORDELING, Hulpvraag.CIJFER, Hulpvraag.BEOORDELINGREACTIE FROM Hulpvraag WHERE Flagged != 'Y' AND Auto = 'Y'", con);
            }
            else if (filter == "Eigen hulpvragen" && gebruiker.Rol.ToLower() == "vrijwilliger")
            {
                // overzicht eigen toegekende hulpvragen voor vrijwilligers
                command = new OracleCommand("SELECT Hulpvraag.HulpvraagID, Hulpvraag.Locatie, Hulpvraag.Plaatsnaam, Hulpvraag.Auto, (SELECT Gebruikersnaam FROM Gebruiker WHERE Hulpvraag.GebruikerID = Gebruiker.GebruikerID) as hulpbeh, (SELECT Gebruikersnaam FROM Gebruiker WHERE Hulpvraag.VrijwilligerID = Gebruiker.GebruikerID) as vrijwilliger, Hulpvraag.Omschrijving,  Hulpvraag.startdatum, Hulpvraag.einddatum, Hulpvraag.Urgent, Hulpvraag.Titel, Hulpvraag.BEOORDELING, Hulpvraag.CIJFER, Hulpvraag.BEOORDELINGREACTIE FROM Hulpvraag WHERE VrijwilligerID=:gebruikerid", con);
                command.Parameters.Add(new OracleParameter(":gebruikerid", OracleDbType.Int32)).Value = gebruiker.GebruikersID;
            }
            else if (filter == "Urgent" && gebruiker.Rol.ToLower() == "vrijwilliger")
            {
                // overzicht eigen toegekende hulpvragen voor vrijwilligers
                command = new OracleCommand("SELECT Hulpvraag.HulpvraagID, Hulpvraag.Locatie, Hulpvraag.Plaatsnaam, Hulpvraag.Auto, (SELECT Gebruikersnaam FROM Gebruiker WHERE Hulpvraag.GebruikerID = Gebruiker.GebruikerID) as hulpbeh, (SELECT Gebruikersnaam FROM Gebruiker WHERE Hulpvraag.VrijwilligerID = Gebruiker.GebruikerID) as vrijwilliger, Hulpvraag.Omschrijving,  Hulpvraag.startdatum, Hulpvraag.einddatum, Hulpvraag.Urgent, Hulpvraag.Titel, Hulpvraag.BEOORDELING, Hulpvraag.CIJFER, Hulpvraag.BEOORDELINGREACTIE FROM Hulpvraag WHERE Urgent='Y'", con);
                command.Parameters.Add(new OracleParameter(":gebruikerid", OracleDbType.Int32)).Value = gebruiker.GebruikersID;
            }
            else if (filter == "Urgent" && gebruiker.Rol.ToLower() == "hulpbehoevende")
            {
                // overzicht eigen toegekende hulpvragen voor vrijwilligers
                command = new OracleCommand("SELECT Hulpvraag.HulpvraagID, Hulpvraag.Locatie, Hulpvraag.Plaatsnaam, Hulpvraag.Auto, (SELECT Gebruikersnaam FROM Gebruiker WHERE Hulpvraag.GebruikerID = Gebruiker.GebruikerID) as hulpbeh, (SELECT Gebruikersnaam FROM Gebruiker WHERE Hulpvraag.VrijwilligerID = Gebruiker.GebruikerID) as vrijwilliger, Hulpvraag.Omschrijving,  Hulpvraag.startdatum, Hulpvraag.einddatum, Hulpvraag.Urgent, Hulpvraag.Titel, Hulpvraag.BEOORDELING, Hulpvraag.CIJFER, Hulpvraag.BEOORDELINGREACTIE FROM Hulpvraag WHERE GebruikerID=:gebruikerid AND Urgent='Y'", con);
                command.Parameters.Add(new OracleParameter(":gebruikerid", OracleDbType.Int32)).Value = gebruiker.GebruikersID;
            }
            else if (filter == "Nieuwe reacties" && gebruiker.Rol.ToLower() == "vrijwilliger")
            {
                // Eigen hulpvragen weergeven waarop een nieuwe reactie is gegeven.
                command = new OracleCommand("SELECT Hulpvraag.HulpvraagID, Hulpvraag.Locatie, Hulpvraag.Plaatsnaam, Hulpvraag.Auto, (SELECT Gebruikersnaam FROM Gebruiker WHERE Hulpvraag.GebruikerID = Gebruiker.GebruikerID) as hulpbeh, (SELECT Gebruikersnaam FROM Gebruiker WHERE Hulpvraag.VrijwilligerID = Gebruiker.GebruikerID) as vrijwilliger, Hulpvraag.Omschrijving,  Hulpvraag.startdatum, Hulpvraag.einddatum, Hulpvraag.Urgent, Hulpvraag.Titel, Hulpvraag.BEOORDELING, Hulpvraag.CIJFER, Hulpvraag.BEOORDELINGREACTIE FROM Hulpvraag WHERE VrijwilligerID=:gebruikerid", con);
                command.Parameters.Add(new OracleParameter(":gebruikerid", OracleDbType.Int32)).Value = gebruiker.GebruikersID;
                command.Parameters.Add(new OracleParameter(":gebruikersnaam", OracleDbType.Varchar2)).Value = gebruiker.Gebruikersnaam;
            }
            else if (filter == "Nieuwe reacties" && gebruiker.Rol.ToLower() == "hulpbehoevende")
            {
                // Hulpbehoevende hulpvragen weergeven waarop een nieuwe reactie is 
                command = new OracleCommand("SELECT Hulpvraag.HulpvraagID,Hulpvraag.Locatie, Hulpvraag.Plaatsnaam, Hulpvraag.Auto, (SELECT Gebruikersnaam FROM Gebruiker WHERE Hulpvraag.GebruikerID = Gebruiker.GebruikerID) as hulpbeh, (SELECT Gebruikersnaam FROM Gebruiker WHERE Hulpvraag.VrijwilligerID = Gebruiker.GebruikerID) as vrijwilliger, Hulpvraag.Omschrijving,  Hulpvraag.startdatum, Hulpvraag.einddatum, Hulpvraag.Urgent, Hulpvraag.Titel, Hulpvraag.startdatum, Hulpvraag.einddatum, Hulpvraag.BEOORDELING, Hulpvraag.CIJFER, Hulpvraag.BEOORDELINGREACTIE FROM Hulpvraag WHERE GebruikerID=:gebruikerid AND LaatstGereageerdDoor !=:gebruikersnaam AND LaatstGereageerdDoor != 'Geen Reacties'", con); // GebruikerID=:gebruikerid AND LaatstGereageerdDoor !=:gebruikersnaam  ERRORRR
                command.Parameters.Add(new OracleParameter(":gebruikerid", OracleDbType.Int32)).Value = gebruiker.GebruikersID;
                command.Parameters.Add(new OracleParameter(":gebruikersnaam", OracleDbType.Varchar2)).Value = gebruiker.Gebruikersnaam;
            }
            else if (filter == "Beoordelingen" && gebruiker.Rol.ToLower() == "vrijwilliger")
            {
                // overzicht eigen toegekende hulpvragen voor vrijwilligers
                command = new OracleCommand("SELECT Hulpvraag.HulpvraagID,Hulpvraag.Locatie, Hulpvraag.Plaatsnaam, Hulpvraag.Auto, (SELECT Gebruikersnaam FROM Gebruiker WHERE Hulpvraag.GebruikerID = Gebruiker.GebruikerID) as hulpbeh, (SELECT Gebruikersnaam FROM Gebruiker WHERE Hulpvraag.VrijwilligerID = Gebruiker.GebruikerID) as vrijwilliger, Hulpvraag.Omschrijving,  Hulpvraag.startdatum, Hulpvraag.einddatum, Hulpvraag.Urgent, Hulpvraag.Titel, Hulpvraag.BEOORDELING, Hulpvraag.CIJFER, Hulpvraag.BEOORDELINGREACTIE FROM Hulpvraag WHERE VrijwilligerID=:gebruikerid AND Beoordeling IS NOT NULL", con);
                command.Parameters.Add(new OracleParameter(":gebruikerid", OracleDbType.Int32)).Value = gebruiker.GebruikersID;
            }
            else if (gebruiker.Rol.ToLower() == "beheerder")
            {
                // Flagged Hulpvragen
                command = new OracleCommand("SELECT Hulpvraag.HulpvraagID, Hulpvraag.Locatie, Hulpvraag.Plaatsnaam, Hulpvraag.Auto, (SELECT Gebruikersnaam FROM Gebruiker WHERE Hulpvraag.GebruikerID = Gebruiker.GebruikerID) as hulpbeh, (SELECT Gebruikersnaam FROM Gebruiker WHERE Hulpvraag.VrijwilligerID = Gebruiker.GebruikerID) as vrijwilliger, Hulpvraag.Omschrijving,  Hulpvraag.startdatum, Hulpvraag.einddatum, Hulpvraag.Urgent, Hulpvraag.Titel, Hulpvraag.BEOORDELING, Hulpvraag.CIJFER, Hulpvraag.BEOORDELINGREACTIE FROM Hulpvraag WHERE Hulpvraag.Flagged = 'Y'", con);
            }
            else
            {
                // Overzicht eigen hulpvragen voor hulpbehoevende.
                command = new OracleCommand("SELECT Hulpvraag.HulpvraagID, Hulpvraag.Locatie, Hulpvraag.Plaatsnaam, Hulpvraag.Auto, (SELECT Gebruikersnaam FROM Gebruiker WHERE Hulpvraag.GebruikerID = Gebruiker.GebruikerID) as hulpbeh, (SELECT Gebruikersnaam FROM Gebruiker WHERE Hulpvraag.VrijwilligerID = Gebruiker.GebruikerID) as vrijwilliger, Hulpvraag.Omschrijving,  Hulpvraag.startdatum, Hulpvraag.einddatum, Hulpvraag.Urgent, Hulpvraag.Titel, Hulpvraag.BEOORDELING, Hulpvraag.CIJFER, Hulpvraag.BEOORDELINGREACTIE FROM Hulpvraag WHERE (SELECT Gebruikersnaam FROM Gebruiker WHERE Hulpvraag.GebruikerID = Gebruiker.GebruikerID)=:gebruikersnaam", con);
                command.Parameters.Add(new OracleParameter(":gebruikersnaam", OracleDbType.Varchar2)).Value = gebruiker.Gebruikersnaam;
            }

            reader = command.ExecuteReader();
            while (reader.Read())
            {
                Hulpvraag hulpvraag = new Hulpvraag();

                hulpvraag.HulpvraagID = Convert.ToInt32(reader["HulpvraagID"]);
                hulpvraag.Titel = reader["Titel"].ToString();
                hulpvraag.Hulpbehoevende = reader["hulpbeh"].ToString();
                hulpvraag.Vrijwilliger = reader["vrijwilliger"].ToString();
                if(hulpvraag.Vrijwilliger == null)
                {
                    hulpvraag.Vrijwilliger = "";
                }
                hulpvraag.HulpvraagInhoud = reader["Omschrijving"].ToString();
                hulpvraag.StartDatum = Convert.ToDateTime(reader["startdatum"]);
                hulpvraag.EindDatum = Convert.ToDateTime(reader["einddatum"]);
                hulpvraag.Plaatsnaam = reader["Plaatsnaam"].ToString();
                hulpvraag.Locatie = reader["locatie"].ToString();
                hulpvraag.Beoordeling = reader["BEOORDELING"].ToString();
                hulpvraag.Cijfer = reader["CIJFER"].ToString();
                hulpvraag.ReactieBeoordeling = reader["BEOORDELINGREACTIE"].ToString();
                if (reader["Urgent"].ToString() == "Y")
                {
                    hulpvraag.Urgent = true;
                }
                else
                {
                    hulpvraag.Urgent = false;
                }

                if (reader["Auto"].ToString() == "Y")
                {
                    hulpvraag.Auto = true;
                }
                else
                {
                    hulpvraag.Auto = false;
                }

                hulpvraagList.Add(hulpvraag);
            }

            con.Close();

            return hulpvraagList;
        }

        public string HulpvraagProfielFoto(Gebruiker gebruiker, Hulpvraag hulpvraag, string rol)
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
                    if (reader["Foto"].ToString() != string.Empty)
                    {
                        tempString = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\DropBox\CareMatch\" + hulpvraag.Hulpbehoevende + "\\" + reader["Foto"].ToString();
                    }
                }
                else if (rol == "vrijwilliger")
                {
                    if (reader["Foto"].ToString() != string.Empty)
                    {
                        tempString = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\DropBox\CareMatch\" + hulpvraag.Vrijwilliger + "\\" + reader["Foto"].ToString();
                    }
                }
            }

            con.Close();
            return tempString;
        }

        public void HulpvraagBeoordelingToevoegen(Hulpvraag hulpvraag)
        {
            con.Open();
            command = new OracleCommand(@"UPDATE Hulpvraag SET BEOORDELING =:beoordeling, Cijfer =:cijfer WHERE HulpvraagID=:hulpvraagid", con);
            command.Parameters.Add(new OracleParameter(":beoordeling", OracleDbType.Varchar2)).Value = hulpvraag.Beoordeling;
            command.Parameters.Add(new OracleParameter(":cijfer", OracleDbType.Int32)).Value = hulpvraag.Cijfer;
            command.Parameters.Add(new OracleParameter(":hulpvraagid", OracleDbType.Int32)).Value = hulpvraag.HulpvraagID;
            command.ExecuteNonQuery();
            con.Close();
        }

        public void HulpvraagReactieBeoordelingToevoegen(Hulpvraag hulpvraag)
        {
            con.Open();
            command = new OracleCommand(@"UPDATE Hulpvraag SET BEOORDELINGREACTIE =:beoordeling WHERE HulpvraagID=:hulpvraagid", con);
            command.Parameters.Add(new OracleParameter(":beoordeling", OracleDbType.Varchar2)).Value = hulpvraag.ReactieBeoordeling;
            command.Parameters.Add(new OracleParameter(":hulpvraagid", OracleDbType.Int32)).Value = hulpvraag.HulpvraagID;
            command.ExecuteNonQuery();
            con.Close();
        }
        #endregion
        #region Agenda Queries        
        public List<Agenda.AgendaPunt> AgendaOverzicht(Gebruiker gebruiker)
        {
            List<Agenda.AgendaPunt> agendaPuntList = new List<Agenda.AgendaPunt>();
            try
            {
                con.Open();
            }
            catch
            {
                // Soms is de connectie niet goed afgesloten en komt er een foutmelding: CON already Open. 
                // Als dat zo is, gewoon doorgaan met code. dus hoeft niet afgevangen te worden.
            }
            command = new OracleCommand("SELECT * FROM Agenda WHERE VrijwilligerID =(SELECT GebruikerID FROM Gebruiker WHERE GebruikerID = :gebruikerid)", con);
            command.Parameters.Add(new OracleParameter(":gebruikerid", OracleDbType.Varchar2)).Value = gebruiker.GebruikersID;
            reader = command.ExecuteReader();

            while (reader.Read())
            {
                agendaPunt = new Agenda.AgendaPunt();

                agendaPunt.Titel = reader["Titel"].ToString();
                agendaPunt.Beschrijving = reader["Inhoud"].ToString();
                agendaPunt.AgendaEigenaar = Convert.ToInt32(reader["VrijwilligerID"]);
                agendaPunt.DatumTijdStart = Convert.ToDateTime(reader["StartTijd"]);
                agendaPunt.DatumTijdEind = Convert.ToDateTime(reader["EindTijd"]);
                agendaPunt.AfspraakDatum = Convert.ToDateTime(reader["AfspraakDatum"]);

                agendaPuntList.Add(agendaPunt);
            }

            con.Close();
            return agendaPuntList;
        }
        public void AgendaPuntToevoegen(Agenda.AgendaPunt agendaPunt, Gebruiker gebruiker)
        {
            con.Open();
            command = new OracleCommand("INSERT INTO Agenda(VrijwilligerID, Inhoud, StartTijd, EindTijd, Titel, AfspraakDatum)" +
                                                "VALUES(:gebruikersid,:beschrijving ,:starttijd ,:eindtijd ,:titel , :datum)", con);

            command.Parameters.Add(new OracleParameter(":gebruikersid", OracleDbType.Int32)).Value = gebruiker.GebruikersID;
            command.Parameters.Add(new OracleParameter(":beschrijving", OracleDbType.Varchar2)).Value = agendaPunt.Beschrijving;
            command.Parameters.Add(new OracleParameter(":starttijd", OracleDbType.Varchar2)).Value = agendaPunt.DatumTijdStart;
            command.Parameters.Add(new OracleParameter(":eindtijd", OracleDbType.Varchar2)).Value = agendaPunt.DatumTijdEind;
            command.Parameters.Add(new OracleParameter(":titel", OracleDbType.Varchar2)).Value = agendaPunt.Titel;
            command.Parameters.Add(new OracleParameter(":datum", OracleDbType.Varchar2)).Value = agendaPunt.AfspraakDatum;
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

        #endregion
        #region Chat Queries

        public int ChatCheckGelezen(int ontvangerid, int verzenderid)
        {
            int count = 0;
            try { con.Open(); }
            catch { }
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

        public string FotoPartner(string partner)
        {
            string foto = string.Empty;
            try { con.Open(); }
            catch { }
            command = new OracleCommand("SELECT FOTO FROM GEBRUIKER WHERE GEBRUIKERSNAAM =: partner", con);
            command.Parameters.Add(new OracleParameter("partner", OracleDbType.Varchar2)).Value = partner;
            reader = command.ExecuteReader();

            while (reader.Read())
            {
                foto = reader["FOTO"].ToString();
            }

            con.Close();
            return foto;

        }

        public bool ChatNieuwBericht(Gebruiker gebruiker)
        {
            bool nieuwBericht = false;
            try { con.Open(); }
            catch { }
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

        // Bericht is gelezen
        public void ChatBerichtGelezen(int berichtid)
        {

            con.Open();
            command = new OracleCommand("UPDATE CHAT SET GELEZEN =:STATUS WHERE CHATID =:berichtid", con);
            command.Parameters.Add(new OracleParameter(":STATUS", OracleDbType.Char)).Value = "Y";
            command.Parameters.Add(new OracleParameter(":berichtid", OracleDbType.Int32)).Value = berichtid;
            command.ExecuteNonQuery();
            con.Close();

        }

        // Geeft de onlinestatus van je chatpartner
        public string ChatPartnerStatus(int id)
        {
            string status = string.Empty;

            try { con.Open(); }
            catch { }
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

        // Zet de gebruiker online
        public void ChatZetOnline(int gebruikerID)
        {
            try { con.Open(); }
            catch { }
            command = new OracleCommand("UPDATE Gebruiker SET \"Online\" =:STATUS WHERE GebruikerID =:gebruikerid", con);
            command.Parameters.Add(new OracleParameter("STATUS", OracleDbType.Char)).Value = "Y";
            command.Parameters.Add(new OracleParameter("gebruikerid", OracleDbType.Int32)).Value = gebruikerID;
            command.ExecuteNonQuery();
            con.Close();
        }

        // Zet de gebruiker Offline
        public void ChatZetOffline(int gebruikerID)
        {
            try { con.Open(); }
            catch { }
            command = new OracleCommand("UPDATE Gebruiker SET ONLINESTATUS =:STATUS WHERE GebruikerID =:gebruikerid", con);
            command.Parameters.Add(new OracleParameter("STATUS", OracleDbType.Char)).Value = "N";
            command.Parameters.Add(new OracleParameter("gebruikerid", OracleDbType.Int32)).Value = gebruikerID;
            command.ExecuteNonQuery();
            con.Close();
        }

        // Geeft een lijst van alle vrijwilligers
        public List<string> VrijwilligersLijst()
        {
            List<string> vrijwilligerlijst;
            vrijwilligerlijst = new List<string>();

            try { con.Open(); }
            catch { }
            command = new OracleCommand("SELECT Gebruikersnaam FROM gebruiker WHERE rol = 'vrijwilliger' ORDER BY Gebruikersnaam ASC", con);
            reader = command.ExecuteReader();

            while (reader.Read())
            {
                vrijwilligerlijst.Add(reader["Gebruikersnaam"].ToString());
            }

            con.Close();

            return vrijwilligerlijst;
        }

        // Geeft een lijst van alle hulpbehoevende
        public List<string> HulpbehoevendeLijst()
        {
            List<string> hulpbehoevendelijst;
            hulpbehoevendelijst = new List<string>();

            try { con.Open(); }
            catch { }
            command = new OracleCommand("SELECT Gebruikersnaam FROM gebruiker WHERE rol = 'hulpbehoevende'  ORDER BY Gebruikersnaam ASC", con);
            reader = command.ExecuteReader();

            while (reader.Read())
            {
                hulpbehoevendelijst.Add(reader["Gebruikersnaam"].ToString());
            }

            con.Close();

            return hulpbehoevendelijst;
        }

        // Geeft het ID van je chat partner
        public int ChatpartnerID(string naam)
        {
            int id = 0;
            try { con.Open(); }
            catch { }
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

        public string ChatpartnerNaam(int id)
        {
            string naam = string.Empty;
            try { con.Open(); }
            catch { }
            command = new OracleCommand("SELECT Gebruikersnaam FROM gebruiker WHERE GebruikerID = :id", con);
            command.Parameters.Add(new OracleParameter("id", id));
            reader = command.ExecuteReader();

            while (reader.Read())
            {
                naam = reader["GEBRUIKERSNAAM"].ToString();
            }

            con.Close();
            return naam;
        }

        // Voegt een chatbericht toe aan de database
        public void ChatInvoegen(int chatid, string inhoud, int ontvangerID, int verzenderID, string datum)
        {
            int Chatcount = 0;

            try { con.Open(); }
            catch { }
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

        // Geeft de lijst van vrijwilligers waar je een open chat mee hebt
        public List<string> BestaandeChatlijstVrijwilligers(int id)
        {
            List<string> vrijwilligerlijst;
            vrijwilligerlijst = new List<string>();

            try { con.Open(); }
            catch { }
            command = new OracleCommand("SELECT Gebruikersnaam FROM gebruiker WHERE rol = 'vrijwilliger' AND (GEBRUIKERID IN (SELECT ONTVANGERID FROM CHAT WHERE VERZENDERID = :id OR ONTVANGERID = :id) OR GEBRUIKERID IN (SELECT VERZENDERID FROM CHAT WHERE VERZENDERID = :id OR ONTVANGERID = :id)) ORDER BY Gebruikersnaam ASC ", con);
            command.Parameters.Add(new OracleParameter("id", OracleDbType.Int32)).Value = id;
            reader = command.ExecuteReader();


            while (reader.Read())
            {
                vrijwilligerlijst.Add(reader["Gebruikersnaam"].ToString());
            }

            con.Close();

            return vrijwilligerlijst;
        }

        // Geeft de lijst van hulpbehoevende waar je een open chat mee hebt
        public List<string> BestaandeChatlijstHulpbehoevende(int id)
        {
            List<string> hulpbehoevendelijst;
            hulpbehoevendelijst = new List<string>();

            try { con.Open(); }
            catch { }
            command = new OracleCommand("SELECT Gebruikersnaam FROM gebruiker WHERE rol = 'hulpbehoevende' AND (GEBRUIKERID IN (SELECT ONTVANGERID FROM CHAT WHERE VERZENDERID = :id OR ONTVANGERID = :id) OR GEBRUIKERID IN (SELECT VERZENDERID FROM CHAT WHERE VERZENDERID = :id OR ONTVANGERID = :id)) ORDER BY Gebruikersnaam ASC", con);
            command.Parameters.Add(new OracleParameter("id", OracleDbType.Int32)).Value = id;
            reader = command.ExecuteReader();

            while (reader.Read())
            {
                hulpbehoevendelijst.Add(reader["Gebruikersnaam"].ToString());
            }

            con.Close();

            return hulpbehoevendelijst;
        }

        // Geeft een lijst met chatberichten die bestaan tussen perssoon a en persoon b
        public List<Chatbericht> ChatLaden(string partnerNaam, string gebruikerNaam, int partnerID, int gebruikerID)
        {
            List<Chatbericht> berichtenlijst = new List<Chatbericht>();
            try { con.Open(); }
            catch { }
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

        // Geeft het hoogste chat id
        public int ControlleerMaxChatID()
        {
            int id = 0;
            try { con.Open(); }
            catch { }
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
        // Agenda Queries
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

        // Chat en Reactie Queries
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

        // Gebruiker Queries
        public List<Gebruiker> GebruikerBeheer(string query)
        {
            con.Open();
            if (query == "Alles")
            {
                tempString = "SELECT * FROM GEBRUIKER";
            }
            else if (query == "Niet goedgekeurde gebruikers")
            {
                tempString = "SELECT * FROM GEBRUIKER WHERE APPROVED = 'N'";
            }
            else if (query == "Vrijwilligers zonder VOG")
            {
                tempString = "SELECT * FROM GEBRUIKER WHERE ROL = 'vrijwilliger' AND VOG IS NULL";
            }
            else if (query == "Vrijwilligers")
            {
                tempString = "SELECT * FROM GEBRUIKER WHERE ROL = 'vrijwilliger'";
            }
            else if (query == "Hulpbehoevenden")
            {
                tempString = "SELECT * FROM GEBRUIKER WHERE ROL = 'hulpbehoevende'";
            }

            command = new OracleCommand(tempString, con);
            reader = command.ExecuteReader();


            List<Gebruiker> gebruikerlist = new List<Gebruiker>();

            while (reader.Read())
            {
                bool tempbool;
                Gebruiker tempGebruiker = new Gebruiker();
                tempGebruiker.Gebruikersnaam = reader["gebruikersnaam"].ToString();
                tempGebruiker.GebruikersID = Convert.ToInt32(reader["GebruikerID"]);
                tempGebruiker.Voornaam = Convert.ToString(reader["Voornaam"]);
                tempGebruiker.Achternaam = Convert.ToString(reader["Achternaam"]);
                tempGebruiker.Rol = reader["Rol"].ToString();
                tempGebruiker.VOG = Convert.ToString(reader["VOG"]);
                if (Convert.ToString(reader["Approved"]) == "Y")
                {
                    tempbool = true;
                }
                else
                {
                    tempbool = false;
                }

                tempGebruiker.Approved = tempbool;
                gebruikerlist.Add(tempGebruiker);
            }

            con.Close();
            return gebruikerlist;
        }

        /// <summary>
        /// deletes the user based on user ID
        /// </summary>
        /// <param name="id"></param>
        public void VerwijderGebruiker(int id)
        {
            con.Open();

            command = new OracleCommand("DELETE FROM Gebruiker WHERE GebruikerID =:id", con);
            command.Parameters.Add(new OracleParameter(":id", OracleDbType.Int32)).Value = id;
            command.ExecuteNonQuery();
            con.Close();
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

        public void DataUpdateBeheerApproved(int gebruikerID)
        {
            // set gebruiker als approved
            con.Open();
            OracleCommand cmd = con.CreateCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "UPDATE GEBRUIKER SET APPROVED = 'Y' WHERE GebruikerID = :gebruikerID";
            cmd.Parameters.Add(new OracleParameter(":gebruikerID", OracleDbType.Int32)).Value = gebruikerID;
            cmd.ExecuteNonQuery();            
        }

        public void DataUpdateBeheerRol(int gebruikerID)
        {
            // set gebruiker als beheerder
            con.Open();
            OracleCommand cmd = con.CreateCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "UPDATE GEBRUIKER SET ROL = 'Beheerder' WHERE GebruikerID = :gebruikerID";
            cmd.Parameters.Add(new OracleParameter(":gebruikerID", OracleDbType.Int32)).Value = gebruikerID;
            cmd.ExecuteNonQuery();
        }
        public void ResetWachtwoord(int gebruikerID)
        {
            con.Open();
            OracleCommand cmd = con.CreateCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "UPDATE GEBRUIKER SET Wachtwoord = :wachtwoord WHERE GebruikerID = :gebruikerID";
            command.Parameters.Add(new OracleParameter(":gebruikerID", OracleDbType.Int32)).Value = gebruikerID;
            command.Parameters.Add(new OracleParameter(":wachtwoord", OracleDbType.Varchar2)).Value = EncryptString("wachtwoord");
            cmd.ExecuteNonQuery();
        }

        // Hulpvraag Queries
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
            con.Open();

            // Gebruikersnaam zoeken waar gebruikersnaam gelijk is aan de ingevoerde naam + w8woord
            command = new OracleCommand("SELECT * FROM gebruiker WHERE gebruikersnaam = :naam AND wachtwoord = :pw", con);
            command.Parameters.Add(new OracleParameter("naam", naam));
            command.Parameters.Add(new OracleParameter("pw", EncryptString(wachtwoord)));
            reader = command.ExecuteReader();

            while (reader.Read())
            {
                // Nieuwe gebruiker aanmaken op basis van de rol
                gebruiker = new Gebruiker();

                if (reader["ROL"].ToString().ToLower() == "vrijwilliger")
                {
                    // Kan niet vergelijken met string &char.Database approved column moet naar varchar2.alle gebruikers eerst verwijderen.
                    if (reader["Approved"].ToString() == "Y")
                    {
                        gebruiker.Approved = true;
                    }
                    else
                    {
                        gebruiker.Approved = false;
                    }
                }

                // Properties toekennen aan gebruiken.
                gebruiker.Achternaam = reader["Achternaam"].ToString();
                gebruiker.Voornaam = reader["Voornaam"].ToString();
                gebruiker.Wachtwoord = reader["Wachtwoord"].ToString();
                gebruiker.Gebruikersnaam = reader["Gebruikersnaam"].ToString();
                gebruiker.GebruikersID = Convert.ToInt32(reader["GebruikerID"]);
                gebruiker.GebruikerInfo = reader["GebruikerInfo"].ToString();
                gebruiker.VOG = reader["vog"].ToString();
                gebruiker.Rol = reader["Rol"].ToString();
                if (reader["HeeftAuto"].ToString().Trim() == "Y")
                {
                    gebruiker.Auto = true;
                }
                else
                {
                    gebruiker.Auto = false;
                }

                if (reader["Foto"].ToString() != string.Empty)
                {
                    gebruiker.Pasfoto = reader["Foto"].ToString();
                }
            }
            con.Close();

            return gebruiker;
        }

        public bool GebruikerAccountToevoegen(string Gebruikersnaam, string Wachtwoord, string Rol, string filenameFoto, string filenameVOG, string voornaam, string tussenvoegsel, string achternaam, string geslacht, DateTime geboortedatum)
        {
            try
            {
                con.Open();

                // Hulpbehoevende hoeft geen VOG te inserten.
                if (Rol.ToLower() == "hulpbehoevende")
                {
                    command = new OracleCommand(@"INSERT INTO GEBRUIKER(GEBRUIKERSNAAM, WACHTWOORD, VOORNAAM, TUSSENVOEGSEL, ACHTERNAAM, FOTO, APPROVED, ROL, GEBOORTEDATUM, FLAGGED)" + "VALUES(:gebruikersnaam, :wachtwoord, :voornaam, :tussenvoegsel, :achternaam, :filenamefoto, :Approved, :Rol, :Geboortedatum, 'N' )", con);
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
                else
                {
                    // Vrijwilliger wel.
                    command = new OracleCommand(@"INSERT INTO GEBRUIKER(GEBRUIKERSNAAM, WACHTWOORD, VOORNAAM, TUSSENVOEGSEL, ACHTERNAAM, FOTO, APPROVED, ROL, VOG, GEBOORTEDATUM)" + "VALUES(:gebruikersnaam, :wachtwoord, :voornaam, :tussenvoegsel, :achternaam, :filenamefoto, 'N', :rol, :filenameVOG, :Geboortedatum)", con);
                    command.Parameters.Add(new OracleParameter(":gebruikersnaam", OracleDbType.Varchar2)).Value = Gebruikersnaam;
                    command.Parameters.Add(new OracleParameter(":wachtwoord", OracleDbType.Varchar2)).Value = EncryptString(Wachtwoord);
                    command.Parameters.Add(new OracleParameter(":voornaam", OracleDbType.Varchar2)).Value = voornaam;
                    command.Parameters.Add(new OracleParameter(":tussenvoegsel", OracleDbType.Varchar2)).Value = tussenvoegsel;
                    command.Parameters.Add(new OracleParameter(":achternaam", OracleDbType.Varchar2)).Value = achternaam;
                    command.Parameters.Add(new OracleParameter(":filenameFoto", OracleDbType.Varchar2)).Value = filenameFoto;
                    command.Parameters.Add(new OracleParameter(":rol", OracleDbType.Varchar2)).Value = Rol;
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
            con.Open();

            // Verschil maken tussen welke info veranderd is. Anders wordt er een encryptie 

            // over encryptie van het wachtwoord gedaan elke keer dat je iets aan het profiel aanpast
            if (fotoChanged)
            {
                command = new OracleCommand("UPDATE Gebruiker SET GebruikerInfo=:info, Foto=:pasfoto, HeeftAuto=:temp, Voornaam=:voornaam, Achternaam=:achternaam  WHERE GebruikerID =:gebruikerid", con);
                command.Parameters.Add(new OracleParameter("info", OracleDbType.Varchar2)).Value = gebruiker.GebruikerInfo;
                command.Parameters.Add(new OracleParameter("pasfoto", OracleDbType.Varchar2)).Value = gebruiker.Pasfoto;
                if (gebruiker.Auto == true)
                {
                    command.Parameters.Add(new OracleParameter("temp", OracleDbType.Char)).Value = "Y".Trim();
                }
                else
                {
                    command.Parameters.Add(new OracleParameter("temp", OracleDbType.Char)).Value = "N".Trim();
                }

                command.Parameters.Add(new OracleParameter("voornaam", OracleDbType.Varchar2)).Value = gebruiker.Voornaam;
                command.Parameters.Add(new OracleParameter("achternaam", OracleDbType.Varchar2)).Value = gebruiker.Achternaam;
                command.Parameters.Add(new OracleParameter("gebruikerid", OracleDbType.Int32)).Value = gebruiker.GebruikersID;
            }
            else if (wachtwoordChanged)
            {
                command = new OracleCommand("UPDATE Gebruiker SET Wachtwoord =:password, GebruikerInfo=:info, HeeftAuto=:temp, Voornaam=:voornaam, Achternaam=:achternaam WHERE GebruikerID =:gebruikerid", con);
                command.Parameters.Add(new OracleParameter("password", OracleDbType.Varchar2)).Value = EncryptString(gebruiker.Wachtwoord);
                command.Parameters.Add(new OracleParameter("info", OracleDbType.Varchar2)).Value = gebruiker.GebruikerInfo;
                if (gebruiker.Auto == true)
                {
                    command.Parameters.Add(new OracleParameter("temp", OracleDbType.Char)).Value = "Y".Trim();
                }
                else
                {
                    command.Parameters.Add(new OracleParameter("temp", OracleDbType.Char)).Value = "N".Trim();
                }

                command.Parameters.Add(new OracleParameter("voornaam", OracleDbType.Varchar2)).Value = gebruiker.Voornaam;
                command.Parameters.Add(new OracleParameter("achternaam", OracleDbType.Varchar2)).Value = gebruiker.Achternaam;
                command.Parameters.Add(new OracleParameter("gebruikerid", OracleDbType.Int32)).Value = gebruiker.GebruikersID;
            }
            else
            {
                command = new OracleCommand("UPDATE Gebruiker SET GebruikerInfo=:info, HeeftAuto=:temp, Voornaam=:voornaam, Achternaam=:achternaam WHERE GebruikerID =:gebruikerid", con);
                command.Parameters.Add(new OracleParameter("info", OracleDbType.Varchar2)).Value = gebruiker.GebruikerInfo;
                if (gebruiker.Auto == true)
                {
                    command.Parameters.Add(new OracleParameter("temp", OracleDbType.Char)).Value = "Y".Trim();
                }
                else
                {
                    command.Parameters.Add(new OracleParameter("temp", OracleDbType.Char)).Value = "N".Trim();
                }

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


        public Gebruiker GebruikerInfoOpvragen(string gebruikersnaam)
        {
            Gebruiker gebruikerinfo = new Gebruiker();
            con.Open();
            command = new OracleCommand("SELECT * FROM GEBRUIKER WHERE Gebruikersnaam =:gebruiker", con);
            command.Parameters.Add(new OracleParameter("gebruiker", OracleDbType.Varchar2)).Value = gebruikersnaam;
            reader = command.ExecuteReader();

            while (reader.Read())
            {
                gebruikerinfo.Achternaam = reader["Achternaam"].ToString();
                gebruikerinfo.Tussenvoegsel = reader["Tussenvoegsel"].ToString();
                gebruikerinfo.GebruikerInfo = reader["GEBRUIKERINFO"].ToString();
                gebruikerinfo.Voornaam = reader["Voornaam"].ToString();
                gebruikerinfo.Pasfoto = reader["FOTO"].ToString();
                gebruikerinfo.GeboorteDatum = Convert.ToDateTime(reader["Geboortedatum"]);
            }

            con.Close();

            return gebruikerinfo;
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
            try
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
            catch
            {

            }
            return string.Empty;
        }
    }
}
