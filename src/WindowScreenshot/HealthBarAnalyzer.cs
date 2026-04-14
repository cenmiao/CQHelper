using System.Drawing;

namespace WindowScreenshot;

/// <summary>
/// ROI 配置
/// </summary>
public class RoiConfig
{
    public double XPercent { get; set; }
    public double YPercent { get; set; }
    public double WidthPercent { get; set; }
    public double HeightPercent { get; set; }
}

/// <summary>
/// 血量/蓝量分析器
/// </summary>
public class HealthBarAnalyzer : IGameAnalyzer
{
    private readonly TemplateManager _templateManager;
    private readonly RoiConfig _hpRoi;
    private readonly RoiConfig _mpRoi;

    /// <summary>
    /// 识别结果
    /// </summary>
    public string? LastResult { get; private set; }

    /// <summary>
    /// 初始化 HealthBarAnalyzer 的新实例
    /// </summary>
    /// <param name="templateManager">模板管理器</param>
    /// <param name="hpRoi">HP ROI 配置</param>
    /// <param name="mpRoi">MP ROI 配置</param>
    public HealthBarAnalyzer(TemplateManager templateManager, RoiConfig? hpRoi = null, RoiConfig? mpRoi = null)
    {
        _templateManager = templateManager;
        _hpRoi = hpRoi ?? new RoiConfig
        {
            XPercent = 1.5,
            YPercent = 93,
            WidthPercent = 7,
            HeightPercent = 4
        };
        _mpRoi = mpRoi ?? new RoiConfig
        {
            XPercent = 7,
            YPercent = 93,
            WidthPercent = 7,
            HeightPercent = 4
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
            // 计算 HP ROI
            using var hpRoiRect = CalculateRoi(screenshot, _hpRoi);
            var hpResult = _templateManager.Recognize(hpRoiRect, _templateManager.HpMpTemplates);

            // 计算 MP ROI
            using var mpRoiRect = CalculateRoi(screenshot, _mpRoi);
            var mpResult = _templateManager.Recognize(mpRoiRect, _templateManager.HpMpTemplates);

            LastResult = $"{hpResult}/{hpResult} {mpResult}/{mpResult}";
            return LastResult;
        }
        catch (Exception ex)
        {
            LastResult = null;
            Console.WriteLine($"[HealthBarAnalyzer] 分析失败：{ex.Message}");
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
