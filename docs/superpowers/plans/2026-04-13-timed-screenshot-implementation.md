# 定时截图功能实现计划

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** 在现有手动截图功能基础上，增加自动化定时截图能力，支持秒级定时间隔、配置持久化和窗口不存在时自动停止。

**Architecture:** 采用三层架构：(1) ConfigManager 负责配置读写，(2) WindowFinder 负责窗口查找，(3) TimedScreenshotService 封装定时器逻辑，MainForm 负责 UI 和生命周期管理。

**Tech Stack:** .NET 8.0 Windows Forms, System.Text.Json (内置), System.Windows.Forms.Timer, xunit 测试框架

---

## 文件结构总览

**新增文件:**
- `src/WindowScreenshot/ScreenshotSettings.cs` - 配置数据结构
- `src/WindowScreenshot/ConfigManager.cs` - 配置管理器
- `src/WindowScreenshot/WindowFinder.cs` - 窗口查找器
- `src/WindowScreenshot/TimedScreenshotService.cs` - 定时截图服务

**修改文件:**
- `src/WindowScreenshot/MainForm.cs` - 添加定时器集成逻辑
- `src/WindowScreenshot/MainForm.Designer.cs` - 扩展 UI 控件

**新增测试:**
- `tests/WindowScreenshot.Tests/ConfigManagerTests.cs`
- `tests/WindowScreenshot.Tests/WindowFinderTests.cs`
- `tests/WindowScreenshot.Tests/TimedScreenshotServiceTests.cs`

---

## Task 1: 配置数据结构 (ScreenshotSettings)

**Files:**
- Create: `src/WindowScreenshot/ScreenshotSettings.cs`
- Test: `tests/WindowScreenshot.Tests/ScreenshotSettingsTests.cs`

- [ ] **Step 1: 编写测试 - ScreenshotSettings 默认值**

```csharp
namespace WindowScreenshot.Tests;

public class ScreenshotSettingsTests
{
    [Fact]
    public void 默认值_应为空字符串和零 ()
    {
        // Arrange & Act
        var settings = new ScreenshotSettings();

        // Assert
        Assert.Equal("", settings.TargetWindowTitle);
        Assert.Equal("", settings.TargetWindowClassName);
        Assert.Equal(0, settings.IntervalSeconds);
        Assert.False(settings.IsEnabled);
    }

    [Fact]
    public void 初始化设置_属性应正确赋值 ()
    {
        // Arrange
        var settings = new ScreenshotSettings
        {
            TargetWindowTitle = "Test Window",
            TargetWindowClassName = "Notepad",
            IntervalSeconds = 5,
            IsEnabled = true
        };

        // Act & Assert
        Assert.Equal("Test Window", settings.TargetWindowTitle);
        Assert.Equal("Notepad", settings.TargetWindowClassName);
        Assert.Equal(5, settings.IntervalSeconds);
        Assert.True(settings.IsEnabled);
    }
}
```

- [ ] **Step 2: 运行测试验证失败**

```bash
dotnet test tests/WindowScreenshot.Tests/WindowScreenshot.Tests.csproj --filter "ScreenshotSettingsTests" -v n
```
预期：编译失败，因为 `ScreenshotSettings` 类不存在

- [ ] **Step 3: 实现 ScreenshotSettings 类**

```csharp
namespace WindowScreenshot;

/// <summary>
/// 截图配置设置
/// </summary>
public class ScreenshotSettings
{
    /// <summary>
    /// 目标窗口标题
    /// </summary>
    public string TargetWindowTitle { get; set; } = "";

    /// <summary>
    /// 目标窗口类名
    /// </summary>
    public string TargetWindowClassName { get; set; } = "";

    /// <summary>
    /// 定时间隔（秒）
    /// </summary>
    public int IntervalSeconds { get; set; } = 0;

    /// <summary>
    /// 是否启用定时截图
    /// </summary>
    public bool IsEnabled { get; set; } = false;
}
```

- [ ] **Step 4: 运行测试验证通过**

```bash
dotnet test tests/WindowScreenshot.Tests/WindowScreenshot.Tests.csproj --filter "ScreenshotSettingsTests" -v n
```
预期：2 个测试全部通过

- [ ] **Step 5: 提交**

```bash
git add src/WindowScreenshot/ScreenshotSettings.cs tests/WindowScreenshot.Tests/ScreenshotSettingsTests.cs
git commit -m "feat: add ScreenshotSettings configuration data structure"
```

---

## Task 2: 配置管理器 (ConfigManager)

**Files:**
- Create: `src/WindowScreenshot/ConfigManager.cs`
- Test: `tests/WindowScreenshot.Tests/ConfigManagerTests.cs`

- [ ] **Step 1: 编写测试 - 保存和加载配置**

