using Xunit;
using WindowScreenshot;

public class GameLogTests
{
    [Fact]
    public void Constructor_ShouldInitializeEmptyList()
    {
        // Arrange & Act
        var log = new GameLog();

        // Assert
        Assert.Empty(log.Entries);
        Assert.Equal(0, log.Count);
    }

    [Fact]
    public void Append_ShouldAddEntry()
    {
        // Arrange
        var log = new GameLog();

        // Act
        log.Append("Test message");

        // Assert
        Assert.Single(log.Entries);
        Assert.Equal("Test message", log.Entries[0].Message);
    }

    [Fact]
    public void Append_ShouldLimitToMaxEntries()
    {
        // Arrange
        var log = new GameLog();

        // Act - 添加 150 条日志
        for (int i = 0; i < 150; i++)
        {
            log.Append($"Message {i}");
        }

        // Assert - 最多保留 100 条
        Assert.Equal(100, log.Entries.Count);
        Assert.Equal("Message 50", log.Entries[0].Message);
        Assert.Equal("Message 149", log.Entries[99].Message);
    }

    [Fact]
    public void Clear_ShouldRemoveAllEntries()
    {
        // Arrange
        var log = new GameLog();
        log.Append("Message 1");
        log.Append("Message 2");

        // Act
        log.Clear();

        // Assert
        Assert.Empty(log.Entries);
        Assert.Equal(0, log.Count);
    }

    [Fact]
    public void Append_ShouldSetTimestamp()
    {
        // Arrange
        var log = new GameLog();
        var before = DateTime.Now;

        // Act
        log.Append("Test message");

        // Assert
        var after = DateTime.Now;
        Assert.True(log.Entries[0].Timestamp >= before && log.Entries[0].Timestamp <= after);
    }

    [Fact]
    public void Append_ShouldSetDefaultLevelToInfo()
    {
        // Arrange
        var log = new GameLog();

        // Act
        log.Append("Test message");

        // Assert
        Assert.Equal("Info", log.Entries[0].Level);
    }

    [Fact]
    public void Append_ShouldSetCustomLevel()
    {
        // Arrange
        var log = new GameLog();

        // Act
        log.Append("Test message", "Warning");

        // Assert
        Assert.Equal("Warning", log.Entries[0].Level);
    }

    [Fact]
    public void GetLast_ShouldReturnLastNEntries()
    {
        // Arrange
        var log = new GameLog();
        for (int i = 0; i < 20; i++)
        {
            log.Append($"Message {i}");
        }

        // Act
        var last5 = log.GetLast(5);

        // Assert
        Assert.Equal(5, last5.Count);
        Assert.Equal("Message 15", last5[0].Message);
        Assert.Equal("Message 19", last5[4].Message);
    }

    [Fact]
    public void GetLast_ShouldReturnAll_WhenCountExceedsEntries()
    {
        // Arrange
        var log = new GameLog();
        log.Append("Message 1");
        log.Append("Message 2");

        // Act
        var result = log.GetLast(10);

        // Assert
        Assert.Equal(2, result.Count);
    }
}
