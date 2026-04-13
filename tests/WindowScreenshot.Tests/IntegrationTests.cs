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
