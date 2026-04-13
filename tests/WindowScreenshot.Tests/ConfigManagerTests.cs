using System.IO;

namespace WindowScreenshot.Tests;

public class ConfigManagerTests
{
    private readonly string _testConfigPath;
    private readonly string _testConfigDir;

    public ConfigManagerTests()
    {
        _testConfigDir = Path.Combine(Path.GetTempPath(), "CQHelperTests_" + Guid.NewGuid());
        _testConfigPath = Path.Combine(_testConfigDir, "config.json");
    }

    [Fact]
    public void Save_ShouldWriteToJsonFile()
    {
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
            configManager.Save(settings);

            Assert.True(File.Exists(_testConfigPath));
            var content = File.ReadAllText(_testConfigPath);
            Assert.Contains("Test Window", content);
            Assert.Contains("Notepad", content);
            Assert.Contains("10", content);
        }
        finally
        {
            if (File.Exists(_testConfigPath))
                File.Delete(_testConfigPath);
            if (Directory.Exists(_testConfigDir))
                Directory.Delete(_testConfigDir, true);
        }
    }

    [Fact]
    public void Load_ShouldReturnSavedData()
    {
        var configManager = new ConfigManager(_testConfigPath);
        var originalSettings = new ScreenshotSettings
        {
            TargetWindowTitle = "Loaded Window",
            TargetWindowClassName = "Chrome",
            IntervalSeconds = 30,
            IsEnabled = false
        };

        try
        {
            configManager.Save(originalSettings);
            var loadedSettings = configManager.Load();

            Assert.Equal("Loaded Window", loadedSettings.TargetWindowTitle);
            Assert.Equal("Chrome", loadedSettings.TargetWindowClassName);
            Assert.Equal(30, loadedSettings.IntervalSeconds);
            Assert.False(loadedSettings.IsEnabled);
        }
        finally
        {
            if (File.Exists(_testConfigPath))
                File.Delete(_testConfigPath);
            if (Directory.Exists(_testConfigDir))
                Directory.Delete(_testConfigDir, true);
        }
    }

    [Fact]
    public void Load_NonExistentFile_ShouldReturnDefaultSettings()
    {
        var configManager = new ConfigManager(_testConfigPath);
        Assert.False(File.Exists(_testConfigPath));

        var settings = configManager.Load();

        Assert.Equal("", settings.TargetWindowTitle);
        Assert.Equal("", settings.TargetWindowClassName);
        Assert.Equal(0, settings.IntervalSeconds);
        Assert.False(settings.IsEnabled);
    }

    [Fact]
    public void Save_DirectoryNotExists_ShouldCreateDirectory()
    {
        var nestedPath = Path.Combine(_testConfigDir, "nested", "config.json");
        var configManager = new ConfigManager(nestedPath);
        var settings = new ScreenshotSettings();

        try
        {
            configManager.Save(settings);

            Assert.True(File.Exists(nestedPath));
            Assert.True(Directory.Exists(Path.GetDirectoryName(nestedPath)));
        }
        finally
        {
            if (Directory.Exists(_testConfigDir))
                Directory.Delete(_testConfigDir, true);
        }
    }
}
