using System.Drawing;

namespace WindowScreenshot;

/// <summary>
/// 游戏分析服务 - 协调多个分析器工作
/// </summary>
public class GameAnalysisService : IDisposable
{
    private readonly HealthBarAnalyzer _healthBarAnalyzer;
    private readonly LevelAnalyzer _levelAnalyzer;
    private readonly GameLog _gameLog;
    private bool _disposed = false;

    /// <summary>
    /// 是否启用分析
    /// </summary>
    public bool IsEnabled { get; set; } = true;

    /// <summary>
    /// 分析完成事件
    /// </summary>
    public event Action<GameInfo>? AnalysisCompleted;

    /// <summary>
    /// 最后分析结果
    /// </summary>
    public GameInfo? LastResult { get; private set; }

    /// <summary>
    /// 初始化 GameAnalysisService 的新实例
    /// </summary>
    /// <param name="healthBarAnalyzer">血量/蓝量分析器</param>
    /// <param name="levelAnalyzer">等级分析器</param>
    /// <param name="gameLog">日志管理器</param>
    public GameAnalysisService(HealthBarAnalyzer healthBarAnalyzer, LevelAnalyzer levelAnalyzer, GameLog gameLog)
    {
        _healthBarAnalyzer = healthBarAnalyzer;
        _levelAnalyzer = levelAnalyzer;
        _gameLog = gameLog;
    }

    /// <summary>
    /// 分析截图
    /// </summary>
    /// <param name="screenshot">游戏窗口截图</param>
    public void Analyze(Bitmap screenshot)
    {
        if (!IsEnabled)
            return;

        try
        {
            var gameInfo = new GameInfo();

            // 分析 HP/MP
            var hpMpResult = _healthBarAnalyzer.Analyze(screenshot);
            if (!string.IsNullOrEmpty(hpMpResult))
            {
                // 解析 "536/536 545/545" 格式
                var parts = hpMpResult.Split(' ');
                if (parts.Length >= 2)
                {
                    var hpParts = parts[0].Split('/');
                    var mpParts = parts[1].Split('/');

                    if (hpParts.Length == 2 && int.TryParse(hpParts[0], out var currentHp) && int.TryParse(hpParts[1], out var maxHp))
                    {
                        gameInfo.CurrentHp = currentHp;
                        gameInfo.MaxHp = maxHp;
                    }

                    if (mpParts.Length == 2 && int.TryParse(mpParts[0], out var currentMp) && int.TryParse(mpParts[1], out var maxMp))
                    {
                        gameInfo.CurrentMp = currentMp;
                        gameInfo.MaxMp = maxMp;
                    }
                }

                _gameLog.Append($"HP: {hpMpResult}", "Info");
            }
            else
            {
                _gameLog.Append("HP/MP 识别失败", "Warning");
            }

            // 分析等级
            var levelResult = _levelAnalyzer.Analyze(screenshot);
            if (!string.IsNullOrEmpty(levelResult) && int.TryParse(levelResult, out var level))
            {
                gameInfo.Level = level;
                _gameLog.Append($"等级：{level}", "Info");
            }
            else
            {
                _gameLog.Append("等级识别失败", "Warning");
            }

            LastResult = gameInfo;

            // 触发事件
            AnalysisCompleted?.Invoke(gameInfo);
        }
        catch (Exception ex)
        {
            _gameLog.Append($"分析异常：{ex.Message}", "Error");
        }
    }

    /// <summary>
    /// 释放资源
    /// </summary>
    public void Dispose()
    {
        if (!_disposed)
        {
            _healthBarAnalyzer.Dispose();
            _levelAnalyzer.Dispose();
            _disposed = true;
        }
    }
}
