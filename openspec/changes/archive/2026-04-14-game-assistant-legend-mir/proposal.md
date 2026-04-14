## Why

将现有窗口截图工具改造为热血传奇游戏辅助工具，实现对游戏窗口截图的自动分析，识别角色血量/蓝量和等级信息并在界面上显示，为后续自动化功能（如自动喝药）提供数据基础。

## What Changes

- **新增** 游戏图像分析服务（`GameAnalysisService`），在每次定时截图后自动分析
- **新增** 血量/蓝量识别功能（`HealthBarAnalyzer`），从左下角识别 HP/MP 数值
- **新增** 等级识别功能（`LevelAnalyzer`），从右下角识别角色等级
- **新增** 模板匹配系统（`TemplateManager`），支持运行时提取数字模板并持久化缓存
- **新增** 游戏辅助 UI 界面（Tab 分页），显示识别结果和日志输出
- **新增** 日志管理系统，支持最多 100 条滚动显示和手动清空
- **扩展** `MainForm` 支持 Tab 布局和游戏辅助功能入口
- **扩展** `TimedScreenshotService` 支持截图后回调分析

## Capabilities

### New Capabilities

- `game-image-analysis`: 游戏图像分析核心能力，包括截图接收、分析协调、结果汇总
- `health-mp-recognition`: 血量/蓝量识别能力，从左下角 ROI 识别当前值/最大值
- `level-recognition`: 等级识别能力，从右下角 ROI 识别角色等级
- `template-matching`: 模板匹配能力，支持数字模板提取、缓存、识别
- `game-assistant-ui`: 游戏辅助 UI 能力，包括识别结果面板、日志输出框、Tab 布局

### Modified Capabilities

- `timed-screenshot`: 扩展定时截图服务，增加截图后分析回调（非破坏性扩展）

## Impact

- **依赖新增**: 需要引入 `OpenCvSharp4` NuGet 包用于图像识别
- **文件依赖**: 首次运行需要 `screenshot/HP-MP.png` 和 `screenshot/LEVEL.png` 用于模板提取
- **配置扩展**: `config.json` 需要新增游戏辅助相关配置（ROI 百分比、启用状态等）
- **UI 变更**: `MainForm` 重构为 Tab 布局，原有功能保留在"基础设置"Tab
- **分辨率假设**: 默认游戏窗口为 1920x1080，使用相对坐标（百分比）定位 ROI
