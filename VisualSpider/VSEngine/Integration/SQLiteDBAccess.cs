using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using VSEngine.Data;

namespace VSEngine.Integration
{
    public class SQLiteDBAccess : DBAccess
    {
        public SQLiteConnection Connection { get; set; }

        string createNavUnitTable = "CREATE TABLE navunit (count int, id VARCHAR, address text, addresshash varchar(32), " +
            "timefound text, navtype text, scriptref text);";
        string createNavUnitResolvedTable = "create table navunitresolved (count int, id text, address text, addresshash varchar(32), " +
            "timefound text, navtype text, scriptref text, contenthash varchar(32), timescrapped text, resolvedaddress text, image blob)";
        string createNavUnitLinks = "create table navunitlinks (count int, navunitid text, linkedunitid text)";
        string createScriptErrorsTable = "create table scripterrors (count int, navunitidex int, error text)";
        string createNavigationErrorsTable = "create table navigationerrors (count int, navunitindex int, error text)";

        private void CreateDB(string file)
        {
            if (File.Exists(Directory.GetCurrentDirectory() + "\\VSResults.db")) File.Delete(Directory.GetCurrentDirectory() + "\\VSResults.db");

            SQLiteConnection.CreateFile(Directory.GetCurrentDirectory() + "\\VSResults.db");

            if (!File.Exists(Directory.GetCurrentDirectory() + "\\VSResults.db"))
                throw new Exception("Database could not be found");

            Connection = new SQLiteConnection("Data Source=" + Directory.GetCurrentDirectory() + "\\VSResults.db;Version=3;");
            Connection.Open();
            SQLiteCommand createNavUnit = new SQLiteCommand(createNavUnitTable, Connection);
            createNavUnit.ExecuteNonQuery();
            SQLiteCommand createNavUnitResolved = new SQLiteCommand(createNavUnitResolvedTable, Connection);
            createNavUnitResolved.ExecuteNonQuery();
            SQLiteCommand createNavUnitLink = new SQLiteCommand(createNavUnitLinks, Connection);
            createNavUnitLink.ExecuteNonQuery();
            SQLiteCommand createScriptErrors = new SQLiteCommand(createScriptErrorsTable, Connection);
            createScriptErrors.ExecuteNonQuery();
            SQLiteCommand createNavErrors = new SQLiteCommand(createNavigationErrorsTable, Connection);
            createNavErrors.ExecuteNonQuery();
        }

        public override void ConnectDB(string path)
        {
            if (!File.Exists(Directory.GetCurrentDirectory() + "\\" + path))
                CreateDB(path);

            Connection = new SQLiteConnection("Data Source=" + Directory.GetCurrentDirectory() + "\\VSResults.db;Version=3;");
            Connection.Open();
        }

        public override string StoreNavUnit(NavUnit unit)
        {
            if (!CheckForMatchUnit(unit.Address.ToString()))
            {
                if (string.IsNullOrEmpty(unit.ScriptRef)) unit.ScriptRef = "none";

                SQLiteCommand storeUnit = new SQLiteCommand("insert into navunit (id, address, addresshash, timefound, navtype, scriptref) " + "values ('" + unit.ID.ToString() +
                    "', \"" + unit.Address.ToString() + "\", '" + unit.AddressHash + "', '" + unit.TimeFound.ToString() + "', '" + unit.Type.ToString() + "', '" + unit.ScriptRef +
                    "')", Connection);
                int rows = storeUnit.ExecuteNonQuery();

                return unit.ID.ToString();
            }

            return string.Empty;
        }

        public override void StoreResolvedNavUnit(ResolvedNavUnit unit, Config cfg)
        {
            SQLiteCommand removeNavUnitRec = new SQLiteCommand("delete from navunit where address = '" + unit.Address + "'", Connection);
            removeNavUnitRec.ExecuteNonQuery();

            if (string.IsNullOrEmpty(unit.ScriptRef)) unit.ScriptRef = "none";

            SQLiteCommand storeUnit = new SQLiteCommand("insert into navunitresolved (id, address, addresshash, timefound, navtype, scriptref, contenthash, timescrapped, resolvedaddress, image) " + "values ('" + unit.ID.ToString() +
                "', '" + unit.Address.ToString() + "', '" + unit.AddressHash + "', '" + unit.TimeFound.ToString() + "', '" + unit.Type.ToString() + "', '" + unit.ScriptRef +
                "', '" + unit.ContentHash + "', '" + unit.TimeScrapped.ToString() + "', '" + unit.ResolvedAddress.ToString() + "', @image)", Connection);
            storeUnit.Parameters.Add("@image", System.Data.DbType.Binary, 20).Value = unit.Image;
            int rows = storeUnit.ExecuteNonQuery();

            foreach (string currentURL in unit.URLSFound)
            {
                if (string.IsNullOrEmpty(currentURL) || currentURL.Contains("javascript"))
                {
                    continue;
                }

                if (cfg.SingleDomain)
                {
                    if (!currentURL.Contains(cfg.RootDoamin))
                    {
                        continue;
                    }
                }

                string newID = StoreNavUnit(new NavUnit(currentURL));

                if (!string.IsNullOrEmpty(newID))
                {
                    SQLiteCommand storeLink = new SQLiteCommand("insert into navunitlinks (navunitid, linkedunitid) values ('" + unit.ID.ToString() + "', '" + newID + "')", Connection);
                    storeLink.ExecuteNonQuery();
                }
            }

        }