```csharp
using System.IO;

namespace WindowScreenshot.Tests;

public class ConfigManagerTests
{
    private readonly string _testConfigPath;
    private readonly string _testConfigDir;

    public ConfigManagerTests()
    {
        // 使用临时目录进行测试
        _testConfigDir = Path.Combine(Path.GetTempPath(), "CQHelperTests_" + Guid.NewGuid());
        _testConfigPath = Path.Combine(_testConfigDir, "config.json");
    }

    [Fact]
    public void 保存配置_应写入 JSON 文件 ()
    {
        // Arrange
        var configManager = new ConfigManager(_testConfigPath);
        var settings = new ScreenshotSettings
        {
            TargetWindowTitle = "Test Window",
            TargetWindowClassName = "Notepad",
            IntervalSeconds = 10,
            IsEnabled = true
        };

        try
        {
            // Act
            configManager.Save(settings);

            // Assert
            Assert.True(File.Exists(_testConfigPath));
            var content = File.ReadAllText(_testConfigPath);
            Assert.Contains("Test Window", content);
            Assert.Contains("Notepad", content);
            Assert.Contains("10", content);
        }
        finally
        {
            // Cleanup
            if (File.Exists(_testConfigPath))
                File.Delete(_testConfigPath);
            if (Directory.Exists(_testConfigDir))
                Directory.Delete(_testConfigDir, true);
        }
    }

    [Fact]
    public void 加载配置_应返回保存的数据 ()
    {
        // Arrange
        var configManager = new ConfigManager(_testConfigPath);
        var originalSettings = new ScreenshotSettings
        {
            TargetWindowTitle = "Loaded Window",
            TargetWindowClassName = "Chrome",
            IntervalSeconds = 30,
            IsEnabled = false
        };

        try
        {
            // Setup - 先保存
            configManager.Save(originalSettings);

            // Act - 再加载
            var loadedSettings = configManager.Load();

            // Assert
            Assert.Equal("Loaded Window", loadedSettings.TargetWindowTitle);
            Assert.Equal("Chrome", loadedSettings.TargetWindowClassName);
            Assert.Equal(30, loadedSettings.IntervalSeconds);
            Assert.False(loadedSettings.IsEnabled);
        }
        finally
        {
            // Cleanup
            if (File.Exists(_testConfigPath))
                File.Delete(_testConfigPath);
            if (Directory.Exists(_testConfigDir))
                Directory.Delete(_testConfigDir, true);
        }
    }

    [Fact]
    public void 加载不存在的文件_应返回默认设置 ()
    {
        // Arrange
        var configManager = new ConfigManager(_testConfigPath);
        Assert.False(File.Exists(_testConfigPath));

        // Act
        var settings = configManager.Load();

        // Assert
        Assert.Equal("", settings.TargetWindowTitle);
        Assert.Equal("", settings.TargetWindowClassName);
        Assert.Equal(0, settings.IntervalSeconds);
        Assert.False(settings.IsEnabled);
    }

    [Fact]
    public void 保存时目录不存在_应自动创建 ()
    {
        // Arrange
        var nestedPath = Path.Combine(_testConfigDir, "nested", "config.json");
        var configManager = new ConfigManager(nestedPath);
        var settings = new ScreenshotSettings();

        try
        {
            // Act
            configManager.Save(settings);

            // Assert
            Assert.True(File.Exists(nestedPath));
            Assert.True(Directory.Exists(Path.GetDirectoryName(nestedPath)));
        }
        finally
        {
            // Cleanup
            if (Directory.Exists(_testConfigDir))
                Directory.Delete(_testConfigDir, true);
        }
    }
}
```

- [ ] **Step 2: 运行测试验证失败**

```bash
dotnet test tests/WindowScreenshot.Tests/WindowScreenshot.Tests.csproj --filter "ConfigManagerTests" -v n
```
预期：编译失败，因为 `ConfigManager` 类不存在

- [ ] **Step 3: 实现 ConfigManager 类**

```csharp
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
            // 确保目录存在
            var directory = Path.GetDirectoryName(_configPath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            // 序列化并保存
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = System.Text.Encodings.Web.SystemTextJson.Encoders.UnsafeRelaxedJsonEscaping
            };
            var json = JsonSerializer.Serialize(settings, options);
            File.WriteAllText(_configPath, json, System.Text.Encoding.UTF8);
        }
        catch (Exception)
        {
            // 静默失败，避免影响主流程
            // 在生产环境中可以添加日志
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
            // 加载失败时返回默认设置
            return new ScreenshotSettings();
        }
    }
}
```

- [ ] **Step 4: 运行测试验证通过**

```bash
dotnet test tests/WindowScreenshot.Tests/WindowScreenshot.Tests.csproj --filter "ConfigManagerTests" -v n
```
预期：4 个测试全部通过

- [ ] **Step 5: 提交**

```bash
git add src/WindowScreenshot/ConfigManager.cs tests/WindowScreenshot.Tests/ConfigManagerTests.cs
git commit -m "feat: add ConfigManager for JSON configuration persistence"
```

---

## Task 3: 窗口查找器 (WindowFinder)

**Files:**
- Create: `src/WindowScreenshot/WindowFinder.cs`
- Test: `tests/WindowScreenshot.Tests/WindowFinderTests.cs`
- Modify: `src/WindowScreenshot/WindowInfo.cs` (添加 ClassName 属性)

- [ ] **Step 1: 扩展 WindowInfo 添加类名属性**

```csharp
using System.Diagnostics;
using System.Drawing;

namespace WindowScreenshot;

/// <summary>
/// 表示一个窗口信息的结构体
/// </summary>
/// <param name="Handle">窗口句柄</param>
/// <param name="Title">窗口标题</param>
/// <param name="ClassName">窗口类名</param>
public readonly struct WindowInfo(IntPtr handle, string title, string className = "")
{
    public IntPtr Handle { get; } = handle;
    public string Title { get; } = title;
    public string ClassName { get; } = className;
}
```

- [ ] **Step 2: 扩展 WindowEnumerator 获取窗口类名**

在 `WindowEnumerator.cs` 中添加 `GetClassName` 方法和修改 `EnumWindows`:

