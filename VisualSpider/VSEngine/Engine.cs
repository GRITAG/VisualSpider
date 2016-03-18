using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VSEngine.Integration;

namespace VSEngine
{
    /// <summary>
    /// The engine that runs the Visual Spider main loop
    /// </summary>
    public class Engine
    {
        Init Initilization = new Init();
        NavLoop NavigationLoop = new NavLoop();
        PostReporting CleanupAndReporting = new PostReporting();

        DBAccess Database = new DBAccess();

        public Engine()
        {
            Initilization.LoadConfigs();
            Initilization.CreateSQLiteDB();
            Initilization.FirstTimeURLStore();
        }
    }
}
