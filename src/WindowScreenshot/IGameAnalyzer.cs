using System.Drawing;

namespace WindowScreenshot;

/// <summary>
/// 游戏分析器接口
/// </summary>
public interface IGameAnalyzer
{
    /// <summary>
    /// 分析截图，返回识别结果
    /// </summary>
    /// <param name="screenshot">游戏窗口截图</param>
    /// <returns>识别结果字符串</returns>
    string? Analyze(Bitmap screenshot);
}
