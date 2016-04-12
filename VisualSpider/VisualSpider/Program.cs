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

                    if(currentArg.ToLower().Contains("/x"))
                    {
                        // exit
                    }
                }
            }

            //Console.ReadKey();
        }
    }
}
