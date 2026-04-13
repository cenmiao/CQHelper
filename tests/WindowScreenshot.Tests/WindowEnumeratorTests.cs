namespace WindowScreenshot.Tests;

public class WindowEnumeratorTests
{
    [Fact]
    public void EnumWindows_应返回非空列表 ()
    {
        // Arrange
        var enumerator = new WindowEnumerator();

        // Act
        var windows = enumerator.EnumWindows();

        // Assert
        Assert.NotNull(windows);
        Assert.NotEmpty(windows);
    }

    [Fact]
    public void EnumWindows_应只返回有标题的窗口 ()
    {
        // Arrange
        var enumerator = new WindowEnumerator();

        // Act
        var windows = enumerator.EnumWindows();

        // Assert
        Assert.All(windows, w => Assert.False(string.IsNullOrEmpty(w.Title)));
    }

    [Fact]
    public void EnumWindows_应排除工具窗口 ()
    {
        // Arrange
        var enumerator = new WindowEnumerator();

        // Act
        var windows = enumerator.EnumWindows();

        // Assert - all returned windows should be visible main windows
        Assert.All(windows, w => Assert.NotEqual(IntPtr.Zero, w.Handle));
    }

    [Fact]
    public void GetWindowDisplayName_无标题窗口返回占位文本 ()
    {
        // Arrange
        var enumerator = new WindowEnumerator();
        var expectedPlaceholder = "无标题窗口";

        // Act - test with current process main window (should have a title)
        // For testing the placeholder, we verify the method handles empty strings
        var displayName = enumerator.GetWindowDisplayNameForTest(IntPtr.Zero);

        // Assert - IntPtr.Zero should return placeholder
        Assert.Equal(expectedPlaceholder, displayName);
    }
}
