namespace WindowScreenshot;

partial class MainForm
{
    private System.ComponentModel.IContainer components = null;

    // Tab 控件
    private System.Windows.Forms.TabControl mainTabControl;
    private System.Windows.Forms.TabPage basicSettingsTabPage;
    private System.Windows.Forms.TabPage gameAssistantTabPage;

    // 基础设置 Tab 控件
    private System.Windows.Forms.ComboBox windowComboBox;
    private System.Windows.Forms.Label windowLabel;
    private System.Windows.Forms.Button refreshButton;
    private System.Windows.Forms.Button screenshotButton;
    private System.Windows.Forms.PictureBox previewPictureBox;
    private System.Windows.Forms.Label intervalLabel;
    private System.Windows.Forms.NumericUpDown intervalNumericUpDown;
    private System.Windows.Forms.Button startTimerButton;
    private System.Windows.Forms.Label timerStatusLabel;
    private System.Windows.Forms.Label nextScreenshotLabel;

    // 游戏辅助 Tab 控件
    private System.Windows.Forms.Label hpLabel;
    private System.Windows.Forms.Label hpValueLabel;
    private System.Windows.Forms.Label mpLabel;
    private System.Windows.Forms.Label mpValueLabel;
    private System.Windows.Forms.Label levelLabel;
    private System.Windows.Forms.Label levelValueLabel;
    private System.Windows.Forms.TextBox logTextBox;
    private System.Windows.Forms.Button clearLogButton;
    private System.Windows.Forms.Label gameAnalysisStatusLabel;

    // 状态栏
    private System.Windows.Forms.Label statusLabel;

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
        // Tab 控件
        this.mainTabControl = new System.Windows.Forms.TabControl();
        this.basicSettingsTabPage = new System.Windows.Forms.TabPage();
        this.gameAssistantTabPage = new System.Windows.Forms.TabPage();

        // 基础设置控件
        this.windowComboBox = new System.Windows.Forms.ComboBox();
        this.windowLabel = new System.Windows.Forms.Label();
        this.refreshButton = new System.Windows.Forms.Button();
        this.screenshotButton = new System.Windows.Forms.Button();
        this.previewPictureBox = new System.Windows.Forms.PictureBox();
        this.intervalLabel = new System.Windows.Forms.Label();
        this.intervalNumericUpDown = new System.Windows.Forms.NumericUpDown();
        this.startTimerButton = new System.Windows.Forms.Button();
        this.timerStatusLabel = new System.Windows.Forms.Label();
        this.nextScreenshotLabel = new System.Windows.Forms.Label();

        // 游戏辅助控件
        this.hpLabel = new System.Windows.Forms.Label();
        this.hpValueLabel = new System.Windows.Forms.Label();
        this.mpLabel = new System.Windows.Forms.Label();
        this.mpValueLabel = new System.Windows.Forms.Label();
        this.levelLabel = new System.Windows.Forms.Label();
        this.levelValueLabel = new System.Windows.Forms.Label();
        this.logTextBox = new System.Windows.Forms.TextBox();
        this.clearLogButton = new System.Windows.Forms.Button();
        this.gameAnalysisStatusLabel = new System.Windows.Forms.Label();

        // 状态栏
        this.statusLabel = new System.Windows.Forms.Label();

        // mainTabControl
        this.mainTabControl.Controls.Add(this.basicSettingsTabPage);
        this.mainTabControl.Controls.Add(this.gameAssistantTabPage);
        this.mainTabControl.Dock = System.Windows.Forms.DockStyle.Fill;
        this.mainTabControl.Location = new System.Drawing.Point(0, 0);
        this.mainTabControl.Name = "mainTabControl";
        this.mainTabControl.SelectedIndex = 0;
        this.mainTabControl.Size = new System.Drawing.Size(722, 520);
        this.mainTabControl.TabIndex = 0;

