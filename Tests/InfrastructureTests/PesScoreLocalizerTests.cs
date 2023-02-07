using System.Globalization;
using Infrastructure.Services;
using Xunit;

namespace InfrastructureTests;

public class PesScoreLocalizerTests
{
    private readonly PesScoreLocalizer _sut;

    public PesScoreLocalizerTests()
    {
        _sut = new PesScoreLocalizer();
    }

    [Fact]
    public void GetLocalizedNameFor_ShouldReturnNull_WhenNullIsProvided()
    {
        //Arrange
        string input = null;
        //Act
        string actual = _sut.GetLocalizedNameFor(input, CultureInfo.InvariantCulture);
        //Assert
        Assert.Null(actual);
    }

    [Fact]
    public void GetLocalizedNameFor_ShouldReturnNull_WhenEmptyStringIsProvided()
    {
        //Arrange
        string input = "";
        //Act
        string actual = _sut.GetLocalizedNameFor(input, CultureInfo.InvariantCulture);
        //Assert
        Assert.Null(actual);
    }

    [Fact]
    public void GetLocalizedNameFor_ShouldReturnEnglishValue_WhenUnsupportedCultureIsProvided()
    {
        //Arrange
        string input = "Null";
        CultureInfo unsupported = CultureInfo.GetCultureInfo("kk-KZ");
        //Act
        string actual = _sut.GetLocalizedNameFor(input, unsupported);
        //Assert
        string expected = "Semi-Doge";
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void GetLocalizedNameFor_ShouldReturnCultureSpecificValue_WhenSupportedCultureIsProvided()
    {
        //Arrange
        string input = "Null";
        CultureInfo unsupported = CultureInfo.GetCultureInfo("ru-RU");
        //Act
        string actual = _sut.GetLocalizedNameFor(input, unsupported);
        //Assert
        string expected = "Полу-Пёс";
        Assert.Equal(expected, actual);
    }
}