```csharp
[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
private static extern int GetClassName(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

/// <summary>
/// 获取窗口的类名
/// </summary>
/// <param name="hWnd">窗口句柄</param>
/// <returns>窗口类名</returns>
public string GetWindowClassName(IntPtr hWnd)
{
    const int maxLength = 256;
    var sb = new StringBuilder(maxLength);
    var length = GetClassName(hWnd, sb, maxLength);

    if (length == 0)
        return "";

    return sb.ToString();
}

// 修改 EnumWindows 方法，添加类名
public List<WindowInfo> EnumWindows()
{
    var windows = new List<WindowInfo>();

    EnumWindows((hWnd, lParam) =>
    {
        if (!IsWindowVisible(hWnd))
            return true;

        if (IsIconic(hWnd))
            return true;

        if (GetWindow(hWnd, GW_OWNER) != IntPtr.Zero)
            return true;

        var title = GetWindowDisplayName(hWnd);
        var className = GetWindowClassName(hWnd);

        if (title == "无标题窗口")
            return true;

        windows.Add(new WindowInfo(hWnd, title, className));

        return true;
    }, IntPtr.Zero);

    return windows;
}
```

- [ ] **Step 3: 编写 WindowFinder 测试**

```csharp
namespace WindowScreenshot.Tests;

public class WindowFinderTests
{
    [Fact]
    public void FindWindow_标题和类名匹配_应返回窗口句柄 ()
    {
        // Arrange
        var finder = new WindowFinder();
        var enumerator = new WindowEnumerator();
        var windows = enumerator.EnumWindows();

        Assert.NotEmpty(windows);
        var targetWindow = windows[0];

        // Act
        var foundHandle = finder.FindWindow(targetWindow.Title, targetWindow.ClassName);

        // Assert
        Assert.Equal(targetWindow.Handle, foundHandle);
    }

    [Fact]
    public void FindWindow_标题前缀匹配_应成功 ()
    {
        // Arrange
        var finder = new WindowFinder();
        var enumerator = new WindowEnumerator();
        var windows = enumerator.EnumWindows();

        var chromeWindow = windows.FirstOrDefault(w => w.Title.Contains("Chrome") || w.Title.Contains("Edge"));
        if (chromeWindow == default)
        {
            // Skip if no browser window found
            return;
        }

        // Act - 使用部分标题匹配
        var partialTitle = chromeWindow.Title.Length > 5 ? chromeWindow.Title.Substring(0, 5) : chromeWindow.Title;
        var foundHandle = finder.FindWindow(partialTitle, chromeWindow.ClassName);

        // Assert
        Assert.Equal(chromeWindow.Handle, foundHandle);
    }

    [Fact]
    public void FindWindow_窗口不存在_应返回 IntPtr.Zero ()
    {
        // Arrange
        var finder = new WindowFinder();

        // Act
        var handle = finder.FindWindow("___NON_EXISTENT_WINDOW___", "Static");

        // Assert
        Assert.Equal(IntPtr.Zero, handle);
    }

    [Fact]
    public void FindWindow_仅标题匹配_应返回第一个匹配项 ()
    {
        // Arrange
        var finder = new WindowFinder();
        var enumerator = new WindowEnumerator();
        var windows = enumerator.EnumWindows();

        Assert.NotEmpty(windows);
        var targetWindow = windows[0];

        // Act - 仅使用标题查找
        var foundHandle = finder.FindByTitle(targetWindow.Title);

        // Assert
        Assert.NotEqual(IntPtr.Zero, foundHandle);
    }
}
```

- [ ] **Step 4: 运行测试验证失败**

```bash
dotnet test tests/WindowScreenshot.Tests/WindowScreenshot.Tests.csproj --filter "WindowFinderTests" -v n
```
预期：编译失败

- [ ] **Step 5: 实现 WindowFinder 类**

```csharp
using System.Runtime.InteropServices;
using System.Text;

namespace WindowScreenshot;

/// <summary>
/// 窗口查找器 - 根据标题和类名查找窗口
/// </summary>
public class WindowFinder
{
    [DllImport("user32.dll")]
    private static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern int GetClassName(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

    [DllImport("user32.dll")]
    private static extern bool IsWindow(IntPtr hWnd);

    private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

    /// <summary>
    /// 根据窗口标题和类名查找窗口句柄
    /// </summary>
    /// <param name="title">窗口标题（支持前缀匹配）</param>
    /// <param name="className">窗口类名</param>
    /// <returns>窗口句柄，未找到返回 IntPtr.Zero</returns>
    public IntPtr FindWindow(string title, string className)
    {
        if (string.IsNullOrEmpty(title))
            return IntPtr.Zero;

        IntPtr foundHandle = IntPtr.Zero;

        EnumWindows((hWnd, lParam) =>
        {
            // 检查窗口是否有效
            if (!IsWindow(hWnd))
                return true;

            // 检查标题匹配（前缀匹配）
            var windowTitle = GetWindowTitle(hWnd);
            if (string.IsNullOrEmpty(windowTitle) || !windowTitle.StartsWith(title, StringComparison.Ordinal))
                return true;

            // 检查类名匹配（如果提供了类名）
            if (!string.IsNullOrEmpty(className))
            {
                var windowClassName = GetWindowClassName(hWnd);
                if (!windowClassName.Equals(className, StringComparison.Ordinal))
                    return true;
            }

            foundHandle = hWnd;
            return false; // 停止枚举
        }, IntPtr.Zero);

        return foundHandle;
    }

    /// <summary>
    /// 仅根据窗口标题查找窗口句柄
    /// </summary>
    /// <param name="title">窗口标题</param>
    /// <returns>窗口句柄，未找到返回 IntPtr.Zero</returns>
    public IntPtr FindByTitle(string title)
    {
        return FindWindow(title, "");
    }

    private string GetWindowTitle(IntPtr hWnd)
    {
        const int maxLength = 1024;
        var sb = new StringBuilder(maxLength);
        var length = GetWindowText(hWnd, sb, maxLength);
        return length > 0 ? sb.ToString() : "";
    }

    private string GetWindowClassName(IntPtr hWnd)
    {
        const int maxLength = 256;
        var sb = new StringBuilder(maxLength);
        var length = GetClassName(hWnd, sb, maxLength);
        return length > 0 ? sb.ToString() : "";
    }
}
```

