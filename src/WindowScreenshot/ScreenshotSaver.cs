using System.Drawing;

namespace WindowScreenshot;

/// <summary>
/// 截图保存器 - 生成文件名和保存截图
/// </summary>
public class ScreenshotSaver
{
    private const string SCREENSHOT_DIRECTORY = "screenshot";
    private const string TIMESTAMP_FORMAT = "yyyyMMdd_HHmmss";
    private readonly WindowCapturer _capturer;

    /// <summary>
    /// 初始化 ScreenshotSaver 的新实例
    /// </summary>
    public ScreenshotSaver() : this(new WindowCapturer())
    {
    }

    /// <summary>
    /// 初始化 ScreenshotSaver 的新实例
    /// </summary>
    /// <param name="capturer">WindowCapturer 实例</param>
    public ScreenshotSaver(WindowCapturer capturer)
    {
        _capturer = capturer;
    }

    /// <summary>
    /// 生成截图文件名
    /// </summary>
    /// <returns>时间戳格式的文件名</returns>
    public string GenerateFilename()
    {
        var timestamp = DateTime.Now.ToString(TIMESTAMP_FORMAT);
        return $"screenshot_{timestamp}.png";
    }

    /// <summary>
    /// 保存截图到文件
    /// </summary>
    /// <param name="image">要保存的 Bitmap 图像</param>
    /// <param name="filename">文件名</param>
    /// <returns>保存文件的完整路径</returns>
    public string Save(Bitmap image, string filename)
    {
        // 确保 screenshot 目录存在
        var screenshotDir = Path.Combine(AppContext.BaseDirectory, SCREENSHOT_DIRECTORY);
        if (!Directory.Exists(screenshotDir))
        {
            Directory.CreateDirectory(screenshotDir);
        }

        // 构建完整路径
        var fullPath = Path.Combine(screenshotDir, filename);

        // 以 PNG 格式保存
        image.Save(fullPath, System.Drawing.Imaging.ImageFormat.Png);

        return fullPath;
    }

    /// <summary>
    /// 捕获窗口截图并保存
    /// </summary>
    /// <param name="handle">窗口句柄</param>
    /// <returns>保存文件的完整路径</returns>
    public string CaptureAndSave(IntPtr handle)
    {
        using var bitmap = _capturer.Capture(handle);
        var filename = GenerateFilename();
        return Save(bitmap, filename);
    }
}
