using D2RConfigEditor.BL.BO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace D2RConfigEditor.BL
{
    public interface IConfigReaderWriter
    {
        IOption[] ReadConfig();

        void WriteConfig(IOption[] options);
    }

    public class ConfigReaderWriter : IConfigReaderWriter
    {
        private readonly IColorParser colorParser;
        private readonly string filepath;

        public ConfigReaderWriter(string filepath, IColorParser colorParser)
        {
            if (!File.Exists(filepath))
            {
                throw new System.ArgumentException($"Es existiert keine Datei \"{filepath}\".", nameof(filepath));
            }

            this.filepath = filepath;
            this.colorParser = colorParser;
        }

        public IOption[] ReadConfig()
        {
            List<IOption> options = new List<IOption>();
            JArray array = JArray.Parse(File.ReadAllText(filepath));
            foreach (JToken o in array)
            {
                OptionType optionType;
                bool canParse = true;
                switch (o["Type"].ToString())
                {
                    case "Text":
                        optionType = OptionType.Text;
                        options.Add(new TextOption(o["Name"].ToString(), o["Value"].ToString(), optionType, o["Comment"].ToString()));
                        break;

                    case "Float":
                        optionType = OptionType.Float;
                        canParse = float.TryParse(o["Value"].ToString(), out float floatvalue);
                        options.Add(new FloatOption(o["Name"].ToString(), floatvalue, optionType, o["Comment"].ToString()));
                        break;

                    case "Int":
                        optionType = OptionType.Int;
                        canParse = int.TryParse(o["Value"].ToString(), out int intvalue);
                        options.Add(new IntOption(o["Name"].ToString(), intvalue, optionType, o["Comment"].ToString()));
                        break;

                    case "Bool":
                        optionType = OptionType.Bool;
                        canParse = bool.TryParse(o["Value"].ToString(), out bool boolvalue);
                        options.Add(new BoolOption(o["Name"].ToString(), boolvalue, optionType, o["Comment"].ToString()));
                        break;

                    case "Color":
                        optionType = OptionType.Color;
                        Color colorvalue = colorParser.ParseColorFromRGBString(o["Value"].ToString());
                        canParse = true;
                        options.Add(new ColorOption(o["Name"].ToString(), colorvalue, optionType, o["Comment"].ToString(), colorParser));
                        break;

                    default:
                        throw new ArgumentException("Config contains unsupported Type");
                }
                if (!canParse)
                    throw new FormatException("Error in parsing config");
            }
            return options.ToArray();
        }

        public void WriteConfig(IOption[] options)
        {
            File.WriteAllText(filepath, JsonConvert.SerializeObject(options, Formatting.Indented));
        }
    }
}
