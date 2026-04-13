using System.Runtime.InteropServices;
using System.Text;

namespace WindowScreenshot;

/// <summary>
/// 窗口查找器 - 根据标题和类名查找窗口
/// </summary>
public class WindowFinder
{
    [DllImport("user32.dll")]
    private static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern int GetClassName(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

    [DllImport("user32.dll")]
    private static extern bool IsWindow(IntPtr hWnd);

    private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

    /// <summary>
    /// 根据窗口标题和类名查找窗口句柄
    /// </summary>
    /// <param name="title">窗口标题（支持前缀匹配）</param>
    /// <param name="className">窗口类名</param>
    /// <returns>窗口句柄，未找到返回 IntPtr.Zero</returns>
    public IntPtr FindWindow(string title, string className)
    {
        if (string.IsNullOrEmpty(title))
            return IntPtr.Zero;

        IntPtr foundHandle = IntPtr.Zero;

        EnumWindows((hWnd, lParam) =>
        {
            if (!IsWindow(hWnd))
                return true;

            var windowTitle = GetWindowTitle(hWnd);
            if (string.IsNullOrEmpty(windowTitle) || !windowTitle.StartsWith(title, StringComparison.Ordinal))
                return true;

            if (!string.IsNullOrEmpty(className))
            {
                var windowClassName = GetWindowClassName(hWnd);
                if (!windowClassName.Equals(className, StringComparison.Ordinal))
                    return true;
            }

            foundHandle = hWnd;
            return false;

        }, IntPtr.Zero);

        return foundHandle;
    }

    /// <summary>
    /// 仅根据窗口标题查找窗口句柄
    /// </summary>
    /// <param name="title">窗口标题</param>
    /// <returns>窗口句柄，未找到返回 IntPtr.Zero</returns>
    public IntPtr FindByTitle(string title)
    {
        return FindWindow(title, "");
    }

    private string GetWindowTitle(IntPtr hWnd)
    {
        const int maxLength = 1024;
        var sb = new StringBuilder(maxLength);
        var length = GetWindowText(hWnd, sb, maxLength);
        return length > 0 ? sb.ToString() : "";
    }

    private string GetWindowClassName(IntPtr hWnd)
    {
        const int maxLength = 256;
        var sb = new StringBuilder(maxLength);
        var length = GetClassName(hWnd, sb, maxLength);
        return length > 0 ? sb.ToString() : "";
    }
}
