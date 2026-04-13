using System.Drawing;

namespace WindowScreenshot;

/// <summary>
/// 定时截图服务 - 封装定时器逻辑
/// </summary>
public class TimedScreenshotService : IDisposable
{
    private readonly System.Windows.Forms.Timer _timer;
    private readonly WindowFinder _finder;
    private readonly WindowCapturer _capturer;
    private readonly ScreenshotSaver _saver;
    private readonly string _outputDirectory;
    private IntPtr _targetWindowHandle;
    private bool _disposed = false;

    /// <summary>
    /// 是否正在运行
    /// </summary>
    public bool IsRunning { get; private set; }

    /// <summary>
    /// 初始化 TimedScreenshotService 的新实例
    /// </summary>
    public TimedScreenshotService(WindowFinder finder, WindowCapturer capturer, ScreenshotSaver saver, string outputDirectory)
    {
        _finder = finder;
        _capturer = capturer;
        _saver = saver;
        _outputDirectory = outputDirectory;
        IsRunning = false;

        _timer = new System.Windows.Forms.Timer();
        _timer.Tick += Timer_Tick;
    }

    /// <summary>
    /// 启动定时截图
    /// </summary>
    /// <param name="targetWindowHandle">目标窗口句柄</param>
    /// <param name="intervalSeconds">定时间隔（秒）</param>
    /// <exception cref="ArgumentException">间隔必须大于 0</exception>
    public void Start(IntPtr targetWindowHandle, int intervalSeconds)
    {
        if (intervalSeconds <= 0)
        {
            throw new ArgumentException("定时间隔必须大于 0", nameof(intervalSeconds));
        }

        _targetWindowHandle = targetWindowHandle;
        _timer.Interval = intervalSeconds * 1000;
        _timer.Start();
        IsRunning = true;
    }

    /// <summary>
    /// 停止定时截图
    /// </summary>
    public void Stop()
    {
        _timer.Stop();
        IsRunning = false;
    }

    /// <summary>
    /// 检查窗口是否存在
    /// </summary>
    /// <param name="handle">窗口句柄</param>
    /// <returns>窗口是否存在</returns>
    public bool CheckWindowExists(IntPtr handle)
    {
        return WindowCapturer.IsWindowValid(handle);
    }

    private void Timer_Tick(object? sender, EventArgs e)
    {
        if (!_targetWindowHandle.Equals(IntPtr.Zero) && CheckWindowExists(_targetWindowHandle))
        {
            PerformScreenshot();
        }
    }

    /// <summary>
    /// 执行截图操作
    /// </summary>
    private void PerformScreenshot()
    {
        try
        {
            using var bitmap = _capturer.Capture(_targetWindowHandle);
            var filename = GenerateTimestampedFilename();
            var fullPath = Path.Combine(_outputDirectory, filename);

            if (!Directory.Exists(_outputDirectory))
            {
                Directory.CreateDirectory(_outputDirectory);
            }

            bitmap.Save(fullPath, System.Drawing.Imaging.ImageFormat.Png);
        }
        catch (Exception)
        {
            // 静默失败
        }
    }

    private string GenerateTimestampedFilename()
    {
        var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        return $"timed_screenshot_{timestamp}.png";
    }

    /// <summary>
    /// 释放资源
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _timer?.Stop();
                _timer?.Dispose();
            }
            _disposed = true;
        }
    }
}
