using System.Drawing;

namespace WindowScreenshot.Tests;

public class IntegrationTests : IDisposable
{
    private readonly string _testConfigPath;
    private readonly string _testOutputDir;

    public IntegrationTests()
    {
        _testConfigPath = Path.Combine(Path.GetTempPath(), "IntegrationTest_" + Guid.NewGuid(), "config.json");
        _testOutputDir = Path.Combine(Path.GetTempPath(), "IntegrationTest_" + Guid.NewGuid(), "screenshots");
    }

    [Fact]
    public void 完整流程_配置保存和加载 ()
    {
        // Arrange
        var configManager = new ConfigManager(_testConfigPath);
        var settings = new ScreenshotSettings
        {
            TargetWindowTitle = "Test Window",
            TargetWindowClassName = "Notepad",
            IntervalSeconds = 10,
            IsEnabled = true
        };

        try
        {
            // Act
            configManager.Save(settings);
            var loaded = configManager.Load();

            // Assert
            Assert.Equal(settings.TargetWindowTitle, loaded.TargetWindowTitle);
            Assert.Equal(settings.TargetWindowClassName, loaded.TargetWindowClassName);
            Assert.Equal(settings.IntervalSeconds, loaded.IntervalSeconds);
        }
        finally
        {
            Cleanup();
        }
    }

    [Fact]
    public void 完整流程_窗口查找 ()
    {
        // Arrange
        var finder = new WindowFinder();
        var enumerator = new WindowEnumerator();
        var windows = enumerator.EnumWindows();

        // Act
        var targetWindow = windows[0];
        var foundHandle = finder.FindWindow(targetWindow.Title, targetWindow.ClassName);

        // Assert
        Assert.NotEmpty(windows);
        Assert.Equal(targetWindow.Handle, foundHandle);
    }

    [Fact]
    public void 完整流程_定时截图服务启动和停止 ()
    {
        // Arrange
        var capturer = new WindowCapturer();
        var saver = new ScreenshotSaver(capturer);
        var finder = new WindowFinder();
        var service = new TimedScreenshotService(finder, capturer, saver, _testOutputDir);
        var enumerator = new WindowEnumerator();
        var windows = enumerator.EnumWindows();

        Assert.NotEmpty(windows);
        var targetWindow = windows[0];

        try
        {
            // Act
            service.Start(targetWindow.Handle, 5);

            // Assert
            Assert.True(service.IsRunning);

            // Stop
            service.Stop();
            Assert.False(service.IsRunning);
        }
        finally
        {
            service.Dispose();
            Cleanup();
        }
    }

    [Fact]
    public void 完整流程_窗口不存在时停止并触发事件 ()
    {
        // Arrange
        var capturer = new WindowCapturer();
        var saver = new ScreenshotSaver(capturer);
        var finder = new WindowFinder();
        var service = new TimedScreenshotService(finder, capturer, saver, _testOutputDir);
        var invalidHandle = (IntPtr)(-1);
        bool eventTriggered = false;

        service.WindowNotFound += (s, e) => eventTriggered = true;

        try
        {
            // Act - 使用无效窗口句柄启动
            service.Start(invalidHandle, 1);
            service.TriggerTickForTest();

            // Assert
            Assert.True(eventTriggered, "WindowNotFound 事件应该被触发");
            Assert.False(service.IsRunning, "定时器应该已停止");
        }
        finally
        {
            service.Dispose();
            Cleanup();
        }
    }

    [Fact]
    public void 完整流程_游戏分析服务初始化 ()
    {
        // Arrange
        var testTemplatesDir = Path.Combine(Path.GetTempPath(), "templates_" + Guid.NewGuid());
        var testScreenshotDir = Path.Combine(Path.GetTempPath(), "screenshots_" + Guid.NewGuid());
        Directory.CreateDirectory(testTemplatesDir);
        Directory.CreateDirectory(testScreenshotDir);

        var templateManager = new TemplateManager(testTemplatesDir, testScreenshotDir);
        var gameLog = new GameLog();

        try
        {
            // Act - 创建分析器和服务
            var healthBarAnalyzer = new HealthBarAnalyzer(templateManager);
            var levelAnalyzer = new LevelAnalyzer(templateManager);
            var gameAnalysisService = new GameAnalysisService(healthBarAnalyzer, levelAnalyzer, gameLog);

            // Assert
            Assert.NotNull(gameAnalysisService);
            Assert.True(gameAnalysisService.IsEnabled);
        }
        finally
        {
            if (Directory.Exists(testTemplatesDir))
                Directory.Delete(testTemplatesDir, true);
            if (Directory.Exists(testScreenshotDir))
                Directory.Delete(testScreenshotDir, true);
        }
    }