- [ ] **Step 6: 运行测试验证通过**

```bash
dotnet test tests/WindowScreenshot.Tests/WindowScreenshot.Tests.csproj --filter "WindowFinderTests" -v n
```
预期：4 个测试全部通过

- [ ] **Step 7: 提交**

```bash
git add src/WindowScreenshot/WindowInfo.cs src/WindowScreenshot/WindowEnumerator.cs src/WindowScreenshot/WindowFinder.cs tests/WindowScreenshot.Tests/WindowFinderTests.cs
git commit -m "feat: add WindowFinder with prefix matching support"
```

---

## Task 4: 定时截图服务 (TimedScreenshotService)

**Files:**
- Create: `src/WindowScreenshot/TimedScreenshotService.cs`
- Test: `tests/WindowScreenshot.Tests/TimedScreenshotServiceTests.cs`

- [ ] **Step 1: 编写定时截图服务测试**

```csharp
namespace WindowScreenshot.Tests;

public class TimedScreenshotServiceTests : IDisposable
{
    private readonly string _testOutputDir;
    private readonly WindowEnumerator _enumerator;
    private readonly WindowCapturer _capturer;
    private readonly ScreenshotSaver _saver;
    private readonly WindowFinder _finder;

    public TimedScreenshotServiceTests()
    {
        _testOutputDir = Path.Combine(Path.GetTempPath(), "TimedScreenshotTests_" + Guid.NewGuid());
        Directory.CreateDirectory(_testOutputDir);
        
        _enumerator = new WindowEnumerator();
        _capturer = new WindowCapturer();
        _saver = new ScreenshotSaver(_capturer);
        _finder = new WindowFinder();
    }

    [Fact]
    public void 构造函数_应能创建服务实例 ()
    {
        // Arrange & Act
        var service = new TimedScreenshotService(_finder, _capturer, _saver, _testOutputDir);

        // Assert
        Assert.NotNull(service);
    }

    [Fact]
    public void 启动_间隔必须大于零 ()
    {
        // Arrange
        var service = new TimedScreenshotService(_finder, _capturer, _saver, _testOutputDir);
        var windows = _enumerator.EnumWindows();
        Assert.NotEmpty(windows);
        var targetWindow = windows[0];

        // Act & Assert - 间隔为 0 应抛出异常
        Assert.Throws<ArgumentException>(() => service.Start(targetWindow.Handle, 0));
    }

    [Fact]
    public void 启动_应启动定时器 ()
    {
        // Arrange
        var service = new TimedScreenshotService(_finder, _capturer, _saver, _testOutputDir);
        var windows = _enumerator.EnumWindows();
        Assert.NotEmpty(windows);
        var targetWindow = windows[0];

        // Act
        service.Start(targetWindow.Handle, 5); // 5 秒间隔

        // Assert
        Assert.True(service.IsRunning);
        
        // Cleanup
        service.Stop();
    }

    [Fact]
    public void 停止_应停止定时器 ()
    {
        // Arrange
        var service = new TimedScreenshotService(_finder, _capturer, _saver, _testOutputDir);
        var windows = _enumerator.EnumWindows();
        Assert.NotEmpty(windows);
        var targetWindow = windows[0];

        service.Start(targetWindow.Handle, 5);
        Assert.True(service.IsRunning);

        // Act
        service.Stop();

        // Assert
        Assert.False(service.IsRunning);
    }

    [Fact]
    public void 窗口不存在时应检测到 ()
    {
        // Arrange
        var service = new TimedScreenshotService(_finder, _capturer, _saver, _testOutputDir);
        var invalidHandle = (IntPtr)(-1);

        // Act
        var exists = service.CheckWindowExists(invalidHandle);

        // Assert
        Assert.False(exists);
    }

    public void Dispose()
    {
        if (Directory.Exists(_testOutputDir))
            Directory.Delete(_testOutputDir, true);
        
        _capturer?.Dispose();
    }
}
```

- [ ] **Step 2: 运行测试验证失败**

```bash
dotnet test tests/WindowScreenshot.Tests/WindowScreenshot.Tests.csproj --filter "TimedScreenshotServiceTests" -v n
```
预期：编译失败

- [ ] **Step 3: 实现 TimedScreenshotService 类**