        private bool CheckForMatchUnit(string url)
        {
            SQLiteCommand checkForMatch = new SQLiteCommand("select * from navunit where address = \"" + url + "\"", Connection);
            bool rowsReturned = checkForMatch.ExecuteReader(System.Data.CommandBehavior.Default).HasRows; //ExecuteNonQuery();

            SQLiteCommand checkForMatchResolved = new SQLiteCommand("select * from navunitresolved where address = \"" + url + "\"", Connection);
            bool rowsReturnedResolved = checkForMatchResolved.ExecuteReader(System.Data.CommandBehavior.Default).HasRows;

            if (rowsReturned || rowsReturnedResolved)
            {
                return true;
            }

            return false;
        }

        public override List<NavUnit> RetriveUnitSet(int max)
        {
            List<NavUnit> tempNav = new List<NavUnit>();

            SQLiteCommand nextUnitSet = new SQLiteCommand("select * from navunit limit " + max, Connection);
            SQLiteDataReader reader = nextUnitSet.ExecuteReader(System.Data.CommandBehavior.Default);

            while (reader.Read())
            {
                NavUnit tempN = new NavUnit
                {
                    ID = Guid.Parse(reader.GetString(1)),
                    Address = new Uri(reader.GetString(2)),
                    AddressHash = reader.GetString(3),
                    TimeFound = DateTime.Parse(reader.GetString(4)),
                    ScriptRef = reader.GetString(6)
                };

                if (reader.GetString(5) == "URL")
                {
                    tempN.Type = NavType.URL;
                }
                else
                {
                    tempN.Type = NavType.Script;
                }

                tempNav.Add(tempN);
            }

            return tempNav;
        }

        public override List<NavUnit> RetriveUnits()
        {
            List<NavUnit> tempNav = new List<NavUnit>();

            SQLiteCommand nextUnitSet = new SQLiteCommand("select * from navunit", Connection);
            SQLiteDataReader reader = nextUnitSet.ExecuteReader(System.Data.CommandBehavior.Default);

            while (reader.Read())
            {
                NavUnit tempN = new NavUnit
                {
                    ID = Guid.Parse(reader.GetString(1)),
                    Address = new Uri(reader.GetString(2)),
                    AddressHash = reader.GetString(3),
                    TimeFound = DateTime.Parse(reader.GetString(4)),
                    ScriptRef = reader.GetString(6)
                };

                if (reader.GetString(5) == "URL")
                {
                    tempN.Type = NavType.URL;
                }
                else
                {
                    tempN.Type = NavType.Script;
                }

                tempNav.Add(tempN);
            }

            return tempNav;
        }

        public override int NavUnitCount()
        {
            SQLiteCommand checkForMatch = new SQLiteCommand("select count(address) from navunit", Connection);
            //checkForMatch.CommandType = System.Data.CommandType.Text;
            SQLiteDataReader readNav = checkForMatch.ExecuteReader(System.Data.CommandBehavior.Default);
            readNav.Read();
            int tempLinkCount = readNav.GetInt32(0);

            SQLiteCommand checkForMatchResolved = new SQLiteCommand("select count(address) from navunitresolved", Connection);
            SQLiteDataReader readRes = checkForMatchResolved.ExecuteReader(System.Data.CommandBehavior.Default);
            readRes.Read();
            int tempResLinkCount = readRes.GetInt32(0);

            tempLinkCount = tempLinkCount + tempResLinkCount;

            return tempLinkCount;
        }

        public override int ResolvedNavUnitCount()
        {
            SQLiteCommand checkForMatchResolved = new SQLiteCommand("select count(address) from navunitresolved", Connection);
            SQLiteDataReader readRes = checkForMatchResolved.ExecuteReader(System.Data.CommandBehavior.Default);
            readRes.Read();
            int tempResLinkCount = readRes.GetInt32(0);

            return tempResLinkCount;
        }

        public override Dictionary<string, byte[]> GetRawImages()
        {
            List<byte[]> tempRawImages = new List<byte[]>();
            List<string> tempAddress = new List<string>();

            Dictionary<string, byte[]> finalList = new Dictionary<string, byte[]>();

            SQLiteCommand retriveImages = new SQLiteCommand("select image from navunitresolved", Connection);
            SQLiteDataReader imageReader = retriveImages.ExecuteReader(System.Data.CommandBehavior.Default);

            while (imageReader.Read())
            {
                tempRawImages.Add(GetBytes(imageReader));
            }

            SQLiteCommand retriveAddress = new SQLiteCommand("select address from navunitresolved", Connection);
            SQLiteDataReader addressReader = retriveAddress.ExecuteReader(System.Data.CommandBehavior.Default);

            while (addressReader.Read())
            {
                tempAddress.Add(addressReader.GetString(0));
            }

            for (int index = 0; index < tempAddress.Count; index++)
            {
                finalList.Add(tempAddress[index], tempRawImages[index]);
            }

            return finalList;
        }

        public override void CloseDB()
        {
            Connection.Close();
            Connection.Dispose();
        }
    }
}
