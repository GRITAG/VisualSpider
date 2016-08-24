using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VSEngine;

namespace VisualSpider
{
    class Program
    {
        public static Engine GOGO;

        static void Main(string[] args)
        {
            Console.Title = "Visual Spider";

            //args = new string[2] { "/l", "links.txt" };

            Engine.UIEvent += HandleUI;

            if (args.Length < 1)
            {
                GOGO = new Engine(EngineState.Main);
            }
            else
            {
                foreach(string currentArg in args)
                {
                    if(currentArg.ToLower().Contains("/g"))
                    {
                        GOGO = new Engine(EngineState.GenerateConfig);
                    }

                    if(currentArg.ToLower().Contains("/l"))
                    {
                        GOGO = new Engine(EngineState.LinkCheck, args[1]);
                    }

                    if(currentArg.ToLower().Contains("/x"))
                    {
                        // exit
                    }
                }
            }

            Environment.Exit(0);
            //Console.ReadKey();
        }

        private static void HandleUI(object sender, UIData e)
        {
            if(e.Display == DisplayType.Log)
            {
                foreach(string currentString in e.Messages)
                {
                    Console.WriteLine(currentString);
                }
            }
            else
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Visual Spider");
                Console.ForegroundColor = ConsoleColor.Cyan;
                for (int i = 0; i < System.Console.WindowWidth; i++)
                {
                    Console.Write('=');
                }
                Console.Write("\n");
                Console.ForegroundColor =ConsoleColor.Magenta;
                Console.Write("\tMode: ");
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write(e.Mode + "\n");

                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.Write("\tThread Count: ");
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write(e.ThreadCount);
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.Write("\t\tLink Count: ");
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write(e.LinkCount + "\n\n");

                Console.ForegroundColor = ConsoleColor.DarkGray;
                foreach (string currentUnit in e.Messages)
                {
                    Console.WriteLine("\t" + currentUnit);
                }
            }
        }
    
    }
}