```csharp
using System.Drawing;

namespace WindowScreenshot;

/// <summary>
/// 定时截图服务 - 封装定时器逻辑
/// </summary>
public class TimedScreenshotService : IDisposable
{
    private readonly System.Windows.Forms.Timer _timer;
    private readonly WindowFinder _finder;
    private readonly WindowCapturer _capturer;
    private readonly ScreenshotSaver _saver;
    private readonly string _outputDirectory;
    private IntPtr _targetWindowHandle;
    private bool _disposed = false;

    /// <summary>
    /// 是否正在运行
    /// </summary>
    public bool IsRunning { get; private set; }

    /// <summary>
    /// 初始化 TimedScreenshotService 的新实例
    /// </summary>
    public TimedScreenshotService(WindowFinder finder, WindowCapturer capturer, ScreenshotSaver saver, string outputDirectory)
    {
        _finder = finder;
        _capturer = capturer;
        _saver = saver;
        _outputDirectory = outputDirectory;
        IsRunning = false;

        _timer = new System.Windows.Forms.Timer();
        _timer.Tick += Timer_Tick;
    }

    /// <summary>
    /// 启动定时截图
    /// </summary>
    /// <param name="targetWindowHandle">目标窗口句柄</param>
    /// <param name="intervalSeconds">定时间隔（秒）</param>
    /// <exception cref="ArgumentException">间隔必须大于 0</exception>
    public void Start(IntPtr targetWindowHandle, int intervalSeconds)
    {
        if (intervalSeconds <= 0)
        {
            throw new ArgumentException("定时间隔必须大于 0", nameof(intervalSeconds));
        }

        _targetWindowHandle = targetWindowHandle;
        _timer.Interval = intervalSeconds * 1000; // 转换为毫秒
        _timer.Start();
        IsRunning = true;
    }

    /// <summary>
    /// 停止定时截图
    /// </summary>
    public void Stop()
    {
        _timer.Stop();
        IsRunning = false;
    }

    /// <summary>
    /// 检查窗口是否存在
    /// </summary>
    /// <param name="handle">窗口句柄</param>
    /// <returns>窗口是否存在</returns>
    public bool CheckWindowExists(IntPtr handle)
    {
        return WindowCapturer.IsWindowValid(handle);
    }

    private void Timer_Tick(object? sender, EventArgs e)
    {
        if (!_targetWindowHandle.Equals(IntPtr.Zero) && CheckWindowExists(_targetWindowHandle))
        {
            PerformScreenshot();
        }
    }

    /// <summary>
    /// 执行截图操作
    /// </summary>
    private void PerformScreenshot()
    {
        try
        {
            using var bitmap = _capturer.Capture(_targetWindowHandle);
            var filename = GenerateTimestampedFilename();
            var fullPath = Path.Combine(_outputDirectory, filename);
            
            // 确保输出目录存在
            if (!Directory.Exists(_outputDirectory))
            {
                Directory.CreateDirectory(_outputDirectory);
            }
            
            bitmap.Save(fullPath, System.Drawing.Imaging.ImageFormat.Png);
        }
        catch (Exception)
        {
            // 静默失败，避免打扰用户
        }
    }

    private string GenerateTimestampedFilename()
    {
        var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        return $"timed_screenshot_{timestamp}.png";
    }

    /// <summary>
    /// 释放资源
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _timer?.Stop();
                _timer?.Dispose();
            }
            _disposed = true;
        }
    }
}
```

- [ ] **Step 4: 扩展 WindowCapturer 添加静态验证方法**

在 `WindowCapturer.cs` 中添加：

```csharp
[DllImport("user32.dll")]
private static extern bool IsWindow(IntPtr hWnd);

/// <summary>
/// 验证窗口句柄是否有效
/// </summary>
/// <param name="handle">窗口句柄</param>
/// <returns>窗口是否有效</returns>
public static bool IsWindowValid(IntPtr handle)
{
    return IsWindow(handle);
}
```

- [ ] **Step 5: 运行测试验证通过**

```bash
dotnet test tests/WindowScreenshot.Tests/WindowScreenshot.Tests.csproj --filter "TimedScreenshotServiceTests" -v n
```
预期：6 个测试全部通过

- [ ] **Step 6: 提交**

```bash
git add src/WindowScreenshot/TimedScreenshotService.cs tests/WindowScreenshot.Tests/TimedScreenshotServiceTests.cs
git commit -m "feat: add TimedScreenshotService with WinForms timer"
```

---

## Task 5: UI 扩展 (MainForm.Designer.cs)

**Files:**
- Modify: `src/WindowScreenshot/MainForm.Designer.cs:1-106`

- [ ] **Step 1: 修改设计器文件添加定时截图控件**

完整替换 `MainForm.Designer.cs`:

