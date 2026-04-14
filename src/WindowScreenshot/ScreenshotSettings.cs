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

    // 游戏辅助配置
    /// <summary>
    /// 是否启用游戏分析
    /// </summary>
    public bool EnableGameAnalysis { get; set; } = false;

    /// <summary>
    /// HP ROI X 百分比
    /// </summary>
    public double HpRoiXPercent { get; set; } = 1.5;

    /// <summary>
    /// HP ROI Y 百分比
    /// </summary>
    public double HpRoiYPercent { get; set; } = 93;

    /// <summary>
    /// MP ROI X 百分比
    /// </summary>
    public double MpRoiXPercent { get; set; } = 7;

    /// <summary>
    /// MP ROI Y 百分比
    /// </summary>
    public double MpRoiYPercent { get; set; } = 93;

    /// <summary>
    /// Level ROI X 百分比
    /// </summary>
    public double LevelRoiXPercent { get; set; } = 91;

    /// <summary>
    /// Level ROI Y 百分比
    /// </summary>
    public double LevelRoiYPercent { get; set; } = 63;
}
