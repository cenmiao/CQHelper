# Game Assistant (Legend MIR) Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** 将现有窗口截图工具改造为热血传奇游戏辅助，实现血量/蓝量/等级自动识别并显示

**Architecture:** 在现有分层架构基础上新增游戏分析服务层，通过事件驱动与定时截图服务解耦。采用 OpenCvSharp4 进行模板匹配识别，模板首次启动提取 + 持久化缓存，ROI 使用相对坐标百分比定位。

**Tech Stack:** .NET 8.0, Windows Forms, OpenCvSharp4, xUnit (testing)

---

## File Structure Map

**新增文件：**
```
src/WindowScreenshot/
├── GameInfo.cs                          # 游戏数据模型
├── GameLog.cs                           # 日志管理（内存 + UI 绑定）
├── TemplateManager.cs                   # 模板管理（提取/缓存/加载/识别）
├── IGameAnalyzer.cs                     # 分析器接口
├── HealthBarAnalyzer.cs                 # 血量/蓝量分析器
├── LevelAnalyzer.cs                     # 等级分析器
└── GameAnalysisService.cs               # 分析服务协调器

tests/WindowScreenshot.Tests/
├── GameInfoTests.cs
├── GameLogTests.cs
├── TemplateManagerTests.cs
├── HealthBarAnalyzerTests.cs
├── LevelAnalyzerTests.cs
└── GameAnalysisServiceTests.cs
```

**修改文件：**
```
src/WindowScreenshot/
├── WindowScreenshot.csproj              # 添加 OpenCvSharp4 引用
├── ScreenshotSettings.cs                # 扩展游戏辅助配置
├── TimedScreenshotService.cs            # 添加 ScreenshotCaptured 事件
├── MainForm.cs                          # 添加游戏辅助逻辑
├── MainForm.Designer.cs                 # 重构为 Tab 布局
```

---

## Phase 1: 依赖与数据模型

### Task 1: 添加 OpenCvSharp4 NuGet 包

**Files:**
- Modify: `src/WindowScreenshot/WindowScreenshot.csproj`

- [ ] **Step 1: 添加 OpenCvSharp4 包引用**

编辑 `src/WindowScreenshot/WindowScreenshot.csproj`，在 `<PropertyGroup>` 后添加：

```xml
<ItemGroup>
  <PackageReference Include="OpenCvSharp4" Version="4.9.0.20240103" />
  <PackageReference Include="OpenCvSharp4.Extensions" Version="4.9.0.20240103" />
  <PackageReference Include="OpenCvSharp4.runtime.win" Version="4.9.0.20240103" />
</ItemGroup>
```

- [ ] **Step 2: 验证包引用正确**

Run: `dotnet build src/WindowScreenshot/WindowScreenshot.csproj`

Expected: 构建成功，无 NU1000 或 NU1100 错误

- [ ] **Step 3: 提交**

```bash
git add src/WindowScreenshot/WindowScreenshot.csproj
git commit -m "feat: add OpenCvSharp4 dependencies for image recognition"
```

---

### Task 2: 创建 GameInfo 数据模型

**Files:**
- Create: `src/WindowScreenshot/GameInfo.cs`
- Test: `tests/WindowScreenshot.Tests/GameInfoTests.cs`

- [ ] **Step 1: 编写 GameInfo 测试**

创建 `tests/WindowScreenshot.Tests/GameInfoTests.cs`：