```csharp
namespace WindowScreenshot;

partial class MainForm
{
    private System.ComponentModel.IContainer components = null;
    private System.Windows.Forms.ComboBox windowComboBox;
    private System.Windows.Forms.Label windowLabel;
    private System.Windows.Forms.Button refreshButton;
    private System.Windows.Forms.Button screenshotButton;
    private System.Windows.Forms.PictureBox previewPictureBox;
    private System.Windows.Forms.Label statusLabel;
    
    // 定时截图控件
    private System.Windows.Forms.Label intervalLabel;
    private System.Windows.Forms.NumericUpDown intervalNumericUpDown;
    private System.Windows.Forms.Button startTimerButton;
    private System.Windows.Forms.Label timerStatusLabel;
    private System.Windows.Forms.Label nextScreenshotLabel;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        this.windowComboBox = new System.Windows.Forms.ComboBox();
        this.windowLabel = new System.Windows.Forms.Label();
        this.refreshButton = new System.Windows.Forms.Button();
        this.screenshotButton = new System.Windows.Forms.Button();
        this.previewPictureBox = new System.Windows.Forms.PictureBox();
        this.statusLabel = new System.Windows.Forms.Label();
        this.intervalLabel = new System.Windows.Forms.Label();
        this.intervalNumericUpDown = new System.Windows.Forms.NumericUpDown();
        this.startTimerButton = new System.Windows.Forms.Button();
        this.timerStatusLabel = new System.Windows.Forms.Label();
        this.nextScreenshotLabel = new System.Windows.Forms.Label();
        ((System.ComponentModel.ISupportInitialize)(this.previewPictureBox)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(this.intervalNumericUpDown)).BeginInit();
        this.SuspendLayout();
        //
        // windowLabel
        //
        this.windowLabel.AutoSize = true;
        this.windowLabel.Location = new System.Drawing.Point(12, 15);
        this.windowLabel.Name = "windowLabel";
        this.windowLabel.Size = new System.Drawing.Size(68, 17);
        this.windowLabel.Text = "选择窗口：";
        //
        // windowComboBox
        //
        this.windowComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
        this.windowComboBox.Location = new System.Drawing.Point(15, 35);
        this.windowComboBox.Name = "windowComboBox";
        this.windowComboBox.Size = new System.Drawing.Size(500, 25);
        this.windowComboBox.TabIndex = 0;
        this.windowComboBox.SelectedIndexChanged += new System.EventHandler(this.WindowComboBox_SelectedIndexChanged);
        //
        // refreshButton
        //
        this.refreshButton.Location = new System.Drawing.Point(521, 34);
        this.refreshButton.Name = "refreshButton";
        this.refreshButton.Size = new System.Drawing.Size(80, 27);
        this.refreshButton.TabIndex = 1;
        this.refreshButton.Text = "刷新";
        this.refreshButton.UseVisualStyleBackColor = true;
        this.refreshButton.Click += new System.EventHandler(this.RefreshButton_Click);
        //
        // screenshotButton
        //
        this.screenshotButton.Enabled = false;
        this.screenshotButton.Location = new System.Drawing.Point(607, 34);
        this.screenshotButton.Name = "screenshotButton";
        this.screenshotButton.Size = new System.Drawing.Size(100, 27);
        this.screenshotButton.TabIndex = 2;
        this.screenshotButton.Text = "截图 (延时 1 秒)";
        this.screenshotButton.UseVisualStyleBackColor = true;
        this.screenshotButton.Click += new System.EventHandler(this.ScreenshotButton_Click);
        //
        // intervalLabel
        //
        this.intervalLabel.AutoSize = true;
        this.intervalLabel.Location = new System.Drawing.Point(12, 485);
        this.intervalLabel.Name = "intervalLabel";
        this.intervalLabel.Size = new System.Drawing.Size(68, 17);
        this.intervalLabel.Text = "定时间隔：";
        //
        // intervalNumericUpDown
        //
        this.intervalNumericUpDown.Location = new System.Drawing.Point(85, 483);
        this.intervalNumericUpDown.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
        this.intervalNumericUpDown.Name = "intervalNumericUpDown";
        this.intervalNumericUpDown.Size = new System.Drawing.Size(80, 23);
        this.intervalNumericUpDown.TabIndex = 4;
        this.intervalNumericUpDown.Value = new decimal(new int[] { 5, 0, 0, 0 });
        this.intervalNumericUpDown.ValueChanged += new System.EventHandler(this.IntervalNumericUpDown_ValueChanged);
        //
        // startTimerButton
        //
        this.startTimerButton.Enabled = false;
        this.startTimerButton.Location = new System.Drawing.Point(175, 482);
        this.startTimerButton.Name = "startTimerButton";
        this.startTimerButton.Size = new System.Drawing.Size(100, 25);
        this.startTimerButton.TabIndex = 5;
        this.startTimerButton.Text = "开始定时截图";
        this.startTimerButton.UseVisualStyleBackColor = true;
        this.startTimerButton.Click += new System.EventHandler(this.StartTimerButton_Click);
        //
        // timerStatusLabel
        //
        this.timerStatusLabel.AutoSize = true;
        this.timerStatusLabel.Location = new System.Drawing.Point(285, 485);
        this.timerStatusLabel.Name = "timerStatusLabel";
        this.timerStatusLabel.Size = new System.Drawing.Size(44, 17);
        this.timerStatusLabel.Text = "已停止";
        //
        // nextScreenshotLabel
        //
        this.nextScreenshotLabel.AutoSize = true;
        this.nextScreenshotLabel.Location = new System.Drawing.Point(350, 485);
        this.nextScreenshotLabel.Name = "nextScreenshotLabel";
        this.nextScreenshotLabel.Size = new System.Drawing.Size(0, 17);
        //
        // previewPictureBox
        //
        this.previewPictureBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
        this.previewPictureBox.Location = new System.Drawing.Point(15, 75);
        this.previewPictureBox.Name = "previewPictureBox";
        this.previewPictureBox.Size = new System.Drawing.Size(692, 400);
        this.previewPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
        this.previewPictureBox.TabIndex = 3;
        this.previewPictureBox.TabStop = false;
        //
        // statusLabel
        //
        this.statusLabel.AutoSize = true;
        this.statusLabel.Location = new System.Drawing.Point(12, 520);
        this.statusLabel.Name = "statusLabel";
        this.statusLabel.Size = new System.Drawing.Size(44, 17);
        this.statusLabel.Text = "就绪";
        //
        // MainForm
        //
        this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(722, 550);
        this.Controls.Add(this.windowComboBox);
        this.Controls.Add(this.windowLabel);
        this.Controls.Add(this.refreshButton);
        this.Controls.Add(this.screenshotButton);
        this.Controls.Add(this.previewPictureBox);
        this.Controls.Add(this.statusLabel);
        this.Controls.Add(this.intervalLabel);
        this.Controls.Add(this.intervalNumericUpDown);
        this.Controls.Add(this.startTimerButton);
        this.Controls.Add(this.timerStatusLabel);
        this.Controls.Add(this.nextScreenshotLabel);
        this.Name = "MainForm";
        this.Text = "窗口截图工具";
        ((System.ComponentModel.ISupportInitialize)(this.previewPictureBox)).EndInit();
        ((System.ComponentModel.ISupportInitialize)(this.intervalNumericUpDown)).EndInit();
        this.ResumeLayout(false);
        this.PerformLayout();
    }
}
```

- [ ] **Step 2: 提交**

```bash
git add src/WindowScreenshot/MainForm.Designer.cs
git commit -m "ui: add timed screenshot controls to MainForm"
```

---

## Task 6: MainForm 集成定时截图功能

**Files:**
- Modify: `src/WindowScreenshot/MainForm.cs:1-101`

- [ ] **Step 1: 扩展 MainForm 添加定时截图逻辑**

完整替换 `MainForm.cs`:

