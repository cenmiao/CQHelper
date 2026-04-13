namespace WindowScreenshot.Tests;

public class WindowFinderTests
{
    [Fact]
    public void FindWindow_TitleAndClassNameMatch_ShouldReturnWindowHandle()
    {
        var finder = new WindowFinder();
        var enumerator = new WindowEnumerator();
        var windows = enumerator.EnumWindows();

        Assert.NotEmpty(windows);
        var targetWindow = windows[0];

        var foundHandle = finder.FindWindow(targetWindow.Title, targetWindow.ClassName);

        Assert.Equal(targetWindow.Handle, foundHandle);
    }

    [Fact]
    public void FindWindow_WindowDoesNotExist_ShouldReturnZero()
    {
        var finder = new WindowFinder();

        var handle = finder.FindWindow("___NON_EXISTENT_WINDOW___", "Static");

        Assert.Equal(IntPtr.Zero, handle);
    }

    [Fact]
    public void FindWindow_TitleOnlyMatch_ShouldReturnFirstMatch()
    {
        var finder = new WindowFinder();
        var enumerator = new WindowEnumerator();
        var windows = enumerator.EnumWindows();

        Assert.NotEmpty(windows);
        var targetWindow = windows[0];

        var foundHandle = finder.FindByTitle(targetWindow.Title);

        Assert.NotEqual(IntPtr.Zero, foundHandle);
    }
}
