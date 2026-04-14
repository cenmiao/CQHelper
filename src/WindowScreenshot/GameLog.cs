namespace WindowScreenshot;

/// <summary>
/// 日志条目
/// </summary>
public class GameLogEntry
{
    /// <summary>
    /// 时间戳
    /// </summary>
    public DateTime Timestamp { get; set; }

    /// <summary>
    /// 日志级别（Info/Warning/Error）
    /// </summary>
    public string Level { get; set; } = "Info";

    /// <summary>
    /// 日志消息
    /// </summary>
    public string Message { get; set; } = "";

    /// <summary>
    /// 格式化为显示字符串
    /// </summary>
    public override string ToString()
    {
        return $"[{Timestamp:HH:mm:ss}] {Message}";
    }
}

/// <summary>
/// 游戏日志管理器 - 管理最多 100 条日志
/// </summary>
public class GameLog
{
    private const int MaxEntries = 100;
    private readonly List<GameLogEntry> _entries = new();

    /// <summary>
    /// 日志条目列表
    /// </summary>
    public IReadOnlyList<GameLogEntry> Entries => _entries.AsReadOnly();

    /// <summary>
    /// 日志数量
    /// </summary>
    public int Count => _entries.Count;

    /// <summary>
    /// 追加日志
    /// </summary>
    /// <param name="message">日志消息</param>
    /// <param name="level">日志级别</param>
    public void Append(string message, string level = "Info")
    {
        var entry = new GameLogEntry
        {
            Timestamp = DateTime.Now,
            Level = level,
            Message = message
        };

        _entries.Add(entry);

        // 超出最大数量，删除最早的日志
        while (_entries.Count > MaxEntries)
        {
            _entries.RemoveAt(0);
        }
    }

    /// <summary>
    /// 清空所有日志
    /// </summary>
    public void Clear()
    {
        _entries.Clear();
    }

    /// <summary>
    /// 获取最后 N 条日志
    /// </summary>
    /// <param name="count">要获取的日志数量</param>
    /// <returns>最后 N 条日志</returns>
    public List<GameLogEntry> GetLast(int count)
    {
        if (count >= _entries.Count)
            return new List<GameLogEntry>(_entries);

        return _entries.Skip(_entries.Count - count).ToList();
    }
}