        // basicSettingsTabPage
        this.basicSettingsTabPage.Controls.Add(this.windowComboBox);
        this.basicSettingsTabPage.Controls.Add(this.windowLabel);
        this.basicSettingsTabPage.Controls.Add(this.refreshButton);
        this.basicSettingsTabPage.Controls.Add(this.screenshotButton);
        this.basicSettingsTabPage.Controls.Add(this.previewPictureBox);
        this.basicSettingsTabPage.Controls.Add(this.intervalLabel);
        this.basicSettingsTabPage.Controls.Add(this.intervalNumericUpDown);
        this.basicSettingsTabPage.Controls.Add(this.startTimerButton);
        this.basicSettingsTabPage.Controls.Add(this.timerStatusLabel);
        this.basicSettingsTabPage.Controls.Add(this.nextScreenshotLabel);
        this.basicSettingsTabPage.Location = new System.Drawing.Point(4, 25);
        this.basicSettingsTabPage.Name = "basicSettingsTabPage";
        this.basicSettingsTabPage.Padding = new System.Windows.Forms.Padding(10);
        this.basicSettingsTabPage.Size = new System.Drawing.Size(714, 491);
        this.basicSettingsTabPage.TabIndex = 0;
        this.basicSettingsTabPage.Text = "基础设置";
        this.basicSettingsTabPage.UseVisualStyleBackColor = true;

        // gameAssistantTabPage
        this.gameAssistantTabPage.Controls.Add(this.hpLabel);
        this.gameAssistantTabPage.Controls.Add(this.hpValueLabel);
        this.gameAssistantTabPage.Controls.Add(this.mpLabel);
        this.gameAssistantTabPage.Controls.Add(this.mpValueLabel);
        this.gameAssistantTabPage.Controls.Add(this.levelLabel);
        this.gameAssistantTabPage.Controls.Add(this.levelValueLabel);
        this.gameAssistantTabPage.Controls.Add(this.logTextBox);
        this.gameAssistantTabPage.Controls.Add(this.clearLogButton);
        this.gameAssistantTabPage.Controls.Add(this.gameAnalysisStatusLabel);
        this.gameAssistantTabPage.Location = new System.Drawing.Point(4, 25);
        this.gameAssistantTabPage.Name = "gameAssistantTabPage";
        this.gameAssistantTabPage.Padding = new System.Windows.Forms.Padding(10);
        this.gameAssistantTabPage.Size = new System.Drawing.Size(714, 491);
        this.gameAssistantTabPage.TabIndex = 1;
        this.gameAssistantTabPage.Text = "游戏辅助";
        this.gameAssistantTabPage.UseVisualStyleBackColor = true;

        // windowLabel
        this.windowLabel.AutoSize = true;
        this.windowLabel.Location = new System.Drawing.Point(10, 15);
        this.windowLabel.Name = "windowLabel";
        this.windowLabel.Size = new System.Drawing.Size(68, 17);
        this.windowLabel.Text = "选择窗口：";

        // windowComboBox
        this.windowComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
        this.windowComboBox.Location = new System.Drawing.Point(13, 35);
        this.windowComboBox.Name = "windowComboBox";
        this.windowComboBox.Size = new System.Drawing.Size(500, 25);
        this.windowComboBox.TabIndex = 0;
        this.windowComboBox.SelectedIndexChanged += new System.EventHandler(this.WindowComboBox_SelectedIndexChanged);

        // refreshButton
        this.refreshButton.Location = new System.Drawing.Point(519, 34);
        this.refreshButton.Name = "refreshButton";
        this.refreshButton.Size = new System.Drawing.Size(80, 27);
        this.refreshButton.TabIndex = 1;
        this.refreshButton.Text = "刷新";
        this.refreshButton.UseVisualStyleBackColor = true;
        this.refreshButton.Click += new System.EventHandler(this.RefreshButton_Click);

        // screenshotButton
        this.screenshotButton.Enabled = false;
        this.screenshotButton.Location = new System.Drawing.Point(605, 34);
        this.screenshotButton.Name = "screenshotButton";
        this.screenshotButton.Size = new System.Drawing.Size(100, 27);
        this.screenshotButton.TabIndex = 2;
        this.screenshotButton.Text = "截图 (延时 1 秒)";
        this.screenshotButton.UseVisualStyleBackColor = true;
        this.screenshotButton.Click += new System.EventHandler(this.ScreenshotButton_Click);

