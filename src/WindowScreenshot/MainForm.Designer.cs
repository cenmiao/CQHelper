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
