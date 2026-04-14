using Xunit;
using System.IO;
using WindowScreenshot;

public class TemplateManagerTests : IDisposable
{
    private readonly string _testTemplatesDir;
    private readonly string _testScreenshotDir;

    public TemplateManagerTests()
    {
        _testTemplatesDir = Path.Combine(Path.GetTempPath(), $"templates_{Guid.NewGuid()}");
        _testScreenshotDir = Path.Combine(Path.GetTempPath(), $"screenshots_{Guid.NewGuid()}");
        Directory.CreateDirectory(_testTemplatesDir);
        Directory.CreateDirectory(_testScreenshotDir);
    }

    [Fact]
    public void Constructor_ShouldInitializeWithDefaultPaths()
    {
        // Arrange & Act
        var manager = new TemplateManager();

        // Assert
        Assert.NotNull(manager);
    }

    [Fact]
    public void Constructor_ShouldInitializeWithCustomPaths()
    {
        // Arrange & Act
        var manager = new TemplateManager(_testTemplatesDir, _testScreenshotDir);

        // Assert
        Assert.NotNull(manager);
    }

    [Fact]
    public void HasCachedTemplates_ShouldReturnFalse_WhenTemplatesDirNotExists()
    {
        // Arrange
        var newDir = Path.Combine(Path.GetTempPath(), $"new_templates_{Guid.NewGuid()}");
        var manager = new TemplateManager(newDir, _testScreenshotDir);

        // Act
        var result = manager.HasCachedTemplates();

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void HasCachedTemplates_ShouldReturnTrue_WhenTemplatesDirExists()
    {
        // Arrange
        var manager = new TemplateManager(_testTemplatesDir, _testScreenshotDir);
        Directory.CreateDirectory(Path.Combine(_testTemplatesDir, "hp_mp"));

        // Act
        var result = manager.HasCachedTemplates();

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsLoaded_ShouldReturnFalse_WhenNoTemplatesLoaded()
    {
        // Arrange
        var manager = new TemplateManager(_testTemplatesDir, _testScreenshotDir);

        // Act
        var result = manager.IsLoaded;

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void LoadTemplates_ShouldReturnFalse_WhenNoTemplatesDir()
    {
        // Arrange
        var newDir = Path.Combine(Path.GetTempPath(), $"new_templates_{Guid.NewGuid()}");
        var manager = new TemplateManager(newDir, _testScreenshotDir);

        // Act
        var result = manager.LoadTemplates();

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void LoadTemplates_ShouldReturnFalse_WhenTemplatesAreInvalid()
    {
        // Arrange
        var manager = new TemplateManager(_testTemplatesDir, _testScreenshotDir);
        var hpMpDir = Path.Combine(_testTemplatesDir, "hp_mp");
        Directory.CreateDirectory(hpMpDir);

        // 创建模拟模板文件（不是真实的 PNG 文件）
        File.WriteAllText(Path.Combine(hpMpDir, "char_0.png"), "mock");

        // Act
        var result = manager.LoadTemplates();

        // Assert - 由于文件不是有效的 PNG，加载会失败（IsLoaded 为 false）
        Assert.False(manager.IsLoaded);
    }

    public void Dispose()
    {
        if (Directory.Exists(_testTemplatesDir))
            Directory.Delete(_testTemplatesDir, true);
        if (Directory.Exists(_testScreenshotDir))
            Directory.Delete(_testScreenshotDir, true);
    }
}