```csharp
using Xunit;
using WindowScreenshot;

public class GameInfoTests
{
    [Fact]
    public void DefaultValues_ShouldBeZero()
    {
        // Arrange & Act
        var gameInfo = new GameInfo();

        // Assert
        Assert.Equal(0, gameInfo.CurrentHp);
        Assert.Equal(0, gameInfo.MaxHp);
        Assert.Equal(0, gameInfo.CurrentMp);
        Assert.Equal(0, gameInfo.MaxMp);
        Assert.Equal(0, gameInfo.Level);
    }

    [Fact]
    public void Constructor_ShouldSetProperties()
    {
        // Arrange & Act
        var gameInfo = new GameInfo
        {
            CurrentHp = 536,
            MaxHp = 536,
            CurrentMp = 545,
            MaxMp = 545,
            Level = 44
        };

        // Assert
        Assert.Equal(536, gameInfo.CurrentHp);
        Assert.Equal(536, gameInfo.MaxHp);
        Assert.Equal(545, gameInfo.CurrentMp);
        Assert.Equal(545, gameInfo.MaxMp);
        Assert.Equal(44, gameInfo.Level);
    }

    [Fact]
    public void ToString_ShouldReturnFormattedString()
    {
        // Arrange
        var gameInfo = new GameInfo
        {
            CurrentHp = 536,
            MaxHp = 536,
            CurrentMp = 545,
            MaxMp = 545,
            Level = 44
        };

        // Act
        var result = gameInfo.ToString();

        // Assert
        Assert.Contains("536", result);
        Assert.Contains("44", result);
    }
}
```

- [ ] **Step 2: 运行测试验证失败**

Run: `dotnet test tests/WindowScreenshot.Tests/WindowScreenshot.Tests.csproj --filter "GameInfoTests" -v n`

Expected: 失败，提示 `GameInfo` 类型不存在

- [ ] **Step 3: 创建 GameInfo 数据模型**

创建 `src/WindowScreenshot/GameInfo.cs`：

```csharp
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
```

- [ ] **Step 4: 运行测试验证通过**

Run: `dotnet test tests/WindowScreenshot.Tests/WindowScreenshot.Tests.csproj --filter "GameInfoTests" -v n`

Expected: 所有测试通过

- [ ] **Step 5: 提交**

```bash
git add src/WindowScreenshot/GameInfo.cs tests/WindowScreenshot.Tests/GameInfoTests.cs
git commit -m "feat: add GameInfo data model for game state"
```

---

### Task 3: 创建 GameLog 日志管理器

**Files:**
- Create: `src/WindowScreenshot/GameLog.cs`
- Test: `tests/WindowScreenshot.Tests/GameLogTests.cs`

- [ ] **Step 1: 编写 GameLog 测试**

创建 `tests/WindowScreenshot.Tests/GameLogTests.cs`：

```csharp
using Xunit;
using WindowScreenshot;

public class GameLogTests
{
    [Fact]
    public void Constructor_ShouldInitializeEmptyList()
    {
        // Arrange & Act
        var log = new GameLog();

        // Assert
        Assert.Empty(log.Entries);
        Assert.Equal(0, log.Count);
    }

    [Fact]
    public void Append_ShouldAddEntry()
    {
        // Arrange
        var log = new GameLog();

        // Act
        log.Append("Test message");

        // Assert
        Assert.Single(log.Entries);
        Assert.Equal("Test message", log.Entries[0].Message);
    }

    [Fact]
    public void Append_ShouldLimitToMaxEntries()
    {
        // Arrange
        var log = new GameLog();

        // Act - 添加 150 条日志
        for (int i = 0; i < 150; i++)
        {
            log.Append($"Message {i}");
        }

        // Assert - 最多保留 100 条
        Assert.Equal(100, log.Entries.Count);
        Assert.Equal("Message 50", log.Entries[0].Message); // 第一条应该是最早的剩余消息
        Assert.Equal("Message 149", log.Entries[99].Message);
    }

    [Fact]
    public void Clear_ShouldRemoveAllEntries()
    {
        // Arrange
        var log = new GameLog();
        log.Append("Message 1");
        log.Append("Message 2");

        // Act
        log.Clear();

        // Assert
        Assert.Empty(log.Entries);
        Assert.Equal(0, log.Count);
    }

    [Fact]
    public void Append_ShouldSetTimestamp()
    {
        // Arrange
        var log = new GameLog();
        var before = DateTime.Now;

        // Act
        log.Append("Test message");

        // Assert
        var after = DateTime.Now;
        Assert.True(log.Entries[0].Timestamp >= before && log.Entries[0].Timestamp <= after);
    }
}
```

- [ ] **Step 2: 运行测试验证失败**

Run: `dotnet test tests/WindowScreenshot.Tests/WindowScreenshot.Tests.csproj --filter "GameLogTests" -v n`

Expected: 失败，提示 `GameLog` 类型不存在

- [ ] **Step 3: 创建 GameLog 日志管理器**

