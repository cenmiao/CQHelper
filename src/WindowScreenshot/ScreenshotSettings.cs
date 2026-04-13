namespace WindowScreenshot;

/// <summary>
/// 截图配置设置
/// </summary>
public class ScreenshotSettings
{
    /// <summary>
    /// 目标窗口标题
    /// </summary>
    public string TargetWindowTitle { get; set; } = "";

    /// <summary>
    /// 目标窗口类名
    /// </summary>
    public string TargetWindowClassName { get; set; } = "";

    /// <summary>
    /// 定时间隔 (秒)
    /// </summary>
    public int IntervalSeconds { get; set; } = 0;

    /// <summary>
    /// 是否启用定时截图
    /// </summary>
    public bool IsEnabled { get; set; } = false;
}
