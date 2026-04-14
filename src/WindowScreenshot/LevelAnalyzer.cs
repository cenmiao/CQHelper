using System.Drawing;

namespace WindowScreenshot;

/// <summary>
/// 等级分析器
/// </summary>
public class LevelAnalyzer : IGameAnalyzer
{
    private readonly TemplateManager _templateManager;
    private readonly RoiConfig _levelRoi;

    /// <summary>
    /// 识别结果
    /// </summary>
    public string? LastResult { get; private set; }

    /// <summary>
    /// 初始化 LevelAnalyzer 的新实例
    /// </summary>
    /// <param name="templateManager">模板管理器</param>
    /// <param name="levelRoi">等级 ROI 配置</param>
    public LevelAnalyzer(TemplateManager templateManager, RoiConfig? levelRoi = null)
    {
        _templateManager = templateManager;
        _levelRoi = levelRoi ?? new RoiConfig
        {
            XPercent = 91,
            YPercent = 63,
            WidthPercent = 5,
            HeightPercent = 3
        };
    }

    /// <summary>
    /// 分析截图
    /// </summary>
    /// <param name="screenshot">游戏窗口截图</param>
    /// <returns>识别结果字符串</returns>
    public string? Analyze(Bitmap screenshot)
    {
        if (!_templateManager.IsLoaded)
            return null;

        try
        {
            // 计算 Level ROI
            using var levelRoiRect = CalculateRoi(screenshot, _levelRoi);
            var levelResult = _templateManager.Recognize(levelRoiRect, _templateManager.LevelTemplates);

            LastResult = levelResult;
            return LastResult;
        }
        catch (Exception ex)
        {
            LastResult = null;
            Console.WriteLine($"[LevelAnalyzer] 分析失败：{ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// 计算 ROI 区域
    /// </summary>
    /// <param name="source">源图像</param>
    /// <param name="roi">ROI 配置</param>
    /// <returns>ROI 图像</returns>
    private Bitmap CalculateRoi(Bitmap source, RoiConfig roi)
    {
        var x = (int)(source.Width * roi.XPercent / 100);
        var y = (int)(source.Height * roi.YPercent / 100);
        var width = (int)(source.Width * roi.WidthPercent / 100);
        var height = (int)(source.Height * roi.HeightPercent / 100);

        var roiRect = new Rectangle(x, y, width, height);
        var roiImage = new Bitmap(width, height);

        using (var g = Graphics.FromImage(roiImage))
        {
            g.DrawImage(source, new Rectangle(0, 0, width, height), roiRect, GraphicsUnit.Pixel);
        }

        return roiImage;
    }
}
