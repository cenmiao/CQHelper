using System.Diagnostics;
using System.Drawing;

namespace WindowScreenshot.Tests;

public class WindowCapturerTests
{
    [Fact]
    public void GetWindowBounds_应返回有效的矩形区域 ()
    {
        // Arrange
        var capturer = new WindowCapturer();
        var enumerator = new WindowEnumerator();
        var windows = enumerator.EnumWindows();

        Assert.NotEmpty(windows);
        var handle = windows[0].Handle;

        // Act
        var bounds = capturer.GetWindowBounds(handle);

        // Assert
        Assert.NotEqual(Rectangle.Empty, bounds);
        Assert.True(bounds.Width > 0);
        Assert.True(bounds.Height > 0);
    }

    [Fact]
    public void GetWindowBounds_无效句柄抛出异常 ()
    {
        // Arrange
        var capturer = new WindowCapturer();
        var invalidHandle = (IntPtr)(-1);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => capturer.GetWindowBounds(invalidHandle));
    }

    [Fact]
    public void Capture_应返回非空 ()
    {
        // Arrange
        var capturer = new WindowCapturer();
        var enumerator = new WindowEnumerator();
        var windows = enumerator.EnumWindows();

        Assert.NotEmpty(windows);
        var handle = windows[0].Handle;

        // Act
        using var bitmap = capturer.Capture(handle);

        // Assert
        Assert.NotNull(bitmap);
        Assert.True(bitmap.Width > 0);
        Assert.True(bitmap.Height > 0);
    }

    [Fact]
    public void Capture_截图尺寸应与窗口一致 ()
    {
        // Arrange
        var capturer = new WindowCapturer();
        var enumerator = new WindowEnumerator();
        var windows = enumerator.EnumWindows();

        Assert.NotEmpty(windows);
        var handle = windows[0].Handle;
        var bounds = capturer.GetWindowBounds(handle);

        // Act
        using var bitmap = capturer.Capture(handle);

        // Assert
        Assert.Equal(bounds.Width, bitmap.Width);
        Assert.Equal(bounds.Height, bitmap.Height);
    }

    [Fact]
    public void CaptureWithDelay_应等待指定时间后截图 ()
    {
        // Arrange
        var capturer = new WindowCapturer();
        var enumerator = new WindowEnumerator();
        var windows = enumerator.EnumWindows();

        Assert.NotEmpty(windows);
        var handle = windows[0].Handle;
        var delayMs = 100;

        // Act
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        using var bitmap = capturer.CaptureWithDelay(handle, delayMs);
        stopwatch.Stop();

        // Assert
        Assert.NotNull(bitmap);
        Assert.True(stopwatch.ElapsedMilliseconds >= delayMs - 10, $"Expected at least {delayMs}ms, but got {stopwatch.ElapsedMilliseconds}ms");
        Assert.True(bitmap.Width > 0);
        Assert.True(bitmap.Height > 0);
    }
}
