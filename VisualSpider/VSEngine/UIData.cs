using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VSEngine
{
    public class UIData : EventArgs
    {
        public string Title { get; set; }
        public string Mode { get; set; }
        public int ThreadCount { get; set; }
        public int LinkCount { get; set; }
        public DisplayType Display { get; set; }
        public List<string> Messages { get; set; }

        public UIData()
        {
            Messages = new List<string>();
        }
    }

    public enum DisplayType
    {
        Log, Stats
    }
}
