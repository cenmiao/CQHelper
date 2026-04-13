namespace WindowScreenshot;

public partial class MainForm : Form
{
    private readonly WindowEnumerator _enumerator;
    private readonly WindowCapturer _capturer;
    private readonly ScreenshotSaver _saver;
    private List<WindowInfo> _windows;

    // 定时截图相关
    private System.Windows.Forms.Timer? _screenshotTimer;
    private DateTime _nextScreenshotTime;

    public MainForm()
    {
        InitializeComponent();
        _enumerator = new WindowEnumerator();
        _capturer = new WindowCapturer();
        _saver = new ScreenshotSaver(_capturer);
        _windows = new List<WindowInfo>();
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
        screenshotButton.Enabled = windowComboBox.SelectedIndex >= 0;
    }

    private void WindowComboBox_SelectedIndexChanged(object sender, EventArgs e)
    {
        UpdateScreenshotButtonState();
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
        // 间隔值改变时的处理（可用于实时更新）
    }

    private void StartTimerButton_Click(object? sender, EventArgs e)
    {
        if (_screenshotTimer == null)
        {
            // 启动定时器
            var interval = (int)intervalNumericUpDown.Value * 1000; // 转换为毫秒
            _screenshotTimer = new System.Windows.Forms.Timer();
            _screenshotTimer.Interval = interval;
            _screenshotTimer.Tick += ScreenshotTimer_Tick;
            _screenshotTimer.Start();

            startTimerButton.Text = "停止定时截图";
            timerStatusLabel.Text = "运行中";
            UpdateNextScreenshotLabel();
        }
        else
        {
            // 停止定时器
            _screenshotTimer.Stop();
            _screenshotTimer.Dispose();
            _screenshotTimer = null;

            startTimerButton.Text = "开始定时截图";
            timerStatusLabel.Text = "已停止";
            nextScreenshotLabel.Text = "";
        }
    }

    private void ScreenshotTimer_Tick(object? sender, EventArgs e)
    {
        // 定时执行截图
        if (windowComboBox.SelectedIndex >= 0)
        {
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
                UpdateNextScreenshotLabel();
            }
            catch (Exception ex)
            {
                this.WindowState = FormWindowState.Normal;
                this.Activate();
                statusLabel.Text = $"定时截图失败：{ex.Message}";
            }
        }
    }

    private void UpdateNextScreenshotLabel()
    {
        if (_screenshotTimer != null)
        {
            _nextScreenshotTime = DateTime.Now.AddSeconds((double)intervalNumericUpDown.Value);
            nextScreenshotLabel.Text = $"下次截图：{_nextScreenshotTime:HH:mm:ss}";
        }
    }
}