    [Fact]
    public void 完整流程_游戏分析服务分析截图 ()
    {
        // Arrange
        var testTemplatesDir = Path.Combine(Path.GetTempPath(), "templates_" + Guid.NewGuid());
        var testScreenshotDir = Path.Combine(Path.GetTempPath(), "screenshots_" + Guid.NewGuid());
        Directory.CreateDirectory(testTemplatesDir);
        Directory.CreateDirectory(testScreenshotDir);

        // 创建模板文件
        var hpMpDir = Path.Combine(testTemplatesDir, "hp_mp");
        var levelDir = Path.Combine(testTemplatesDir, "level");
        Directory.CreateDirectory(hpMpDir);
        Directory.CreateDirectory(levelDir);

        using var testBitmap = new Bitmap(10, 10);
        for (int i = 0; i <= 9; i++)
        {
            testBitmap.Save(Path.Combine(hpMpDir, $"char_{i}.png"), System.Drawing.Imaging.ImageFormat.Png);
            testBitmap.Save(Path.Combine(levelDir, $"char_{i}.png"), System.Drawing.Imaging.ImageFormat.Png);
        }

        var templateManager = new TemplateManager(testTemplatesDir, testScreenshotDir);
        templateManager.LoadTemplates();
        var gameLog = new GameLog();

        try
        {
            // Act
            var healthBarAnalyzer = new HealthBarAnalyzer(templateManager);
            var levelAnalyzer = new LevelAnalyzer(templateManager);
            var gameAnalysisService = new GameAnalysisService(healthBarAnalyzer, levelAnalyzer, gameLog);

            using var testScreenshot = new Bitmap(800, 600);
            using var g = Graphics.FromImage(testScreenshot);
            g.Clear(Color.White);

            bool analysisCompleted = false;
            gameAnalysisService.AnalysisCompleted += (info) => analysisCompleted = true;

            gameAnalysisService.Analyze(testScreenshot);

            // Assert
            Assert.True(analysisCompleted, "AnalysisCompleted 事件应该被触发");
            Assert.NotNull(gameAnalysisService.LastResult);
            Assert.Equal(2, gameLog.Count); // HP 和 MP 日志
        }
        finally
        {
            if (Directory.Exists(testTemplatesDir))
                Directory.Delete(testTemplatesDir, true);
            if (Directory.Exists(testScreenshotDir))
                Directory.Delete(testScreenshotDir, true);
        }
    }

    [Fact]
    public void 完整流程_定时截图服务与分析服务集成 ()
    {
        // Arrange
        var testTemplatesDir = Path.Combine(Path.GetTempPath(), "templates_" + Guid.NewGuid());
        var testScreenshotDir = Path.Combine(Path.GetTempPath(), "screenshots_" + Guid.NewGuid());
        Directory.CreateDirectory(testTemplatesDir);
        Directory.CreateDirectory(testScreenshotDir);

        var templateManager = new TemplateManager(testTemplatesDir, testScreenshotDir);
        var gameLog = new GameLog();

        // 创建模板文件
        var hpMpDir = Path.Combine(testTemplatesDir, "hp_mp");
        var levelDir = Path.Combine(testTemplatesDir, "level");
        Directory.CreateDirectory(hpMpDir);
        Directory.CreateDirectory(levelDir);

        using var testBitmap = new Bitmap(10, 10);
        for (int i = 0; i <= 9; i++)
        {
            testBitmap.Save(Path.Combine(hpMpDir, $"char_{i}.png"), System.Drawing.Imaging.ImageFormat.Png);
            testBitmap.Save(Path.Combine(levelDir, $"char_{i}.png"), System.Drawing.Imaging.ImageFormat.Png);
        }
        templateManager.LoadTemplates();

        var healthBarAnalyzer = new HealthBarAnalyzer(templateManager);
        var levelAnalyzer = new LevelAnalyzer(templateManager);
        var gameAnalysisService = new GameAnalysisService(healthBarAnalyzer, levelAnalyzer, gameLog);

        var capturer = new WindowCapturer();
        var saver = new ScreenshotSaver(capturer);
        var finder = new WindowFinder();
        var timedService = new TimedScreenshotService(finder, capturer, saver, _testOutputDir);

        bool screenshotCaptured = false;
        timedService.ScreenshotCaptured += (bitmap) => screenshotCaptured = true;

        var enumerator = new WindowEnumerator();
        var windows = enumerator.EnumWindows();

        try
        {
            Assert.NotEmpty(windows);
            var targetWindow = windows[0];

            // Act - 启动定时截图服务
            timedService.Start(targetWindow.Handle, 5);
            Assert.True(timedService.IsRunning);

            // 触发一次截图
            timedService.TriggerTickForTest();

            // Assert - 验证截图事件被触发
            Assert.True(screenshotCaptured, "ScreenshotCaptured 事件应该被触发");
        }
        finally
        {
            timedService.Dispose();
            gameAnalysisService.Dispose();
            Cleanup();
            if (Directory.Exists(testTemplatesDir))
                Directory.Delete(testTemplatesDir, true);
            if (Directory.Exists(testScreenshotDir))
                Directory.Delete(testScreenshotDir, true);
        }
    }

    private bool _cleanedUp = false;

    private void Cleanup()
    {
        if (_cleanedUp)
            return;

        var configDir = Path.GetDirectoryName(_testConfigPath);
        if (!string.IsNullOrEmpty(configDir) && Directory.Exists(configDir))
            Directory.Delete(configDir, true);

        if (Directory.Exists(_testOutputDir))
            Directory.Delete(_testOutputDir, true);

        _cleanedUp = true;
    }

    public void Dispose()
    {
        Cleanup();
    }
}
