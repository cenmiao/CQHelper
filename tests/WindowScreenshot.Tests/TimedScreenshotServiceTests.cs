namespace WindowScreenshot.Tests;

public class TimedScreenshotServiceTests : IDisposable
{
    private readonly string _testOutputDir;
    private readonly WindowEnumerator _enumerator;
    private readonly WindowCapturer _capturer;
    private readonly ScreenshotSaver _saver;
    private readonly WindowFinder _finder;

    public TimedScreenshotServiceTests()
    {
        _testOutputDir = Path.Combine(Path.GetTempPath(), "TimedScreenshotTests_" + Guid.NewGuid());
        Directory.CreateDirectory(_testOutputDir);

        _enumerator = new WindowEnumerator();
        _capturer = new WindowCapturer();
        _saver = new ScreenshotSaver(_capturer);
        _finder = new WindowFinder();
    }

    [Fact]
    public void 构造函数_应能创建服务实例 ()
    {
        var service = new TimedScreenshotService(_finder, _capturer, _saver, _testOutputDir);

        Assert.NotNull(service);
    }

    [Fact]
    public void 启动_间隔必须大于零 ()
    {
        var service = new TimedScreenshotService(_finder, _capturer, _saver, _testOutputDir);
        var windows = _enumerator.EnumWindows();
        Assert.NotEmpty(windows);
        var targetWindow = windows[0];

        Assert.Throws<ArgumentException>(() => service.Start(targetWindow.Handle, 0));
    }

    [Fact]
    public void 启动_应启动定时器 ()
    {
        var service = new TimedScreenshotService(_finder, _capturer, _saver, _testOutputDir);
        var windows = _enumerator.EnumWindows();
        Assert.NotEmpty(windows);
        var targetWindow = windows[0];

        service.Start(targetWindow.Handle, 5);

        Assert.True(service.IsRunning);

        service.Stop();
    }

    [Fact]
    public void 停止_应停止定时器 ()
    {
        var service = new TimedScreenshotService(_finder, _capturer, _saver, _testOutputDir);
        var windows = _enumerator.EnumWindows();
        Assert.NotEmpty(windows);
        var targetWindow = windows[0];

        service.Start(targetWindow.Handle, 5);
        Assert.True(service.IsRunning);

        service.Stop();

        Assert.False(service.IsRunning);
    }

    [Fact]
    public void 窗口不存在时应检测到 ()
    {
        var service = new TimedScreenshotService(_finder, _capturer, _saver, _testOutputDir);
        var invalidHandle = (IntPtr)(-1);

        var exists = service.CheckWindowExists(invalidHandle);

        Assert.False(exists);
    }

    public void Dispose()
    {
        if (Directory.Exists(_testOutputDir))
            Directory.Delete(_testOutputDir, true);

        _capturer?.Dispose();
    }
}