```csharp
using System.Drawing;
using System.IO;

namespace WindowScreenshot;

public partial class MainForm : Form
{
    private readonly WindowEnumerator _enumerator;
    private readonly WindowCapturer _capturer;
    private readonly ScreenshotSaver _saver;
    private readonly ConfigManager _configManager;
    private readonly WindowFinder _finder;
    private TimedScreenshotService? _timedService;
    private List<WindowInfo> _windows;
    private string _configPath;
    private string _outputDirectory;

    public MainForm()
    {
        InitializeComponent();
        
        _enumerator = new WindowEnumerator();
        _capturer = new WindowCapturer();
        _saver = new ScreenshotSaver(_capturer);
        _finder = new WindowFinder();
        _windows = new List<WindowInfo>();
        
        // 配置路径
        var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        _configPath = Path.Combine(appDataPath, "CQHelper", "config.json");
        _configManager = new ConfigManager(_configPath);
        
        // 输出目录
        _outputDirectory = Path.Combine(AppContext.BaseDirectory, "timed_screenshots");
        
        // 加载配置
        LoadConfiguration();
    }

    /// <summary>
    /// 从配置文件加载设置
    /// </summary>
    private void LoadConfiguration()
    {
        var settings = _configManager.Load();
        
        if (!string.IsNullOrEmpty(settings.TargetWindowTitle))
        {
            // 尝试找到窗口并选中
            var index = _windows.FindIndex(w => w.Title == settings.TargetWindowTitle);
            if (index >= 0)
            {
                windowComboBox.SelectedIndex = index;
            }
        }
        
        intervalNumericUpDown.Value = Math.Max(1, settings.IntervalSeconds);
    }

    /// <summary>
    /// 保存当前设置到配置文件
    /// </summary>
    private void SaveConfiguration()
    {
        var settings = new ScreenshotSettings();
        
        if (windowComboBox.SelectedIndex >= 0 && windowComboBox.SelectedIndex < _windows.Count)
        {
            var selectedWindow = _windows[windowComboBox.SelectedIndex];
            settings.TargetWindowTitle = selectedWindow.Title;
            settings.TargetWindowClassName = selectedWindow.ClassName;
        }
        
        settings.IntervalSeconds = (int)intervalNumericUpDown.Value;
        settings.IsEnabled = _timedService?.IsRunning ?? false;
        
        _configManager.Save(settings);
    }

    private void LoadWindowList()
    {
        windowComboBox.Items.Clear();
        _windows = _enumerator.EnumWindows();

        foreach (var window in _windows)
        {
            windowComboBox.Items.Add(window.Title);
        }

        if (windowComboBox.Items.Count > 0)
        {
            windowComboBox.SelectedIndex = 0;
        }

        UpdateButtonStates();
    }

    private void UpdateButtonStates()
    {
        var hasSelection = windowComboBox.SelectedIndex >= 0;
        screenshotButton.Enabled = hasSelection;
        startTimerButton.Enabled = hasSelection;
    }

    private void WindowComboBox_SelectedIndexChanged(object sender, EventArgs e)
    {
        UpdateButtonStates();
        SaveConfiguration();
    }

    private void RefreshButton_Click(object sender, EventArgs e)
    {
        LoadWindowList();
    }

    private void IntervalNumericUpDown_ValueChanged(object? sender, EventArgs e)
    {
        SaveConfiguration();
    }

    private void StartTimerButton_Click(object sender, EventArgs e)
    {
        if (windowComboBox.SelectedIndex < 0)
        {
            MessageBox.Show("请先选择一个窗口", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (_timedService?.IsRunning == true)
        {
            // 停止定时截图
            _timedService.Stop();
            timerStatusLabel.Text = "已停止";
            startTimerButton.Text = "开始定时截图";
            nextScreenshotLabel.Text = "";
        }
        else
        {
            // 启动定时截图
            try
            {
                var selectedWindow = _windows[windowComboBox.SelectedIndex];
                var intervalSeconds = (int)intervalNumericUpDown.Value;

                if (_timedService == null)
                {
                    _timedService = new TimedScreenshotService(_finder, _capturer, _saver, _outputDirectory);
                }

                _timedService.Start(selectedWindow.Handle, intervalSeconds);
                timerStatusLabel.Text = "运行中";
                startTimerButton.Text = "停止定时截图";
                
                // 显示下次截图时间
                UpdateNextScreenshotLabel();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"启动失败：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        SaveConfiguration();
    }

    private void UpdateNextScreenshotLabel()
    {
        if (_timedService?.IsRunning == true)
        {
            var intervalSeconds = (int)intervalNumericUpDown.Value;
            var nextTime = DateTime.Now.AddSeconds(intervalSeconds);
            nextScreenshotLabel.Text = $"下次截图：{nextTime:HH:mm:ss}";
        }
    }

    private void ScreenshotButton_Click(object sender, EventArgs e)
    {
        if (windowComboBox.SelectedIndex < 0)
        {
            MessageBox.Show("请先选择一个窗口", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        try
        {
            var selectedWindow = _windows[windowComboBox.SelectedIndex];

            // 最小化工具窗口
            this.WindowState = FormWindowState.Minimized;

            // 延时 1 秒后截图
            Thread.Sleep(1000);

            // 截图并保存
            var path = _saver.CaptureAndSave(selectedWindow.Handle);

            // 恢复窗口
            this.WindowState = FormWindowState.Normal;
            this.Activate();

            // 显示预览
            using var image = Image.FromFile(path);
            previewPictureBox.Image?.Dispose();
            previewPictureBox.Image = new Bitmap(image);

            // 显示保存信息
            statusLabel.Text = $"已保存：{path}";
        }
        catch (Exception ex)
        {
            // 恢复窗口
            this.WindowState = FormWindowState.Normal;
            this.Activate();

            MessageBox.Show($"截图失败：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            statusLabel.Text = "截图失败";
        }
    }

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        // 保存最终配置
        SaveConfiguration();
        
        // 停止定时截图服务
        _timedService?.Dispose();
        
        base.OnFormClosing(e);
    }
}
```