创建 `src/WindowScreenshot/GameLog.cs`：

```csharp
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
    public List<GameLogEntry> GetLast(int count)
    {
        if (count >= _entries.Count)
            return new List<GameLogEntry>(_entries);

        return _entries.Skip(_entries.Count - count).ToList();
    }
}
```

- [ ] **Step 4: 运行测试验证通过**

Run: `dotnet test tests/WindowScreenshot.Tests/WindowScreenshot.Tests.csproj --filter "GameLogTests" -v n`

Expected: 所有测试通过

- [ ] **Step 5: 提交**

```bash
git add src/WindowScreenshot/GameLog.cs tests/WindowScreenshot.Tests/GameLogTests.cs
git commit -m "feat: add GameLog manager with 100-entry limit"
```

---

## Phase 2: 模板匹配系统

### Task 4: 创建 TemplateManager 模板管理器

**Files:**
- Create: `src/WindowScreenshot/TemplateManager.cs`
- Test: `tests/WindowScreenshot.Tests/TemplateManagerTests.cs`

- [ ] **Step 1: 编写 TemplateManager 测试**

创建 `tests/WindowScreenshot.Tests/TemplateManagerTests.cs`：

```csharp
using Xunit;
using System.IO;
using WindowScreenshot;

public class TemplateManagerTests : IDisposable
{
    private readonly string _testTemplatesDir;
    private readonly string _testScreenshotDir;

    public TemplateManagerTests()
    {
        _testTemplatesDir = Path.Combine(Path.GetTempPath(), $"templates_{Guid.NewGuid()}");
        _testScreenshotDir = Path.Combine(Path.GetTempPath(), $"screenshots_{Guid.NewGuid()}");
        Directory.CreateDirectory(_testTemplatesDir);
        Directory.CreateDirectory(_testScreenshotDir);
    }

    [Fact]
    public void HasCachedTemplates_ShouldReturnFalse_WhenTemplatesDirNotExists()
    {
        // Arrange
        var manager = new TemplateManager(_testTemplatesDir);

        // Act
        var result = manager.HasCachedTemplates();

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void HasCachedTemplates_ShouldReturnTrue_WhenTemplatesDirExists()
    {
        // Arrange
        var manager = new TemplateManager(_testTemplatesDir);
        Directory.CreateDirectory(Path.Combine(_testTemplatesDir, "hp_mp"));

        // Act
        var result = manager.HasCachedTemplates();

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void LoadTemplates_ShouldLoadFromCache()
    {
        // Arrange
        var manager = new TemplateManager(_testTemplatesDir);
        var hpMpDir = Path.Combine(_testTemplatesDir, "hp_mp");
        Directory.CreateDirectory(hpMpDir);
        
        // 创建模拟模板文件（实际测试中可以用空文件或真实图片）
        File.WriteAllText(Path.Combine(hpMpDir, "char_0.png"), "mock");

        // Act
        var result = manager.LoadTemplates();

        // Assert
        Assert.NotNull(result);
    }

    public void Dispose()
    {
        if (Directory.Exists(_testTemplatesDir))
            Directory.Delete(_testTemplatesDir, true);
        if (Directory.Exists(_testScreenshotDir))
            Directory.Delete(_testScreenshotDir, true);
    }
}
```

- [ ] **Step 2: 运行测试验证失败**

Run: `dotnet test tests/WindowScreenshot.Tests/WindowScreenshot.Tests.csproj --filter "TemplateManagerTests" -v n`

Expected: 失败，提示 `TemplateManager` 类型不存在

- [ ] **Step 3: 创建 TemplateManager 模板管理器**

创建 `src/WindowScreenshot/TemplateManager.cs`：

