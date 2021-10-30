using Newtonsoft.Json.Linq;
using System.IO;

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
            //TODO: Config is now array of objects, instead of list of values
            Config = JObject.Parse(File.ReadAllText("config.json"));
        }

        public JObject Config { get; }
    }
}
