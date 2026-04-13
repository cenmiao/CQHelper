## Why

用户在日常使用中经常需要截取特定窗口的截图，但现有截图工具要么功能复杂，要么无法精确选择目标窗口。本工具提供一个简单、专注的窗口截图解决方案。

## What Changes

- 新增 WinForm 桌面应用程序
- 窗口选择功能：通过下拉列表选择目标窗口
- 窗口截图功能：延时 1 秒自动截图
- 截图预览功能：在界面内显示截图结果
- 自动保存功能：截图自动保存到 screenshot 目录
- 错误提示功能：截图失败时显示错误信息

## Capabilities

### New Capabilities

- `window-enumeration`: 枚举系统打开的所有桌面窗口，支持刷新窗口列表
- `window-capture`: 对选定的窗口进行截图，支持延时和自动最小化
- `preview-display`: 在界面内显示截图预览
- `auto-save`: 自动将截图保存到指定目录，使用时间戳命名

### Modified Capabilities

<!-- 无修改现有能力 -->

## Impact

- 新增 .NET 8 WinForm 项目
- 依赖 Windows API (EnumWindows, GetWindowText 等)
- 新增 screenshot 目录用于存储截图
