using System;
using System.Drawing;

namespace D2RConfigEditor.BL.BO
{
    public interface IOption
    {
        string Comment { get; }
        string Name { get; }
        OptionType OptionType { get; }
        string Value { get; set; }
    }

    public class BoolOption : IOption
    {
        private bool value;

        public BoolOption(string name, bool value, OptionType optionType, string comment)
        {
            Name = name;
            this.value = value;
            OptionType = optionType;
            Comment = comment;
        }

        public string Comment { get; }
        public string Name { get; }
        public OptionType OptionType { get; }

        public string Value
        {
            get
            {
                return value.ToString();
            }
            set
            {
                bool.TryParse(value, out this.value);
            }
        }
    }

    public class ColorOption : IOption
    {
        private readonly IColorParser colorParser;
        private Color value;

        public ColorOption(string name, Color value, OptionType optionType, string comment, IColorParser colorParser)
        {
            Name = name;
            this.value = value;
            OptionType = optionType;
            Comment = comment;
            this.colorParser = colorParser;
        }

        public string Comment { get; }
        public string Name { get; }
        public OptionType OptionType { get; }

        public string Value
        {
            get
            {
                return value.ToString();
            }
            set
            {
                try
                {
                    this.value = colorParser.ParseColorFromRGBString(value);
                }
                catch (FormatException e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }
    }

    public class FloatOption : IOption
    {
        private float value;

        public FloatOption(string name, float value, OptionType optionType, string comment)
        {
            Name = name;
            this.value = value;
            OptionType = optionType;
            Comment = comment;
        }

        public string Comment { get; }
        public string Name { get; }
        public OptionType OptionType { get; }

        public string Value
        {
            get
            {
                return value.ToString();
            }
            set
            {
                float.TryParse(value, out this.value);
            }
        }
    }

    public class IntOption : IOption
    {
        private int value;

        public IntOption(string name, int value, OptionType optionType, string comment)
        {
            Name = name;
            this.value = value;
            OptionType = optionType;
            Comment = comment;
        }

        public string Comment { get; }
        public string Name { get; }
        public OptionType OptionType { get; }

        public string Value
        {
            get
            {
                return value.ToString();
            }
            set
            {
                int.TryParse(value, out this.value);
            }
        }
    }

    public class TextOption : IOption
    {
        public TextOption(string name, string value, OptionType optionType, string comment)
        {
            Comment = comment;
            Name = name;
            OptionType = optionType;
            Value = value;
        }

        public string Comment { get; }
        public string Name { get; }
        public OptionType OptionType { get; }
        public string Value { get; set; }
    }
}
