using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VSEngine.Data;
using VSEngine.Integration;

namespace VSEngine
{
    public enum EngineState
    {
        Main, GenerateConfig, ExportImages, ExportResults
    }

    /// <summary>
    /// The engine that runs the Visual Spider main loop
    /// </summary>
    public class Engine
    {
        Config Configs = new Config();

        Init Initilization = new Init();
        NavLoop NavigationLoop = new NavLoop();
        PostReporting CleanupAndReporting = new PostReporting();

        DBAccess Database = new DBAccess();

        public Engine(EngineState engineState)
        {
            switch(engineState)
            {
                case EngineState.Main:
                    MainLoop();
                    break;
                case EngineState.GenerateConfig:
                    Configs.GenerateConfig();
                    break;
            }
        }

        public void MainLoop()
        {
            if (!File.Exists("vs.cfg"))
            {
                Configs.GenerateConfig();
                Configs.SaveConfig();
            }
            else
            {
                Configs.LoadConfig();
            }

            Initilization.LoadConfigs(Configs);
            Initilization.CreateDB(Database);
            Initilization.FirstTimeURLStore(Configs, Database);
            NavigationLoop.Loop(Database, Configs);
            CleanupAndReporting.WriteIamges(Database.GetRawImages());

        }
    }
}