        // previewPictureBox
        this.previewPictureBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
        this.previewPictureBox.Location = new System.Drawing.Point(13, 75);
        this.previewPictureBox.Name = "previewPictureBox";
        this.previewPictureBox.Size = new System.Drawing.Size(688, 350);
        this.previewPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
        this.previewPictureBox.TabIndex = 3;
        this.previewPictureBox.TabStop = false;

        // intervalLabel
        this.intervalLabel.AutoSize = true;
        this.intervalLabel.Location = new System.Drawing.Point(10, 435);
        this.intervalLabel.Name = "intervalLabel";
        this.intervalLabel.Size = new System.Drawing.Size(68, 17);
        this.intervalLabel.Text = "定时间隔：";

        // intervalNumericUpDown
        this.intervalNumericUpDown.Location = new System.Drawing.Point(83, 433);
        this.intervalNumericUpDown.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
        this.intervalNumericUpDown.Name = "intervalNumericUpDown";
        this.intervalNumericUpDown.Size = new System.Drawing.Size(80, 23);
        this.intervalNumericUpDown.TabIndex = 4;
        this.intervalNumericUpDown.Value = new decimal(new int[] { 5, 0, 0, 0 });
        this.intervalNumericUpDown.ValueChanged += new System.EventHandler(this.IntervalNumericUpDown_ValueChanged);

        // startTimerButton
        this.startTimerButton.Enabled = false;
        this.startTimerButton.Location = new System.Drawing.Point(173, 432);
        this.startTimerButton.Name = "startTimerButton";
        this.startTimerButton.Size = new System.Drawing.Size(100, 25);
        this.startTimerButton.TabIndex = 5;
        this.startTimerButton.Text = "开始定时截图";
        this.startTimerButton.UseVisualStyleBackColor = true;
        this.startTimerButton.Click += new System.EventHandler(this.StartTimerButton_Click);

        // timerStatusLabel
        this.timerStatusLabel.AutoSize = true;
        this.timerStatusLabel.Location = new System.Drawing.Point(283, 435);
        this.timerStatusLabel.Name = "timerStatusLabel";
        this.timerStatusLabel.Size = new System.Drawing.Size(44, 17);
        this.timerStatusLabel.Text = "已停止";

        // nextScreenshotLabel
        this.nextScreenshotLabel.AutoSize = true;
        this.nextScreenshotLabel.Location = new System.Drawing.Point(348, 435);
        this.nextScreenshotLabel.Name = "nextScreenshotLabel";
        this.nextScreenshotLabel.Size = new System.Drawing.Size(0, 17);

        // hpLabel
        this.hpLabel.AutoSize = true;
        this.hpLabel.Location = new System.Drawing.Point(20, 20);
        this.hpLabel.Name = "hpLabel";
        this.hpLabel.Size = new System.Drawing.Size(40, 17);
        this.hpLabel.Text = "血量：";

        // hpValueLabel
        this.hpValueLabel.AutoSize = true;
        this.hpValueLabel.Font = new System.Drawing.Font("Microsoft YaHei", 12F, System.Drawing.FontStyle.Bold);
        this.hpValueLabel.ForeColor = System.Drawing.Color.Red;
        this.hpValueLabel.Location = new System.Drawing.Point(80, 20);
        this.hpValueLabel.Name = "hpValueLabel";
        this.hpValueLabel.Size = new System.Drawing.Size(50, 22);
        this.hpValueLabel.Text = "-/-";

        // mpLabel
        this.mpLabel.AutoSize = true;
        this.mpLabel.Location = new System.Drawing.Point(200, 20);
        this.mpLabel.Name = "mpLabel";
        this.mpLabel.Size = new System.Drawing.Size(40, 17);
        this.mpLabel.Text = "蓝量：";

