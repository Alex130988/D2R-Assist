using D2RConfigEditor.BL;
using D2RConfigEditor.BL.BO;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using System;
using System.Drawing;
using System.IO;

namespace D2RConfigEditor.Tests
{
    [TestFixture]
    public class ConfigReaderWriterTest
    {
        private const string readConfigFilepath = @"../../../testconfig.json";
        private const string writeConfigFilepath = @"../../../testconfig2.json";
        private Color color;
        private Mock<IColorParser> colorParserMock;
        private MockRepository mockRepository;
        private Random random;

        [Test]
        public void ReadConfigTest()
        {
            // Arrange
            IConfigReaderWriter sut = new ConfigReaderWriter(readConfigFilepath, colorParserMock.Object);
            colorParserMock.Setup(c => c.ParseColorFromRGBString("50,50,50")).Returns(color);

            // Act
            IOption[] options = sut.ReadConfig();

            // Assert
            options.Should().NotBeEmpty();
            options[4].Value.Should().Be(color.ToString());
        }

        [SetUp]
        public void Setup()
        {
            Environment.CurrentDirectory = Path.GetDirectoryName(typeof(ConfigReaderWriterTest).Assembly.Location);

            mockRepository = new MockRepository(MockBehavior.Strict);
            colorParserMock = mockRepository.Create<IColorParser>();
            random = new Random();

            color = Color.FromArgb(255);
        }

        [Test]
        public void WriteConfigTest()
        {
            string originContent = File.ReadAllText(writeConfigFilepath);
            IOption option = new TextOption("TextOption", random.Next().ToString(), OptionType.Text, string.Empty);
            IOption[] options = { option };
            IConfigReaderWriter sut = new ConfigReaderWriter(writeConfigFilepath, null);

            sut.WriteConfig(options);

            File.ReadAllText(writeConfigFilepath).Should().NotBeSameAs(originContent);
        }
    }
}
