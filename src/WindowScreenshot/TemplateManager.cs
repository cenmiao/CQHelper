using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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
    /// 从截图提取模板（首次启动）
    /// </summary>
    /// <param name="sourcePath">源截图路径（HP-MP.png 或 LEVEL.png）</param>
    /// <param name="outputDir">输出模板目录</param>
    /// <param name="prefix">文件名前缀（用于用户确认）</param>
    /// <returns>提取的字符数量</returns>
    public int ExtractTemplates(string sourcePath, string outputDir, string prefix = "")
    {
        if (!File.Exists(sourcePath))
        {
            Console.WriteLine($"[TemplateManager] 源文件不存在：{sourcePath}");
            return 0;
        }

        // 创建输出目录
        Directory.CreateDirectory(outputDir);

        // 加载截图
        using var sourceImage = new Bitmap(sourcePath);

        // 提取字符
        var characters = ExtractCharacters(sourceImage);

        // 保存模板
        for (int i = 0; i < characters.Count; i++)
        {
            var savePath = Path.Combine(outputDir, $"char_{i}.png");
            characters[i].Save(savePath, System.Drawing.Imaging.ImageFormat.Png);
        }

        Console.WriteLine($"[TemplateManager] 从 {prefix} 提取了 {characters.Count} 个字符到 {outputDir}");
        return characters.Count;
    }

    /// <summary>
    /// 从图像提取字符（二值化、轮廓检测、字符分割）
    /// </summary>
    /// <param name="source">源图像</param>
    /// <returns>字符位图列表</returns>
    private List<Bitmap> ExtractCharacters(Bitmap source)
    {
        var characters = new List<Bitmap>();

        using (var mat = BitmapConverter.ToMat(source))
        using (var gray = new Mat())
        using (var binary = new Mat())
        {
            // 转换为灰度图
            Cv2.CvtColor(mat, gray, ColorConversionCodes.BGR2GRAY);

            // 二值化（阈值可根据实际情况调整）
            Cv2.Threshold(gray, binary, 200, 255, ThresholdTypes.BinaryInv);

            // 轮廓检测
            var contours = Cv2.FindContoursAsArray(binary, RetrievalModes.External, ContourApproximationModes.ApproxSimple);

            // 提取每个字符
            foreach (var contour in contours)
            {
                var rect = Cv2.BoundingRect(contour);

                // 过滤太小的轮廓（噪点）
                if (rect.Width < 5 || rect.Height < 5)
                    continue;

                // 提取 ROI
                var roi = mat[rect.Y..(rect.Y + rect.Height), rect.X..(rect.X + rect.Width)];

                // 转换为 Bitmap
                using var bitmap = BitmapConverter.ToBitmap(roi);
                characters.Add(new Bitmap(bitmap));
            }
        }

        return characters;
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
    /// 从截图提取模板（首次启动，异步版本带用户确认）
    /// </summary>
    /// <param name="confirmCallback">用户确认回调函数，参数为提示信息，返回是否继续</param>
    /// <returns>是否提取成功</returns>
    public async Task<bool> ExtractTemplatesAsync(Func<string, Task<bool>> confirmCallback)
    {
        var hpMpSourcePath = Path.Combine(_screenshotDirectory, "HP-MP.png");
        var levelSourcePath = Path.Combine(_screenshotDirectory, "LEVEL.png");

        if (!File.Exists(hpMpSourcePath) || !File.Exists(levelSourcePath))
        {
            Console.WriteLine($"[TemplateManager] 源截图文件不存在，需要 HP-MP.png 和 LEVEL.png");
            return false;
        }

        // 创建模板目录
        var hpMpTemplateDir = Path.Combine(_templatesDirectory, "hp_mp");
        var levelTemplateDir = Path.Combine(_templatesDirectory, "level");

        Directory.CreateDirectory(hpMpTemplateDir);
        Directory.CreateDirectory(levelTemplateDir);

        // 提取 HP/MP 模板
        using (var hpMpImage = new Bitmap(hpMpSourcePath))
        {
            var hpMpChars = ExtractCharacters(hpMpImage);

            // 弹出对话框让用户确认数值
            var confirmed = await confirmCallback($"检测到 HP/MP 字符，共 {hpMpChars.Count} 个，是否继续？");
            if (!confirmed)
                return false;

            // 保存模板
            for (int i = 0; i < hpMpChars.Count; i++)
            {
                var savePath = Path.Combine(hpMpTemplateDir, $"char_{i}.png");
                hpMpChars[i].Save(savePath, System.Drawing.Imaging.ImageFormat.Png);
            }
        }

        // 提取等级模板
        using (var levelImage = new Bitmap(levelSourcePath))
        {
            var levelChars = ExtractCharacters(levelImage);

            var confirmed = await confirmCallback($"检测到等级字符，共 {levelChars.Count} 个，是否继续？");
            if (!confirmed)
                return false;

            for (int i = 0; i < levelChars.Count; i++)
            {
                var savePath = Path.Combine(levelTemplateDir, $"char_{i}.png");
                levelChars[i].Save(savePath, System.Drawing.Imaging.ImageFormat.Png);
            }
        }

        // 加载到内存
        return LoadTemplates();
    }

    /// <summary>
    /// 获取手动模板放置指南
    /// </summary>
    /// <returns>用户指南字符串</returns>
    public static string GetManualTemplateGuide()
    {
        return @"
=== 手动模板放置指南 ===

如果自动模板提取失败，您可以手动准备模板文件：

1. 准备截图文件
   - 截取包含血量/蓝量数字的图片，保存为 HP-MP.png
   - 截取包含等级数字的图片，保存为 LEVEL.png
   - 将这两个文件放在 screenshot 目录下

2. 重启应用
   - 应用会自动从截图中提取模板

3. 或者手动创建模板
   - 在 templates/hp_mp/ 目录下放置 char_0.png 到 char_9.png（血量/蓝量数字模板）
   - 在 templates/level/ 目录下放置 char_0.png 到 char_9.png（等级数字模板）
   - 如有需要，可以添加 char_slash.png（斜杠分隔符）

4. 模板目录位置：
   - HP/MP 模板：templates/hp_mp/
   - 等级模板：templates/level/

========================
";
    }

    /// <summary>
    /// 检查手动模板是否已准备好
    /// </summary>
    /// <returns>模板是否就绪</returns>
    public bool HasManualTemplates()
    {
        var hpMpDir = Path.Combine(_templatesDirectory, "hp_mp");
        var levelDir = Path.Combine(_templatesDirectory, "level");

        // 检查至少有一个数字模板
        bool hasHpMpTemplates = Directory.Exists(hpMpDir) &&
            Enumerable.Range(0, 10).Any(i => File.Exists(Path.Combine(hpMpDir, $"char_{i}.png")));

        bool hasLevelTemplates = Directory.Exists(levelDir) &&
            Enumerable.Range(0, 10).Any(i => File.Exists(Path.Combine(levelDir, $"char_{i}.png")));

        return hasHpMpTemplates || hasLevelTemplates;
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