```csharp
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
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
    public bool HasCachedTemplates()
    {
        return Directory.Exists(_templatesDirectory);
    }

    /// <summary>
    /// 从缓存加载模板
    /// </summary>
    public bool LoadTemplates()
    {
        if (!HasCachedTemplates())
            return false;

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
    private List<TemplateDigit> LoadDigitTemplates(string directory)
    {
        var templates = new List<TemplateDigit>();

        // 加载 0-9
        for (int i = 0; i <= 9; i++)
        {
            var filePath = Path.Combine(directory, $"char_{i}.png");
            if (File.Exists(filePath))
            {
                using var bitmap = new Bitmap(filePath);
                var mat = BitmapConverter.ToMat(bitmap);
                templates.Add(new TemplateDigit { Image = mat, Digit = i });
            }
        }

        // 加载斜杠（用于 HP/MP）
        if (Directory.Exists(directory))
        {
            var slashPath = Path.Combine(directory, "char_slash.png");
            if (File.Exists(slashPath))
            {
                using var bitmap = new Bitmap(slashPath);
                var mat = BitmapConverter.ToMat(bitmap);
                templates.Add(new TemplateDigit { Image = mat, Digit = -1 }); // -1 表示斜杠
            }
        }

        return templates;
    }

    /// <summary>
    /// 从截图提取模板（首次启动）
    /// </summary>
    public async Task<bool> ExtractTemplatesAsync(Func<string, Task<bool>> confirmCallback)
    {
        var hpMpSourcePath = Path.Combine(_screenshotDirectory, "HP-MP.png");
        var levelSourcePath = Path.Combine(_screenshotDirectory, "LEVEL.png");

        if (!File.Exists(hpMpSourcePath) || !File.Exists(levelSourcePath))
        {
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
            
            // 弹出对话框确认数值
            var confirmed = await confirmCallback($"检测到 HP/MP 字符，共 {hpMpChars.Count} 个，是否继续？");
            if (!confirmed)
                return false;

            // 保存模板
            for (int i = 0; i < hpMpChars.Count; i++)
            {
                var savePath = Path.Combine(hpMpTemplateDir, $"char_{i}.png");
                hpMpChars[i].Save(savePath);
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
                levelChars[i].Save(savePath);
            }
        }

        // 加载到内存
        return LoadTemplates();
    }

    /// <summary>
    /// 从图像提取字符（二值化、轮廓检测、字符分割）
    /// </summary>
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
            var contours = Cv2.FindContours(binary, RetrievalTypes.External, ContourApproximationModes.ApproxSimple);

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
    /// 模板匹配识别
    /// </summary>
    public string Recognize(Bitmap roiImage, List<TemplateDigit> templates)
    {
        using var roiMat = BitmapConverter.ToMat(roiImage);
        using var gray = new Mat();
        
        Cv2.CvtColor(roiMat, gray, ColorConversionCodes.BGR2GRAY);

        var recognizedDigits = new List<int>();
        var lastMatchX = -1;

        foreach (var template in templates)
        {
            if (template.Image.Empty())
                continue;

            using var result = new Mat();
            Cv2.MatchTemplate(gray, template.Image, result, TemplateMatchModes.CCoeffNormed);

            Cv2.MinMaxLoc(result, out _, out var maxVal, out _, out var maxLoc);

            // 匹配阈值（可调整）
            if (maxVal > 0.85)
            {
                // 避免重复匹配同一位置
                if (maxLoc.X > lastMatchX - 5)
                {
                    recognizedDigits.Add(template.Digit);
                    lastMatchX = maxLoc.X;
                }
            }
        }

        // 将数字转换为字符串
        return string.Join("", recognizedDigits.Where(d => d >= 0));
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            foreach (var t in HpMpTemplates)
                t.Image.Dispose();
            foreach (var t in LevelTemplates)
                t.Image.Dispose();
            _disposed = true;
        }
    }
}
```

- [ ] **Step 4: 运行测试验证通过**

Run: `dotnet test tests/WindowScreenshot.Tests/WindowScreenshot.Tests.csproj --filter "TemplateManagerTests" -v n`

Expected: 所有测试通过

- [ ] **Step 5: 提交**

```bash
git add src/WindowScreenshot/TemplateManager.cs tests/WindowScreenshot.Tests/TemplateManagerTests.cs
git commit -m "feat: add TemplateManager for template extraction and matching"
```

---

## Phase 3: 游戏分析器实现

### Task 5: 创建 IGameAnalyzer 接口和分析器

**Files:**
- Create: `src/WindowScreenshot/IGameAnalyzer.cs`
- Create: `src/WindowScreenshot/HealthBarAnalyzer.cs`
- Create: `src/WindowScreenshot/LevelAnalyzer.cs`

