using System;
using System.Collections.Generic;
using System.Threading;
using Thought.Terminals;
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

        public void Loop(DBAccess db, Config cfg, Terminal console, EngineState state)
        {
            console.ClearScreen();
            WriteScreen(console);

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
                        UpdateScreen(queuedUnits, threads, linkcount, "Link Check", console);
                    }
                    else
                    {
                        UpdateScreen(queuedUnits, threads, linkcount, "Crawl", console);
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

            WriteScreen(console);
        }

        private void WriteScreen(Terminal console)
        {
            console.ClearScreen();
            console.SetForeground(TitleColor);
            console.WritePadded("Visual Spider", System.Console.WindowWidth, JustifyText.Center);
            console.SetForeground(DeviderColor);
            for(int i=0; i < System.Console.WindowWidth; i++)
            {
                console.Write('=');
            }
            console.Write("\n");
        }

        private void UpdateScreen(List<NavUnit> units, List<Thread> threds, int linkCount, string mode, Terminal console)
        {
            WriteScreen(console);
            console.SetForeground(LabelColor);
            console.Write("\tMode: ");
            console.SetForeground(FieldColor);
            console.Write(mode + "\n");

            console.SetForeground(LabelColor);
            console.Write("\tThread Count: ");
            console.SetForeground(FieldColor);
            console.Write(threds.Count);
            console.SetForeground(LabelColor);
            console.Write("\t\tLink Count: ");
            console.SetForeground(FieldColor);
            console.Write(linkCount + "\n\n");

            console.SetForeground(LinkColor);
            foreach (NavUnit currentUnit in units)
            {
                console.WriteLine("\t" + currentUnit.Address.ToString());
            }
        }
    }
}
