﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Thought.Terminals;
using VSEngine.Data;
using VSEngine.Integration;

namespace VSEngine
{
    /// <summary>
    /// Cordiates threads and processes resutls
    /// </summary>
    public class NavLoop
    {
        bool WorkToDo = true;
        // check if there is work to do
        // queue up URls / Scripts to run
        // new up treads based on max thread count and work left
        // collect results from finishhed treads
        // sotre navigation results in db
        DBAccess DB;
        Config CFG;
        Terminal Console;

        public void Loop(DBAccess db, Config cfg, Terminal console)
        {
            DB = db;
            CFG = cfg;
            Console = console;
            Console.ClearScreen();
            WriteScreen();

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
                    NavThread tempNavThread = new NavThread(currentUnit);
                    tempNavThread.configRef = cfg;
                    Thread tempThread = new Thread(tempNavThread.Navigate);
                    tempThread.Start();

                    threads.Add(tempThread);
                    navThreads.Add(tempNavThread);
                }
                

                bool threadIsAlive = true;

                while(threadIsAlive)
                {
                    threadIsAlive = false;

                    foreach(Thread currentThread in threads)
                    {
                        if (currentThread.IsAlive) threadIsAlive = true;
                    }

                }

                foreach(NavThread currentNavTh in navThreads)
                {
                    db.StoreResolvedNavUnit(currentNavTh.UnitToPassBack, cfg);
                }

                if (cfg.MaxLinkCount > 0)
                {
                    if (db.ResolvedNavUnitCount() > cfg.MaxLinkCount) WorkToDo = false;
                }

                //WriteScreen();

            }
        }

        private void WriteScreen()
        {
            Console.WriteLine("\t\t\t\t\t\t\tVisual Spider");

        }

        private void UpdateScreen()
        {

        }
    }
}
