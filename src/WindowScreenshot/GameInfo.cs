namespace WindowScreenshot;

/// <summary>
/// 游戏状态信息 - 血量/蓝量/等级
/// </summary>
public class GameInfo
{
    /// <summary>
    /// 当前血量
    /// </summary>
    public int CurrentHp { get; set; }

    /// <summary>
    /// 最大血量
    /// </summary>
    public int MaxHp { get; set; }

    /// <summary>
    /// 当前蓝量
    /// </summary>
    public int CurrentMp { get; set; }

    /// <summary>
    /// 最大蓝量
    /// </summary>
    public int MaxMp { get; set; }

    /// <summary>
    /// 角色等级
    /// </summary>
    public int Level { get; set; }

    /// <summary>
    /// 是否为空（未识别到有效数据）
    /// </summary>
    public bool IsEmpty => CurrentHp == 0 && MaxHp == 0 && CurrentMp == 0 && MaxMp == 0 && Level == 0;

    /// <summary>
    /// 格式化为显示字符串
    /// </summary>
    public override string ToString()
    {
        if (IsEmpty)
            return "未识别";

        return $"HP: {CurrentHp}/{MaxHp} | MP: {CurrentMp}/{MaxMp} | Level: {Level}";
    }
}