- [ ] **Step 1: 创建 IGameAnalyzer 接口**

创建 `src/WindowScreenshot/IGameAnalyzer.cs`：

```csharp
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
```

- [ ] **Step 2: 创建 HealthBarAnalyzer 血量/蓝量分析器**

创建 `src/WindowScreenshot/HealthBarAnalyzer.cs`：

```csharp
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

    public string? Analyze(Bitmap screenshot)
    {
        if (!_templateManager.IsLoaded)
            return null;

        try
        {
            // 计算 HP ROI
            var hpRoiRect = CalculateRoi(screenshot, _hpRoi);
            var hpResult = _templateManager.Recognize(hpRoiRect, _templateManager.HpMpTemplates);

            // 计算 MP ROI
            var mpRoiRect = CalculateRoi(screenshot, _mpRoi);
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
```

- [ ] **Step 3: 创建 LevelAnalyzer 等级分析器**

创建 `src/WindowScreenshot/LevelAnalyzer.cs`：

```csharp
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

    public string? Analyze(Bitmap screenshot)
    {
        if (!_templateManager.IsLoaded)
            return null;

        try
        {
            // 计算 Level ROI
            var levelRoiRect = CalculateRoi(screenshot, _levelRoi);
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
```

- [ ] **Step 4: 提交**

```bash
git add src/WindowScreenshot/IGameAnalyzer.cs src/WindowScreenshot/HealthBarAnalyzer.cs src/WindowScreenshot/LevelAnalyzer.cs
git commit -m "feat: add game analyzers for HP/MP and level recognition"
```

---

## Phase 4: 游戏分析服务

### Task 6: 创建 GameAnalysisService

**Files:**
- Create: `src/WindowScreenshot/GameAnalysisService.cs`
- Test: `tests/WindowScreenshot.Tests/GameAnalysisServiceTests.cs`

- [ ] **Step 1: 创建 GameAnalysisService**

创建 `src/WindowScreenshot/GameAnalysisService.cs`：

```csharp
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

    public GameAnalysisService(HealthBarAnalyzer healthBarAnalyzer, LevelAnalyzer levelAnalyzer, GameLog gameLog)
    {
        _healthBarAnalyzer = healthBarAnalyzer;
        _levelAnalyzer = levelAnalyzer;
        _gameLog = gameLog;
    }

    /// <summary>
    /// 分析截图
    /// </summary>
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

            // 触发事件
            AnalysisCompleted?.Invoke(gameInfo);
        }
        catch (Exception ex)
        {
            _gameLog.Append($"分析异常：{ex.Message}", "Error");
        }
    }

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
```

- [ ] **Step 2: 提交**

```bash
git add src/WindowScreenshot/GameAnalysisService.cs
git commit -m "feat: add GameAnalysisService coordinator"
```

---

## Phase 5: 集成与回调机制

### Task 7: 扩展 TimedScreenshotService 支持回调

**Files:**
- Modify: `src/WindowScreenshot/TimedScreenshotService.cs`

- [ ] **Step 1: 添加 ScreenshotCaptured 事件**

编辑 `src/WindowScreenshot/TimedScreenshotService.cs`，在 `WindowNotFound` 事件后添加：

```csharp
/// <summary>
/// 当目标窗口不存在时触发的事件
/// </summary>
public event EventHandler? WindowNotFound;

/// <summary>
/// 当截图完成时触发的事件（用于游戏分析）
/// </summary>
public event Action<Bitmap>? ScreenshotCaptured;
```

- [ ] **Step 2: 在 PerformScreenshot 中触发事件**

修改 `PerformScreenshot()` 方法：

```csharp
private void PerformScreenshot()
{
    try
    {
        using var bitmap = _capturer.Capture(_targetWindowHandle);
        var filename = GenerateTimestampedFilename();
        var fullPath = Path.Combine(_outputDirectory, filename);

        if (!Directory.Exists(_outputDirectory))
        {
            Directory.CreateDirectory(_outputDirectory);
        }

        bitmap.Save(fullPath, System.Drawing.Imaging.ImageFormat.Png);

        // 触发截图完成事件
        ScreenshotCaptured?.Invoke(bitmap);
    }
    catch (IOException ex)
    {
        Console.WriteLine($"[TimedScreenshotService] 截图保存失败：{ex.Message}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"[TimedScreenshotService] 截图失败：{ex.Message}");
    }
}
```

