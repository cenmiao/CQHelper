using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using OpenCvSharp;
using OpenCvSharp.Extensions;

namespace WindowScreenshot;

/// <summary>
/// 模板 - 数字映射
/// </summary>
public class TemplateDigit
{
    public Mat Image { get; set; } = new Mat();
    public int Digit { get; set; }
}

/// <summary>
/// 模板管理器 - 负责模板的提取、缓存、加载和识别
/// </summary>
public class TemplateManager : IDisposable
{
    private readonly string _templatesDirectory;
    private readonly string _screenshotDirectory;
    private bool _disposed = false;

    /// <summary>
    /// HP/MP 模板
    /// </summary>
    public List<TemplateDigit> HpMpTemplates { get; private set; } = new();

    /// <summary>
    /// 等级模板
    /// </summary>
    public List<TemplateDigit> LevelTemplates { get; private set; } = new();

    /// <summary>
    /// 是否已加载模板
    /// </summary>
    public bool IsLoaded => HpMpTemplates.Count > 0 || LevelTemplates.Count > 0;

    /// <summary>
    /// 初始化 TemplateManager 的新实例
    /// </summary>
    /// <param name="templatesDirectory">模板目录</param>
    /// <param name="screenshotDirectory">截图目录</param>
    public TemplateManager(string templatesDirectory, string screenshotDirectory)
    {
        _templatesDirectory = templatesDirectory;
        _screenshotDirectory = screenshotDirectory;
    }

    /// <summary>
    /// 初始化 TemplateManager 的新实例（默认路径）
    /// </summary>
    public TemplateManager()
        : this(
            Path.Combine(AppContext.BaseDirectory, "templates"),
            Path.Combine(AppContext.BaseDirectory, "screenshot"))
    {
    }

    /// <summary>
    /// 检查是否有缓存的模板
    /// </summary>
    /// <returns>是否有缓存的模板</returns>
    public bool HasCachedTemplates()
    {
        return Directory.Exists(_templatesDirectory);
    }

    /// <summary>
    /// 从缓存加载模板
    /// </summary>
    /// <returns>是否加载成功</returns>
    public bool LoadTemplates()
    {
        if (!HasCachedTemplates())
            return false;

        // 清空现有模板
        HpMpTemplates.Clear();
        LevelTemplates.Clear();

        // 加载 HP/MP 模板
        var hpMpDir = Path.Combine(_templatesDirectory, "hp_mp");
        if (Directory.Exists(hpMpDir))
        {
            HpMpTemplates = LoadDigitTemplates(hpMpDir);
        }

        // 加载等级模板
        var levelDir = Path.Combine(_templatesDirectory, "level");
        if (Directory.Exists(levelDir))
        {
            LevelTemplates = LoadDigitTemplates(levelDir);
        }

        return HpMpTemplates.Count > 0 || LevelTemplates.Count > 0;
    }

    /// <summary>
    /// 从目录加载数字模板
    /// </summary>
    /// <param name="directory">模板目录</param>
    /// <returns>模板列表</returns>
    private List<TemplateDigit> LoadDigitTemplates(string directory)
    {
        var templates = new List<TemplateDigit>();

        // 加载 0-9
        for (int i = 0; i <= 9; i++)
        {
            var filePath = Path.Combine(directory, $"char_{i}.png");
            if (File.Exists(filePath))
            {
                try
                {
                    using var bitmap = new Bitmap(filePath);
                    var mat = BitmapConverter.ToMat(bitmap);
                    templates.Add(new TemplateDigit { Image = mat.Clone(), Digit = i });
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[TemplateManager] 加载模板失败 {filePath}: {ex.Message}");
                }
            }
        }

        // 加载斜杠（用于 HP/MP）
        var slashPath = Path.Combine(directory, "char_slash.png");
        if (File.Exists(slashPath))
        {
            try
            {
                using var bitmap = new Bitmap(slashPath);
                var mat = BitmapConverter.ToMat(bitmap);
                templates.Add(new TemplateDigit { Image = mat.Clone(), Digit = -1 }); // -1 表示斜杠
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[TemplateManager] 加载斜杠模板失败 {slashPath}: {ex.Message}");
            }
        }

        return templates;
    }

    /// <summary>
    /// 模板匹配识别
    /// </summary>
    /// <param name="roiImage">ROI 图像</param>
    /// <param name="templates">模板列表</param>
    /// <returns>识别结果字符串</returns>
    public string Recognize(Bitmap roiImage, List<TemplateDigit> templates)
    {
        if (roiImage == null || templates.Count == 0)
            return "";

        using var roiMat = BitmapConverter.ToMat(roiImage);
        using var gray = new Mat();

        if (roiMat.Empty())
            return "";

        Cv2.CvtColor(roiMat, gray, ColorConversionCodes.BGR2GRAY);

        var recognizedDigits = new List<int>();
        var matchedPositions = new List<(int X, int Width)>();

        foreach (var template in templates)
        {
            if (template.Image == null || template.Image.Empty())
                continue;

            using var result = new Mat();
            Cv2.MatchTemplate(gray, template.Image, result, TemplateMatchModes.CCoeffNormed);

            // 查找所有匹配位置
            while (true)
            {
                Cv2.MinMaxLoc(result, out _, out var maxVal, out _, out var maxLoc);

                // 匹配阈值
                if (maxVal > 0.85)
                {
                    // 检查是否与已有匹配位置重叠
                    var isOverlapping = false;
                    foreach (var pos in matchedPositions)
                    {
                        if (Math.Abs(maxLoc.X - pos.X) < 5)
                        {
                            isOverlapping = true;
                            break;
                        }
                    }

                    if (!isOverlapping)
                    {
                        recognizedDigits.Add(template.Digit);
                        matchedPositions.Add((maxLoc.X, template.Image.Width));

                        // 消除已匹配区域，避免重复匹配
                        var rect = new Rect(maxLoc.X, maxLoc.Y, template.Image.Width, template.Image.Height);
                        Cv2.Rectangle(result, rect, new Scalar(0), -1);
                    }
                    else
                    {
                        break;
                    }
                }
                else
                {
                    break;
                }
            }
        }

        // 按 X 位置排序
        matchedPositions.Sort((a, b) => a.X.CompareTo(b.X));
        recognizedDigits.Sort();

        // 将数字转换为字符串
        return string.Join("", recognizedDigits.Where(d => d >= 0).OrderBy(d => d));
    }

    /// <summary>
    /// 释放资源
    /// </summary>
    public void Dispose()
    {
        if (!_disposed)
        {
            foreach (var t in HpMpTemplates)
                t.Image?.Dispose();
            foreach (var t in LevelTemplates)
                t.Image?.Dispose();
            _disposed = true;
        }
    }
}
