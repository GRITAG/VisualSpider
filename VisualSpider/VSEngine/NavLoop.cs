using System;
using System.Collections.Generic;
using System.Threading;
using VSEngine.Data;
using VSEngine.Integration;

namespace VSEngine
{
    /// <summary>
    /// Coordinates threads and processes results
    /// </summary>
    public class NavLoop 
    {
        ConsoleColor LabelColor = ConsoleColor.Magenta;
        ConsoleColor FieldColor = ConsoleColor.White;
        ConsoleColor TitleColor = ConsoleColor.Green;
        ConsoleColor LinkColor = ConsoleColor.DarkGray;
        ConsoleColor DeviderColor = ConsoleColor.Cyan;

        bool WorkToDo = true;
        // check if there is work to do
        // queue up URls / Scripts to run
        // new up treads based on max thread count and work left
        // collect results from finished treads
        // store navigation results in db

        public void Loop(DBAccess db, Config cfg, EngineState state)
        {

            int linkcount = 0;

            while (WorkToDo)
            {
                List<NavUnit> queuedUnits = db.RetriveUnitSet(cfg.MaxThreads);
                List<Thread> threads = new List<Thread>();
                List<NavThread> navThreads = new List<NavThread>();

                if(queuedUnits.Count < 1)
                {
                    WorkToDo = false;
                    break;
                }

                foreach(NavUnit currentUnit in queuedUnits)
                {
                    NavThread tempNavThread = new NavThread(currentUnit, cfg);
                    if (state == EngineState.LinkCheck) tempNavThread.CollectLinks = false;
                    Thread tempThread = new Thread(tempNavThread.Navigate);
                    tempThread.Start();

                    threads.Add(tempThread);
                    navThreads.Add(tempNavThread);
                }
                

                bool threadIsAlive = true;
                while (threadIsAlive)
                {

                    Thread.Sleep(1000);

                    threadIsAlive = false;

                    foreach(Thread currentThread in threads)
                    {
                        if (currentThread.IsAlive) threadIsAlive = true;
                    }

                    if (state == EngineState.LinkCheck)
                    {
                        List<string> messageUrls = new List<string>();
                        foreach(NavUnit currentUnit in queuedUnits)
                        {
                            messageUrls.Add(currentUnit.Address.ToString());
                        }
                         Engine.WrtieStatus(messageUrls.ToArray(), linkcount, threads.Count, "Visual Link Check", "Link Check");
                    }
                    else
                    {
                        List<string> messageUrls = new List<string>();
                        foreach (NavUnit currentUnit in queuedUnits)
                        {
                            messageUrls.Add(currentUnit.Address.ToString());
                        }
                        Engine.WrtieStatus(messageUrls.ToArray(), linkcount, threads.Count, "Visual Crawl", "Crawl");
                    }
                }

                DateTime batchTiming = DateTime.Now;
                foreach (NavThread currentNavTh in navThreads)
                {
                    db.StoreResolvedNavUnit(currentNavTh.UnitToPassBack, cfg);
                    linkcount++;
                }
                TimeSpan totalTime = DateTime.Now - batchTiming;

                if (cfg.MaxLinkCount > 0)
                {
                    if (db.ResolvedNavUnitCount() > cfg.MaxLinkCount) WorkToDo = false;
                }
            }
        }

        
    }
}
