using System.IO;
using System.Text.Json;

namespace WindowScreenshot;

/// <summary>
/// 配置管理器 - 负责配置的读写
/// </summary>
public class ConfigManager
{
    private readonly string _configPath;

    /// <summary>
    /// 初始化 ConfigManager 的新实例
    /// </summary>
    /// <param name="configPath">配置文件路径</param>
    public ConfigManager(string configPath)
    {
        _configPath = configPath;
    }

    /// <summary>
    /// 保存配置到文件
    /// </summary>
    /// <param name="settings">配置对象</param>
    public void Save(ScreenshotSettings settings)
    {
        try
        {
            var directory = Path.GetDirectoryName(_configPath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };
            var json = JsonSerializer.Serialize(settings, options);
            File.WriteAllText(_configPath, json, System.Text.Encoding.UTF8);
        }
        catch (Exception)
        {
            // 静默失败
        }
    }

    /// <summary>
    /// 从文件加载配置
    /// </summary>
    /// <returns>配置对象</returns>
    public ScreenshotSettings Load()
    {
        try
        {
            if (!File.Exists(_configPath))
            {
                return new ScreenshotSettings();
            }

            var json = File.ReadAllText(_configPath, System.Text.Encoding.UTF8);
            var settings = JsonSerializer.Deserialize<ScreenshotSettings>(json);
            return settings ?? new ScreenshotSettings();
        }
        catch (Exception)
        {
            return new ScreenshotSettings();
        }
    }
}
