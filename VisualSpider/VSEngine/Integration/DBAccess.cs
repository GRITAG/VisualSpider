using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VSEngine.Integration
{
    public class DBAccess
    {
        public SQLiteConnection Connection { get; set; }

        string createURLTable = "create table url (index int, hash varchar(32), url text, datefound text, datescrapped text)";
        string createResolvedURLTable = "create table resolvedurl (index int, hash varchar(32), url text, dateresolved text)";
        string createCollectedURLTable = "create table foundurl (index int, hash varchar(32), url text, datecollected text)";
        string createURLLinkTable = "create table urllink (urlindex int, collectedurl int)";
        string createURLResolvedLinkTable = "create table resolvedurllink (urlindex int, resolvedurl int)";

        public void CreateDB()
        {
            if (File.Exists(Directory.GetCurrentDirectory() + "\\VSResults.db")) File.Delete(Directory.GetCurrentDirectory() + "\\VSResults.db");

            SQLiteConnection.CreateFile(Directory.GetCurrentDirectory() + "\\VSResults.db");

            if (File.Exists(Directory.GetCurrentDirectory() + "\\VSResults.db"))
                throw new Exception("Database could not be found");

            Connection = new SQLiteConnection("Data Source=" + Directory.GetCurrentDirectory() + "\\VSResults.db;Version=3;");
            Connection.Open();
            SQLiteCommand createURL = new SQLiteCommand(createURLTable, Connection);
            createURL.ExecuteNonQuery();
            SQLiteCommand createResolvedURL = new SQLiteCommand(createResolvedURLTable, Connection);
            createResolvedURL.ExecuteNonQuery();
            SQLiteCommand createCollectedURL = new SQLiteCommand(createCollectedURLTable, Connection);
            createCollectedURL.ExecuteNonQuery();
            SQLiteCommand createURLLink = new SQLiteCommand(createURLLinkTable, Connection);
            createURLLink.ExecuteNonQuery();
            SQLiteCommand createURLResolevedLink = new SQLiteCommand(createURLResolvedLinkTable, Connection);
            createURLResolevedLink.ExecuteNonQuery();
        }
    }
}
