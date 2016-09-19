using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VSEngine.Data;
using VSEngine.Integration;

namespace VSEngine
{
    /// <summary>
    /// The initil setup and leg work to run the VSEngine
    /// </summary>
    public class Init
    {        
        public void LoadConfigs(Config cfg) { }

        public void FirstTimeURLStore(Config cfg, DBAccess db)
        {
            if (cfg.SingleDomain)
            {
                cfg.RootDoamin = cfg.StartURL.Split(new char[] { '/' })[2];
                if (cfg.RootDoamin.Contains("www.")) cfg.RootDoamin = cfg.RootDoamin.Replace("www.", "");
            }

            // this is only done before had for the first url
            NavUnit firstNav = new NavUnit(cfg.StartURL);
            db.StoreNavUnit(firstNav);

            NavThread thread = new NavThread(firstNav, cfg);
            Thread navThread = new Thread(thread.Navigate);
            navThread.Start();
            while(navThread.IsAlive)
            {
                Thread.Sleep(1000);
            }

            db.StoreResolvedNavUnit(thread.UnitToPassBack, cfg);

            return;
        }

        public void LoadLinks(string path, DBAccess db)
        {
            if (path.Contains(".db"))
            {
                DBAccess inputDB = new SQLiteDBAccess();
                inputDB.ConnectDB(path);

                foreach(NavUnit currentUnit in inputDB.RetriveUnits())
                {
                    db.StoreNavUnit(currentUnit);
                }
            }

            if (path.Contains(".txt"))
            {
                foreach(string link in File.ReadLines(path))
                {
                    db.StoreNavUnit(new NavUnit(link));
                }
            }

        }
    }
}
