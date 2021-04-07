using Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace InfrastructureTests
{
    public class PesScoreConfigTests
    {
        private readonly Mock<IConfiguration> _configMock = new Mock<IConfiguration>();
        private static Dictionary<int, string> _values = new Dictionary<int, string>
        {
            { 10, "First"},
            { 50, "Second"},
            { 80, "Third"}
        };

        private void SetupConfigMock()
        {
            _configMock.Setup(x => x.GetSection(It.IsAny<string>())).Returns(new ConfigurationSectionMock());
        }

        [Fact]
        public void GetTitleForScore_ShouldReturnFirstOne_WhenValueIsTooLow()
        {
            //Arrange
            SetupConfigMock();
            var config = new PesScoreConfig(_configMock.Object);
            int score = -110000;
            //Act
            string actual = config.GetTitleForScore(score);
            //Assert
            string expected = _values.First().Value;
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void GetTitleForScore_ShouldReturnLastOne_WhenValueIsTooHigh()
        {
            //Arrange
            SetupConfigMock();
            var config = new PesScoreConfig(_configMock.Object);
            int score = 110000;
            //Act
            string actual = config.GetTitleForScore(score);
            //Assert
            string expected = _values.Last().Value;
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void GetTitleForScore_ShouldReturnValueBetweenBounds_WhenValueIsBetweenBounds()
        {
            //Arrange
            SetupConfigMock();
            var config = new PesScoreConfig(_configMock.Object);
            int score = 60;
            //Act
            string actual = config.GetTitleForScore(score);
            //Assert
            string expected = "Second";
            Assert.Equal(expected, actual);
        }

        private class ConfigurationSectionMock : IConfigurationSection
        {
            public IEnumerable<IConfigurationSection> GetChildren()
            {
                return _values.Select(x => new ConfigurationItemMock(x.Key.ToString(), x.Value));
            }
            #region NotImplemented
            public string this[string key] { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

            public string Path { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

            public string Key => throw new NotImplementedException();

            public string Value { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

            public IChangeToken GetReloadToken()
            {
                throw new NotImplementedException();
            }

            public IConfigurationSection GetSection(string key)
            {
                throw new NotImplementedException();
            }
            #endregion
        }
        private class ConfigurationItemMock : IConfigurationSection
        {
            public ConfigurationItemMock(string key, string value)
            {
                Key = key;
                Value = value;
            }
            public string Key { get; set; }
            public string Value { get; set; }

            #region NotImplemented
            public string Path => throw new NotImplementedException();
            public string this[string key] { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

            public IEnumerable<IConfigurationSection> GetChildren()
            {
                throw new NotImplementedException();
            }

            public IChangeToken GetReloadToken()
            {
                throw new NotImplementedException();
            }

            public IConfigurationSection GetSection(string key)
            {
                throw new NotImplementedException();
            }
            #endregion
        }
    }

  
}
