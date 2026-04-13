using System.Drawing;

namespace WindowScreenshot.Tests;

public class ScreenshotSaverTests
{
    [Fact]
    public void GenerateFilename_应使用时间戳格式 ()
    {
        // Arrange
        var saver = new ScreenshotSaver();

        // Act
        var filename = saver.GenerateFilename();

        // Assert - should match pattern screenshot_yyyyMMdd_HHmmss.png
        Assert.Matches(@"screenshot_\d{8}_\d{6}\.png", filename);
    }

    [Fact]
    public void GenerateFilename_文件名应唯一 ()
    {
        // Arrange
        var saver = new ScreenshotSaver();

        // Act
        var filename1 = saver.GenerateFilename();
        Thread.Sleep(1100); // Ensure different timestamp (format is to the second)
        var filename2 = saver.GenerateFilename();

        // Assert
        Assert.NotEqual(filename1, filename2);
    }

    [Fact]
    public void Save_应保存到截图目录 ()
    {
        // Arrange
        var saver = new ScreenshotSaver();
        using var bitmap = new Bitmap(100, 100);
        var filename = "test_" + saver.GenerateFilename();
        string? path = null;

        try
        {
            // Act
            path = saver.Save(bitmap, filename);

            // Assert
            Assert.True(File.Exists(path));
            Assert.Contains("screenshot", path);
        }
        finally
        {
            // Cleanup
            if (path != null && File.Exists(path))
                File.Delete(path);
        }
    }

    [Fact]
    public void Save_目录不存在时应创建 ()
    {
        // Arrange
        var saver = new ScreenshotSaver();
        using var bitmap = new Bitmap(100, 100);
        var filename = "test_" + saver.GenerateFilename();
        string? path = null;

        try
        {
            // Act
            path = saver.Save(bitmap, filename);

            // Assert
            Assert.True(File.Exists(path));
        }
        finally
        {
            // Cleanup
            if (path != null && File.Exists(path))
                File.Delete(path);
        }
    }

    [Fact]
    public void Save_图片格式验证 ()
    {
        // Arrange
        var saver = new ScreenshotSaver();
        using var bitmap = new Bitmap(100, 100);
        var filename = "test_" + saver.GenerateFilename();
        string? path = null;

        try
        {
            // Act
            path = saver.Save(bitmap, filename);

            // Assert
            Assert.EndsWith(".png", path);
            using var image = Image.FromFile(path);
            Assert.Equal(System.Drawing.Imaging.ImageFormat.Png, image.RawFormat);
        }
        finally
        {
            // Cleanup
            if (path != null && File.Exists(path))
                File.Delete(path);
        }
    }

    [Fact]
    public void CaptureAndSave_应返回保存的文件路径 ()
    {
        // Arrange
        var saver = new ScreenshotSaver(new WindowCapturer());
        var enumerator = new WindowEnumerator();
        var windows = enumerator.EnumWindows();

        Assert.NotEmpty(windows);
        var handle = windows[0].Handle;

        try
        {
            // Act
            var path = saver.CaptureAndSave(handle);

            // Assert
            Assert.NotNull(path);
            Assert.True(File.Exists(path));
            Assert.Contains("screenshot", path);
            Assert.EndsWith(".png", path);
        }
        finally
        {
            // Cleanup
            var screenshotDir = Path.Combine(AppContext.BaseDirectory, "screenshot");
            if (Directory.Exists(screenshotDir))
            {
                foreach (var file in Directory.GetFiles(screenshotDir, "screenshot_*.png"))
                {
                    File.Delete(file);
                }
            }
        }
    }
}
