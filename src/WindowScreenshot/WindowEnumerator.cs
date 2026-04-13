using System.Runtime.InteropServices;
using System.Text;

namespace WindowScreenshot;

/// <summary>
/// 枚举系统窗口
/// </summary>
public class WindowEnumerator
{
    // Windows API 函数声明
    [DllImport("user32.dll")]
    private static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern int GetClassName(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

    [DllImport("user32.dll")]
    private static extern bool IsWindowVisible(IntPtr hWnd);

    [DllImport("user32.dll")]
    private static extern bool IsIconic(IntPtr hWnd);

    [DllImport("user32.dll")]
    private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

    [DllImport("user32.dll")]
    private static extern IntPtr GetWindow(IntPtr hWnd, uint uCmd);

    // 委托类型
    private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

    // GetWindow 命令
    private const uint GW_OWNER = 4;

    /// <summary>
    /// 枚举所有可见的桌面窗口
    /// </summary>
    /// <returns>窗口信息列表</returns>
    public List<WindowInfo> EnumWindows()
    {
        var windows = new List<WindowInfo>();

        EnumWindows((hWnd, lParam) =>
        {
            // 跳过不可见窗口
            if (!IsWindowVisible(hWnd))
                return true;

            // 跳过最小化窗口
            if (IsIconic(hWnd))
                return true;

            // 跳过子窗口（只枚举顶层窗口）
            if (GetWindow(hWnd, GW_OWNER) != IntPtr.Zero)
                return true;

            // 获取窗口标题
            var title = GetWindowDisplayName(hWnd);
            // 获取窗口类名
            var className = GetWindowClassName(hWnd);

            // 跳过占位文本窗口（无实际标题）
            if (title == "无标题窗口")
                return true;

            windows.Add(new WindowInfo(hWnd, title, className));

            return true;
        }, IntPtr.Zero);

        return windows;
    }

    /// <summary>
    /// 获取窗口的显示名称
    /// </summary>
    /// <param name="hWnd">窗口句柄</param>
    /// <returns>窗口标题，如果无标题则返回占位文本</returns>
    public string GetWindowDisplayName(IntPtr hWnd)
    {
        const int maxLength = 1024;
        var sb = new StringBuilder(maxLength);
        var length = GetWindowText(hWnd, sb, maxLength);

        if (length == 0)
            return "无标题窗口";

        var title = sb.ToString();

        return string.IsNullOrEmpty(title) ? "无标题窗口" : title;
    }

    /// <summary>
    /// 获取窗口的类名
    /// </summary>
    /// <param name="hWnd">窗口句柄</param>
    /// <returns>窗口类名</returns>
    public string GetWindowClassName(IntPtr hWnd)
    {
        const int maxLength = 256;
        var sb = new StringBuilder(maxLength);
        var length = GetClassName(hWnd, sb, maxLength);

        if (length == 0)
            return "";

        return sb.ToString();
    }

    /// <summary>
    /// 用于测试的内部方法 - 获取窗口显示名称
    /// </summary>
    internal string GetWindowDisplayNameForTest(IntPtr hWnd) => GetWindowDisplayName(hWnd);
}

// RECT 结构体
[StructLayout(LayoutKind.Sequential)]
public struct RECT
{
    public int Left;
    public int Top;
    public int Right;
    public int Bottom;
}