- [ ] **Step 3: 提交**

```bash
git add src/WindowScreenshot/TimedScreenshotService.cs
git commit -m "feat: add ScreenshotCaptured event for game analysis"
```

---

## Phase 6: 配置扩展

### Task 8: 扩展 ScreenshotSettings 和 ConfigManager

**Files:**
- Modify: `src/WindowScreenshot/ScreenshotSettings.cs`

- [ ] **Step 1: 扩展 ScreenshotSettings**

编辑 `src/WindowScreenshot/ScreenshotSettings.cs`，添加游戏辅助配置：

```csharp
namespace WindowScreenshot;

/// <summary>
/// 截图配置设置
/// </summary>
public class ScreenshotSettings
{
    // 现有属性...
    public string TargetWindowTitle { get; set; } = "";
    public string TargetWindowClassName { get; set; } = "";
    public int IntervalSeconds { get; set; } = 0;
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
```

- [ ] **Step 2: 提交**

```bash
git add src/WindowScreenshot/ScreenshotSettings.cs
git commit -m "feat: extend ScreenshotSettings with game analysis config"
```

---

## Phase 7: UI 扩展

### Task 9: 重构 MainForm 为 Tab 布局

**Files:**
- Modify: `src/WindowScreenshot/MainForm.Designer.cs`
- Modify: `src/WindowScreenshot/MainForm.cs`

- [ ] **Step 1: 重构 MainForm.Designer.cs 为 Tab 布局**

（由于代码较长，这里省略完整 Designer 代码，关键修改点：）

1. 添加 `TabControl` 控件
2. 创建两个 TabPage："基础设置" 和 "游戏辅助"
3. 将原有控件移动到"基础设置"Tab
4. 在"游戏辅助"Tab 添加：
   - `hpLabel`, `mpLabel`, `levelLabel` 显示识别结果
   - `logTextBox` 显示日志
   - `clearLogButton` 清空日志

- [ ] **Step 2: 更新 MainForm.cs 添加游戏辅助逻辑**

在 `MainForm` 构造函数中初始化游戏分析组件：

```csharp
private readonly TemplateManager _templateManager;
private readonly GameLog _gameLog;
private GameAnalysisService? _gameAnalysisService;

public MainForm()
{
    InitializeComponent();
    
    // ... 现有初始化代码 ...
    
    // 游戏分析组件
    _templateManager = new TemplateManager();
    _gameLog = new GameLog();
    
    // 检查模板缓存
    if (!_templateManager.HasCachedTemplates())
    {
        // 首次启动，提取模板
        InitializeTemplatesAsync();
    }
    else
    {
        _templateManager.LoadTemplates();
    }
}
```

- [ ] **Step 3: 提交**

```bash
git add src/WindowScreenshot/MainForm.Designer.cs src/WindowScreenshot/MainForm.cs
git commit -m "feat: refactor MainForm to Tab layout with game assistant panel"
```

---

## Verification Checklist

完成所有任务后，验证以下功能：

- [ ] 构建成功：`dotnet build src/WindowScreenshot/WindowScreenshot.csproj`
- [ ] 所有测试通过：`dotnet test tests/WindowScreenshot.Tests/WindowScreenshot.Tests.csproj`
- [ ] 首次启动模板提取正常
- [ ] 后续启动模板加载正常
- [ ] 定时截图 + 分析集成流程正常
- [ ] UI 识别结果实时更新
- [ ] 日志输出功能正常

---

## Risk Mitigation

| 风险 | 应对 |
|------|------|
| OpenCvSharp4 兼容性问题 | 使用稳定版本 4.9.0，测试多环境 |
| ROI 坐标不准 | 支持配置微调，默认值先行 |
| 模板匹配失败 | 提供模板重建功能，删除缓存重新提取 |
| UI 响应慢 | 分析在后台线程执行，UI 线程只更新显示 |
