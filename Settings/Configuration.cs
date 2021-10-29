using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D2RAssist.Settings
{
    public interface IConfiguration
    {
        JObject Config { get; }
    }


    public class Configuration : IConfiguration
    {
        public Configuration()
        {
            Config = JObject.Parse(File.ReadAllText("config.json"));
        }

        public JObject Config { get; }
    }
}
