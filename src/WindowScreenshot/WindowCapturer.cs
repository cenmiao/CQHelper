using System.Drawing;
using System.Runtime.InteropServices;

namespace WindowScreenshot;

/// <summary>
/// 窗口截图器 - 获取窗口边界和截图
/// </summary>
public class WindowCapturer : IDisposable
{
    // Windows API 函数声明
    [DllImport("user32.dll")]
    private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

    [DllImport("user32.dll")]
    private static extern bool IsWindow(IntPtr hWnd);

    private bool _disposed = false;

    /// <summary>
    /// 获取窗口的边界矩形
    /// </summary>
    /// <param name="handle">窗口句柄</param>
    /// <returns>窗口边界矩形</returns>
    /// <exception cref="ArgumentException">当句柄无效时抛出</exception>
    public Rectangle GetWindowBounds(IntPtr handle)
    {
        if (!IsWindow(handle))
        {
            throw new ArgumentException("无效的窗口句柄", nameof(handle));
        }

        if (!GetWindowRect(handle, out var rect))
        {
            throw new ArgumentException("无法获取窗口边界", nameof(handle));
        }

        return Rectangle.FromLTRB(rect.Left, rect.Top, rect.Right, rect.Bottom);
    }

    /// <summary>
    /// 对指定窗口进行截图
    /// </summary>
    /// <param name="handle">窗口句柄</param>
    /// <returns>截图 Bitmap 对象</returns>
    /// <exception cref="ArgumentException">当句柄无效时抛出</exception>
    public Bitmap Capture(IntPtr handle)
    {
        if (!IsWindow(handle))
        {
            throw new ArgumentException("无效的窗口句柄", nameof(handle));
        }

        var bounds = GetWindowBounds(handle);

        // 创建 Bitmap 对象
        var bitmap = new Bitmap(bounds.Width, bounds.Height);

        // 使用 Graphics.CopyFromScreen 进行截图
        using (var graphics = Graphics.FromImage(bitmap))
        {
            graphics.CopyFromScreen(
                bounds.Location,
                Point.Empty,
                bounds.Size,
                CopyPixelOperation.SourceCopy);
        }

        return bitmap;
    }

    /// <summary>
    /// 延时后对指定窗口进行截图
    /// </summary>
    /// <param name="handle">窗口句柄</param>
    /// <param name="delayMs">延时毫秒数</param>
    /// <returns>截图 Bitmap 对象</returns>
    public Bitmap CaptureWithDelay(IntPtr handle, int delayMs)
    {
        Thread.Sleep(delayMs);
        return Capture(handle);
    }

    /// <summary>
    /// 验证窗口句柄是否有效
    /// </summary>
    /// <param name="handle">窗口句柄</param>
    /// <returns>窗口是否有效</returns>
    public static bool IsWindowValid(IntPtr handle)
    {
        return IsWindow(handle);
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
                // 释放托管资源
            }
            _disposed = true;
        }
    }
}
