using System.IO;
using YamlDotNet.Serialization;

namespace VSEngine.Data
{
    public class Config
    {
        public string StartURL { get; set; }
        public int MaxThreads { get; set; }
        public bool SingleDomain { get; set; }
        public string RootDoamin { get; set; }
        public int MaxLinkCount { get; set; }

        public void GenerateConfig ()
        {
            StartURL = "http://www.google.com";
            MaxThreads = 4;
            SingleDomain = true;
            MaxLinkCount = 10;
        }

        public void LoadConfig()
        {
            LoadConfig("vs.cfg");
        }

        public void LoadConfig(string file)
        {
            Deserializer deser = new Deserializer();
            Config temp = deser.Deserialize<Config>(File.OpenText(file));

            StartURL = temp.StartURL;
            MaxThreads = temp.MaxThreads;
            SingleDomain = temp.SingleDomain;
            RootDoamin = temp.RootDoamin;
            MaxLinkCount = temp.MaxLinkCount;
        }

        public void SaveConfig()
        {
            SaveConfig("vs.cfg");
        }

        public void SaveConfig(string file, bool overwrite = true)
        {
            if(File.Exists(file))
            {
                if(overwrite)
                {
                    File.Delete(file);
                }
                else
                {
                    return;
                }
            }

            TextWriter textStream = File.CreateText(file);
            Serializer ser = new Serializer();
            ser.Serialize(textStream, this);
            textStream.Flush();
        }
    }
}
