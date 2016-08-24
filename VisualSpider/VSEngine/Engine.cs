using System;
using System.IO;
using VSEngine.Data;
using VSEngine.Integration;

namespace VSEngine
{
    public enum EngineState
    {
        Main, GenerateConfig, ExportImages, ExportResults, LinkCheck
    }

    /// <summary>
    /// The engine that runs the Visual Spider main loop
    /// </summary>
    public class Engine
    {
        public static event EventHandler<UIData> UIEvent;

        Config Configs = new Config();

        Init Initilization = new Init();
        NavLoop NavigationLoop = new NavLoop();
        PostReporting CleanupAndReporting = new PostReporting();

        string SourcePath { get; set; }
        EngineState State { get; set; }

        DBAccess Database = new DBAccess();

        public Engine(EngineState engineState, string path = "")
        {
            State = engineState;
            SourcePath = path;

            switch(engineState)
            {
                case EngineState.Main:
                    MainLoop();
                    break;
                case EngineState.GenerateConfig:
                    Configs.GenerateConfig();
                    break;
                default:
                    MainLoop();
                    break;
            }
        }

        public void MainLoop()
        {
            WriteLog("Handling Configs");

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
            WriteLog("Building Database");
            Initilization.CreateDB(Database);

            if (State == EngineState.LinkCheck)
            {
                WriteLog("Load Up Links");
                Initilization.LoadLinks(SourcePath, Database);
            }
            else
            {
                WriteLog("Checking first link");
                Initilization.FirstTimeURLStore(Configs, Database);
            }

            WriteLog("Entering Nav Loop");
            NavigationLoop.Loop(Database, Configs, State);
            WriteLog("Compiling Reporting and Cleaning up");
            CleanupAndReporting.WriteIamges(Database.GetRawImages());           
        }

        public Engine SoucreDBPath(string path)
        {
            SourcePath = path;
            return this;
        }

        public static void WriteLog(string text)
        {
            UIData uiData = new UIData();
            uiData.Messages.Add(text);
            uiData.Display = DisplayType.Log;
            UIEvent(null, uiData);
        }

        public static void WrtieStatus(string[] links, int linkCount, int threadCount, string title, string mode)
        {
            UIData uiData = new UIData();
            uiData.Messages.AddRange(links);
            uiData.Display = DisplayType.Stats;
            uiData.LinkCount = linkCount;
            uiData.ThreadCount = threadCount;
            uiData.Title = title;
            uiData.Mode = mode;
            UIEvent(null, uiData);
        }
    }
}
