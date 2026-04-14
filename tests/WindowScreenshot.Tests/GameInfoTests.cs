using Xunit;
using WindowScreenshot;

public class GameInfoTests
{
    [Fact]
    public void DefaultValues_ShouldBeZero()
    {
        // Arrange & Act
        var gameInfo = new GameInfo();

        // Assert
        Assert.Equal(0, gameInfo.CurrentHp);
        Assert.Equal(0, gameInfo.MaxHp);
        Assert.Equal(0, gameInfo.CurrentMp);
        Assert.Equal(0, gameInfo.MaxMp);
        Assert.Equal(0, gameInfo.Level);
    }

    [Fact]
    public void Constructor_ShouldSetProperties()
    {
        // Arrange & Act
        var gameInfo = new GameInfo
        {
            CurrentHp = 536,
            MaxHp = 536,
            CurrentMp = 545,
            MaxMp = 545,
            Level = 44
        };

        // Assert
        Assert.Equal(536, gameInfo.CurrentHp);
        Assert.Equal(536, gameInfo.MaxHp);
        Assert.Equal(545, gameInfo.CurrentMp);
        Assert.Equal(545, gameInfo.MaxMp);
        Assert.Equal(44, gameInfo.Level);
    }

    [Fact]
    public void IsEmpty_ShouldReturnTrue_WhenAllValuesAreZero()
    {
        // Arrange
        var gameInfo = new GameInfo();

        // Act
        var isEmpty = gameInfo.IsEmpty;

        // Assert
        Assert.True(isEmpty);
    }

    [Fact]
    public void IsEmpty_ShouldReturnFalse_WhenAnyValueIsNonZero()
    {
        // Arrange
        var gameInfo = new GameInfo
        {
            CurrentHp = 536
        };

        // Act
        var isEmpty = gameInfo.IsEmpty;

        // Assert
        Assert.False(isEmpty);
    }

    [Fact]
    public void ToString_ShouldReturnFormattedString_WhenHasData()
    {
        // Arrange
        var gameInfo = new GameInfo
        {
            CurrentHp = 536,
            MaxHp = 536,
            CurrentMp = 545,
            MaxMp = 545,
            Level = 44
        };

        // Act
        var result = gameInfo.ToString();

        // Assert
        Assert.Contains("536", result);
        Assert.Contains("44", result);
    }

    [Fact]
    public void ToString_ShouldReturnUnrecognized_WhenEmpty()
    {
        // Arrange
        var gameInfo = new GameInfo();

        // Act
        var result = gameInfo.ToString();

        // Assert
        Assert.Equal("未识别", result);
    }
}