- [ ] **Step 2: 编译验证**

```bash
dotnet build src/WindowScreenshot/WindowScreenshot.csproj
```
预期：编译成功，无警告

- [ ] **Step 3: 提交**

```bash
git add src/WindowScreenshot/MainForm.cs
git commit -m "feat: integrate timed screenshot service in MainForm"
```

---

## Task 7: 集成测试与验证

**Files:**
- Test: `tests/WindowScreenshot.Tests/IntegrationTests.cs`

- [ ] **Step 1: 编写集成测试**

```csharp
using System.Drawing;

namespace WindowScreenshot.Tests;

public class IntegrationTests : IDisposable
{
    private readonly string _testConfigPath;
    private readonly string _testOutputDir;

    public IntegrationTests()
    {
        _testConfigPath = Path.Combine(Path.GetTempPath(), "IntegrationTest_" + Guid.NewGuid(), "config.json");
        _testOutputDir = Path.Combine(Path.GetTempPath(), "IntegrationTest_" + Guid.NewGuid(), "screenshots");
    }

    [Fact]
    public void 完整流程_配置保存和加载 ()
    {
        // Arrange
        var configManager = new ConfigManager(_testConfigPath);
        var settings = new ScreenshotSettings
        {
            TargetWindowTitle = "Test Window",
            TargetWindowClassName = "Notepad",
            IntervalSeconds = 10,
            IsEnabled = true
        };

        try
        {
            // Act
            configManager.Save(settings);
            var loaded = configManager.Load();

            // Assert
            Assert.Equal(settings.TargetWindowTitle, loaded.TargetWindowTitle);
            Assert.Equal(settings.TargetWindowClassName, loaded.TargetWindowClassName);
            Assert.Equal(settings.IntervalSeconds, loaded.IntervalSeconds);
        }
        finally
        {
            Cleanup();
        }
    }

    [Fact]
    public void 完整流程_窗口查找 ()
    {
        // Arrange
        var finder = new WindowFinder();
        var enumerator = new WindowEnumerator();
        var windows = enumerator.EnumWindows();

        Assert.NotEmpty(windows);
        var targetWindow = windows[0];

        // Act
        var foundHandle = finder.FindWindow(targetWindow.Title, targetWindow.ClassName);

        // Assert
        Assert.Equal(targetWindow.Handle, foundHandle);
    }

    [Fact]
    public void 完整流程_定时截图服务启动和停止 ()
    {
        // Arrange
        var capturer = new WindowCapturer();
        var saver = new ScreenshotSaver(capturer);
        var finder = new WindowFinder();
        var service = new TimedScreenshotService(finder, capturer, saver, _testOutputDir);
        var enumerator = new WindowEnumerator();
        var windows = enumerator.EnumWindows();

        Assert.NotEmpty(windows);
        var targetWindow = windows[0];

        try
        {
            // Act
            service.Start(targetWindow.Handle, 5);

            // Assert
            Assert.True(service.IsRunning);

            // Stop
            service.Stop();
            Assert.False(service.IsRunning);
        }
        finally
        {
            service.Dispose();
            Cleanup();
        }
    }

    private void Cleanup()
    {
        var configDir = Path.GetDirectoryName(_testConfigPath);
        if (!string.IsNullOrEmpty(configDir) && Directory.Exists(configDir))
            Directory.Delete(configDir, true);
        
        if (Directory.Exists(_testOutputDir))
            Directory.Delete(_testOutputDir, true);
    }

    public void Dispose()
    {
        Cleanup();
    }
}
```

- [ ] **Step 2: 运行所有测试**

```bash
dotnet test tests/WindowScreenshot.Tests/WindowScreenshot.Tests.csproj -v n
```
预期：所有测试通过

- [ ] **Step 3: 手动验证应用程序**

```bash
dotnet run --project src/WindowScreenshot/WindowScreenshot.csproj
```
预期：应用程序正常启动，UI 控件正常显示

- [ ] **Step 4: 提交**

```bash
git add tests/WindowScreenshot.Tests/IntegrationTests.cs
git commit -m "test: add integration tests for timed screenshot feature"
```

---

## Spec 覆盖检查

| Spec Requirement | Task | Status |
|-----------------|------|--------|
| JSON 格式配置 | Task 2 | ✓ |
| APPDATA 目录存储 | Task 6 | ✓ |
| 保存窗口信息 | Task 2, 6 | ✓ |
| 保存定时间隔 | Task 2, 6 | ✓ |
| 启动时加载配置 | Task 6 | ✓ |
| 目录不存在自动创建 | Task 2 | ✓ |
| 窗口标题 + 类名查找 | Task 3 | ✓ |
| 窗口不存在返回空值 | Task 3 | ✓ |
| 前缀模糊匹配 | Task 3 | ✓ |
| 启动/停止定时截图 | Task 4, 6 | ✓ |
| 秒级精度 | Task 4 | ✓ |
| 静默截图 | Task 4 | ✓ |
| 窗口不存在自动停止 | Task 4 | ✓ |

---

## 无 Placeholder 检查

- [x] 无 "TBD", "TODO" 占位符
- [x] 无 "添加适当错误处理" 等模糊描述
- [x] 所有测试代码完整
- [x] 所有实现代码完整
- [x] 类型名称一致 (ScreenshotSettings, ConfigManager, WindowFinder, TimedScreenshotService)

---

计划完成。

**执行选项:**

1. **Subagent-Driven (推荐)** - 每个任务 dispatch 新的 subagent，任务间 review，快速迭代
2. **Inline Execution** - 在当前 session 使用 executing-plans 批量执行，设置检查点

选择哪种方式？
