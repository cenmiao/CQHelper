## Why

用户在现有窗口截图功能基础上，需要自动化的定时截图能力，以便持续监控窗口状态变化。当前实现仅支持手动触发截图，无法满足需要定期捕获窗口画面的使用场景。

## What Changes

- 新增定时截图功能，支持用户设置秒级定时间隔
- 新增 JSON 配置文件，持久化存储目标窗口信息和定时设置
- 新增定时器管理功能（启动/停止）
- 扩展主界面 UI，增加定时间隔输入框和定时状态显示
- 新增窗口不存在时的自动停止和弹窗提示机制

## Capabilities

### New Capabilities

- `timed-scheduling`: 定时调度能力，支持固定间隔的秒级定时截图
- `configuration-management`: 配置管理能力，使用 JSON 文件持久化存储用户设置
- `window-lookup`: 窗口查找能力，根据窗口标题和类名重新获取窗口句柄

### Modified Capabilities

（无）

## Impact

- **修改文件**: `MainForm.cs`, `MainForm.Designer.cs` - 扩展 UI 和定时器逻辑
- **新增文件**: `ConfigManager.cs` - 配置管理, `TimedScreenshotService.cs` - 定时服务
- **新增依赖**: `System.Text.Json` - JSON 序列化（.NET 8 内置）
- **配置文件**: `%APPDATA%/CQHelper/config.json`
- **新增类**: `System.Windows.Forms.Timer` 实例
