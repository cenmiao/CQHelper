namespace WindowScreenshot;

/// <summary>
/// 表示一个窗口信息的结构体
/// </summary>
/// <param name="Handle">窗口句柄</param>
/// <param name="Title">窗口标题</param>
/// <param name="ClassName">窗口类名</param>
public readonly struct WindowInfo(IntPtr handle, string title, string className = "")
{
    public IntPtr Handle { get; } = handle;
    public string Title { get; } = title;
    public string ClassName { get; } = className;
}
