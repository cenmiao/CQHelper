namespace WindowScreenshot.Tests;

public class ScreenshotSettingsTests
{
    [Fact]
    public void 默认值_应为空字符串和零 ()
    {
        var settings = new ScreenshotSettings();

        Assert.Equal("", settings.TargetWindowTitle);
        Assert.Equal("", settings.TargetWindowClassName);
        Assert.Equal(0, settings.IntervalSeconds);
        Assert.False(settings.IsEnabled);
    }

    [Fact]
    public void 初始化设置_属性应正确赋值 ()
    {
        var settings = new ScreenshotSettings
        {
            TargetWindowTitle = "Test Window",
            TargetWindowClassName = "Notepad",
            IntervalSeconds = 5,
            IsEnabled = true
        };

        Assert.Equal("Test Window", settings.TargetWindowTitle);
        Assert.Equal("Notepad", settings.TargetWindowClassName);
        Assert.Equal(5, settings.IntervalSeconds);
        Assert.True(settings.IsEnabled);
    }
}