        // mpValueLabel
        this.mpValueLabel.AutoSize = true;
        this.mpValueLabel.Font = new System.Drawing.Font("Microsoft YaHei", 12F, System.Drawing.FontStyle.Bold);
        this.mpValueLabel.ForeColor = System.Drawing.Color.Blue;
        this.mpValueLabel.Location = new System.Drawing.Point(260, 20);
        this.mpValueLabel.Name = "mpValueLabel";
        this.mpValueLabel.Size = new System.Drawing.Size(50, 22);
        this.mpValueLabel.Text = "-/-";

        // levelLabel
        this.levelLabel.AutoSize = true;
        this.levelLabel.Location = new System.Drawing.Point(400, 20);
        this.levelLabel.Name = "levelLabel";
        this.levelLabel.Size = new System.Drawing.Size(40, 17);
        this.levelLabel.Text = "等级：";

        // levelValueLabel
        this.levelValueLabel.AutoSize = true;
        this.levelValueLabel.Font = new System.Drawing.Font("Microsoft YaHei", 12F, System.Drawing.FontStyle.Bold);
        this.levelValueLabel.ForeColor = System.Drawing.Color.Green;
        this.levelValueLabel.Location = new System.Drawing.Point(460, 20);
        this.levelValueLabel.Name = "levelValueLabel";
        this.levelValueLabel.Size = new System.Drawing.Size(25, 22);
        this.levelValueLabel.Text = "-";

        // logTextBox
        this.logTextBox.Location = new System.Drawing.Point(20, 60);
        this.logTextBox.Multiline = true;
        this.logTextBox.Name = "logTextBox";
        this.logTextBox.ReadOnly = true;
        this.logTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
        this.logTextBox.Size = new System.Drawing.Size(674, 380);
        this.logTextBox.TabIndex = 0;

        // clearLogButton
        this.clearLogButton.Location = new System.Drawing.Point(20, 450);
        this.clearLogButton.Name = "clearLogButton";
        this.clearLogButton.Size = new System.Drawing.Size(100, 27);
        this.clearLogButton.TabIndex = 1;
        this.clearLogButton.Text = "清空日志";
        this.clearLogButton.UseVisualStyleBackColor = true;
        this.clearLogButton.Click += new System.EventHandler(this.ClearLogButton_Click);

        // gameAnalysisStatusLabel
        this.gameAnalysisStatusLabel.AutoSize = true;
        this.gameAnalysisStatusLabel.Location = new System.Drawing.Point(140, 455);
        this.gameAnalysisStatusLabel.Name = "gameAnalysisStatusLabel";
        this.gameAnalysisStatusLabel.Size = new System.Drawing.Size(100, 17);
        this.gameAnalysisStatusLabel.Text = "分析状态：未启用";

        // statusLabel
        this.statusLabel.AutoSize = true;
        this.statusLabel.Dock = System.Windows.Forms.DockStyle.Bottom;
        this.statusLabel.Location = new System.Drawing.Point(0, 520);
        this.statusLabel.Name = "statusLabel";
        this.statusLabel.Size = new System.Drawing.Size(44, 17);
        this.statusLabel.Text = "就绪";

        // MainForm
        this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(722, 550);
        this.Controls.Add(this.mainTabControl);
        this.Controls.Add(this.statusLabel);
        this.Name = "MainForm";
        this.Text = "窗口截图工具 - 热血传奇辅助";
        ((System.ComponentModel.ISupportInitialize)(this.previewPictureBox)).EndInit();
        ((System.ComponentModel.ISupportInitialize)(this.intervalNumericUpDown)).EndInit();
        this.mainTabControl.ResumeLayout(false);
        this.basicSettingsTabPage.ResumeLayout(false);
        this.basicSettingsTabPage.PerformLayout();
        this.gameAssistantTabPage.ResumeLayout(false);
        this.gameAssistantTabPage.PerformLayout();
        this.ResumeLayout(false);
        this.PerformLayout();
    }
}
