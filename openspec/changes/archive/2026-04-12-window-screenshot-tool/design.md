## Context

用户需要一个简单易用的窗口截图工具，能够选择任意桌面窗口并为其截图。当前项目目录为空，需要从头创建一个 .NET 8 WinForm 应用程序。

**约束条件：**
- 技术栈：C# WinForm
- 目标框架：.NET 8
- 截图保存位置：项目根目录/screenshot 文件夹
- 图片格式：PNG
- 图片命名：时间戳格式

## Goals / Non-Goals

**Goals:**
- 提供下拉列表选择桌面窗口
- 手动刷新窗口列表
- 点击截图后自动最小化工具窗口
- 延时 1 秒后对目标窗口截图
- 截图自动保存到 screenshot 目录
- 在界面内显示截图预览
- 截图失败时显示错误信息

**Non-Goals:**
- 不支持截图编辑功能
- 不支持多窗口批量截图
- 不支持最小化窗口的截图
- 不支持系统托盘常驻
- 不支持截图上传或分享功能

## Decisions

### 1. 窗口枚举方式

**决策：** 使用 Windows API `EnumWindows` + `GetWindowText` 枚举所有可见窗口

**理由：**
- 成熟稳定的 API
- 性能好，枚举速度快
- 可以获取窗口的准确标题和句柄

**替代方案：**
- `Process.GetProcesses()` - 只能获取进程，不能获取同一进程的多个窗口
- `UIAutomation` - 更复杂，性能较差

### 2. 截图方式

**决策：** 使用 `Graphics.CopyFromScreen` + `GetWindowRect` API

**理由：**
- 简单易用，.NET 原生支持
- 可以精确控制截图区域

**替代方案：**
- `PrintWindow` API - 可以截取最小化窗口，但兼容性有问题
- `BitBlt` - 更底层，但代码复杂度更高

### 3. 延时截图实现

**决策：** 使用 `System.Windows.Forms.Timer`，延时 1 秒

**理由：**
- WinForm 原生组件
- 在 UI 线程执行，不需要跨线程处理

### 4. 界面布局

**决策：** 使用 `TableLayoutPanel` + `FlowLayoutPanel` 组合布局

**理由：**
- 支持 DPI 自适应
- 代码可维护性好
- 便于后续调整

### 5. 窗口最小化处理

**决策：** 点击截图按钮后，工具窗口立即最小化，延时结束后截图，然后恢复窗口

**理由：**
- 避免工具窗口遮挡目标窗口
- 用户体验直观

## Risks / Trade-offs

| Risk | Mitigation |
|------|------------|
| 某些窗口可能无法正确获取边界 | 使用 `GetWindowRect` + `DwmGetWindowAttribute` 组合获取准确边界 |
| 多显示器场景下坐标计算可能错误 | 使用 `Screen.FromHandle` 获取正确的显示器信息 |
| 截图时目标窗口可能被其他窗口遮挡 | 文档说明此限制，用户需自行确保目标窗口在前台 |
| 最小化窗口无法截图 | 文档说明此限制，提示用户先恢复窗口 |
| 高 DPI 屏幕下截图可能模糊 | 设置 `AutoScaleMode = DPI`，使用 Per-Monitor DPI 模式 |

## Migration Plan

不适用 - 新项目，无迁移需求。

## Open Questions

无
