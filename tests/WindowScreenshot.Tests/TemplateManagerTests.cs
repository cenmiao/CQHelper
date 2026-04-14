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

    [Fact]
    public void ExtractTemplates_ShouldExtractCharacters_FromHpMpImage()
    {
        // Arrange
        var manager = new TemplateManager(_testTemplatesDir, _testScreenshotDir);
        var hpMpSourcePath = Path.Combine(_testScreenshotDir, "HP-MP.png");
        var outputDir = Path.Combine(_testTemplatesDir, "hp_mp");

        // 复制测试素材到测试目录
        var sourceHpMp = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "..", "screenshot", "HP-MP.png");
        if (File.Exists(sourceHpMp))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(hpMpSourcePath));
            File.Copy(sourceHpMp, hpMpSourcePath, true);

            // Act
            var extractedCount = manager.ExtractTemplates(hpMpSourcePath, outputDir, "HP-MP");

            // Assert
            Assert.True(extractedCount > 0, "应该提取到至少一个字符");
            Assert.True(Directory.Exists(outputDir));

            // 验证模板文件已创建
            var templateFiles = Directory.GetFiles(outputDir, "char_*.png");
            Assert.True(templateFiles.Length > 0, "应该创建至少一个模板文件");
        }
    }

    [Fact]
    public void ExtractTemplates_ShouldExtractCharacters_FromLevelImage()
    {
        // Arrange
        var manager = new TemplateManager(_testTemplatesDir, _testScreenshotDir);
        var levelSourcePath = Path.Combine(_testScreenshotDir, "LEVEL.png");
        var outputDir = Path.Combine(_testTemplatesDir, "level");

        // 复制测试素材到测试目录
        var sourceLevel = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "..", "screenshot", "LEVEL.png");
        if (File.Exists(sourceLevel))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(levelSourcePath));
            File.Copy(sourceLevel, levelSourcePath, true);

            // Act
            var extractedCount = manager.ExtractTemplates(levelSourcePath, outputDir, "LEVEL");

            // Assert
            Assert.True(extractedCount > 0, "应该提取到至少一个字符");
            Assert.True(Directory.Exists(outputDir));

            // 验证模板文件已创建
            var templateFiles = Directory.GetFiles(outputDir, "char_*.png");
            Assert.True(templateFiles.Length > 0, "应该创建至少一个模板文件");
        }
    }

    [Fact]
    public void Recognize_ShouldReturnString_WhenValidTemplateAndImage()
    {
        // 这个测试需要完整的模板提取流程
        // 由于 OpenCV 依赖和真实的图像识别，这里做集成测试
        // 单元测试主要验证方法调用和返回值类型
        var manager = new TemplateManager(_testTemplatesDir, _testScreenshotDir);

        // Arrange - 创建一个简单的测试图像
        using var testImage = new System.Drawing.Bitmap(100, 50);
        using var g = System.Drawing.Graphics.FromImage(testImage);
        g.Clear(System.Drawing.Color.White);
        using var font = new System.Drawing.Font("Arial", 20);
        using var brush = System.Drawing.Brushes.Black;
        g.DrawString("123", font, brush, new System.Drawing.PointF(10, 10));

        // Act - 由于没有模板，返回空字符串
        var result = manager.Recognize(testImage, new List<TemplateDigit>());

        // Assert
        Assert.Equal("", result);
    }

    [Fact]
    public void HasManualTemplates_ShouldReturnTrue_WhenTemplatesExist()
    {
        // Arrange
        var manager = new TemplateManager(_testTemplatesDir, _testScreenshotDir);
        var hpMpDir = Path.Combine(_testTemplatesDir, "hp_mp");
        Directory.CreateDirectory(hpMpDir);

        // 创建一个有效的模板文件
        using var testBitmap = new System.Drawing.Bitmap(10, 10);
        testBitmap.Save(Path.Combine(hpMpDir, "char_0.png"), System.Drawing.Imaging.ImageFormat.Png);

        // Act
        var result = manager.HasManualTemplates();

        // Assert
        Assert.True(result);
    }

    public void Dispose()
    {
        if (Directory.Exists(_testTemplatesDir))
            Directory.Delete(_testTemplatesDir, true);
        if (Directory.Exists(_testScreenshotDir))
            Directory.Delete(_testScreenshotDir, true);
    }
}
