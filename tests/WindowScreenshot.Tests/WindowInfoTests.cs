namespace WindowScreenshot.Tests;

public class WindowInfoTests
{
    [Fact]
    public void WindowInfo_应包含句柄和标题 ()
    {
        // Arrange
        var expectedHandle = (IntPtr)12345;
        var expectedTitle = "Test Window";

        // Act
        var windowInfo = new WindowInfo(expectedHandle, expectedTitle);

        // Assert
        Assert.Equal(expectedHandle, windowInfo.Handle);
        Assert.Equal(expectedTitle, windowInfo.Title);
    }
}
