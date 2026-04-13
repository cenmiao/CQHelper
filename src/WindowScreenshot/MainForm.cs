using System.IO;

namespace WindowScreenshot;

public partial class MainForm : Form
{
    private readonly WindowEnumerator _enumerator;
    private readonly WindowCapturer _capturer;
    private readonly ScreenshotSaver _saver;
    private readonly WindowFinder _finder;
    private readonly ConfigManager _configManager;
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

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        LoadWindowList();
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

        UpdateScreenshotButtonState();
    }

    private void UpdateScreenshotButtonState()
    {
        var hasSelection = windowComboBox.SelectedIndex >= 0;
        screenshotButton.Enabled = hasSelection;
        startTimerButton.Enabled = hasSelection;
    }

    private void WindowComboBox_SelectedIndexChanged(object sender, EventArgs e)
    {
        UpdateScreenshotButtonState();
        SaveConfiguration();
    }

    private void RefreshButton_Click(object sender, EventArgs e)
    {
        LoadWindowList();
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

    private void IntervalNumericUpDown_ValueChanged(object? sender, EventArgs e)
    {
        SaveConfiguration();
    }

    private void StartTimerButton_Click(object? sender, EventArgs e)
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

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        // 保存最终配置
        SaveConfiguration();

        // 停止定时截图服务
        _timedService?.Dispose();

        base.OnFormClosing(e);
    }
}
