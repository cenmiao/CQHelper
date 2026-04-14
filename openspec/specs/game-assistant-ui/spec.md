# Game Assistant UI

## Purpose

游戏辅助 UI 能力，提供 Tab 布局的界面，显示识别结果和日志输出。

## Requirements

### Requirement: 界面应使用 Tab 布局

主界面应使用 Tab 布局，分为"基础设置"和"游戏辅助"两个标签页。

#### Scenario: Tab 布局
- **WHEN** 用户打开程序
- **THEN** 界面显示两个 Tab："基础设置"和"游戏辅助"

### Requirement: "基础设置"Tab 应包含原有功能

"基础设置"Tab 应包含原有的窗口选择、截图预览、定时控制功能。

#### Scenario: 基础功能
- **WHEN** 用户切换到"基础设置"Tab
- **THEN** 用户可以看到窗口选择器、截图预览、定时间隔设置、开始/停止按钮

### Requirement: "游戏辅助"Tab 应显示识别结果

"游戏辅助"Tab 应显示血量、蓝量、等级的识别结果。

#### Scenario: 显示识别结果
- **WHEN** 用户切换到"游戏辅助"Tab
- **THEN** 界面显示 HP、MP、等级的当前值

### Requirement: "游戏辅助"Tab 应显示日志输出

"游戏辅助"Tab 应包含日志输出框，显示识别成功/失败信息。

#### Scenario: 显示日志
- **WHEN** 识别完成后
- **THEN** 日志框显示 "[HH:mm:ss] ✓ HP: 536/536" 或 "[HH:mm:ss] ✗ 等级识别失败"

### Requirement: 日志应支持手动清空

用户应能够手动清空日志框。

#### Scenario: 清空日志
- **WHEN** 用户点击"清空"按钮
- **THEN** 日志框所有内容被清空

### Requirement: 日志应最多保留 100 条

日志框应最多显示 100 条记录，超出部分自动删除。

#### Scenario: 自动删除旧日志
- **WHEN** 日志数量超过 100 条
- **THEN** 系统自动删除最早的日志，保留最近 100 条

### Requirement: 日志应支持自动滚动

当日志超出显示范围时，应自动滚动到最新日志。

#### Scenario: 自动滚动
- **WHEN** 新日志添加且超出可视范围
- **THEN** 日志框自动滚动到底部，显示最新日志

### Requirement: 识别结果应实时更新

识别完成后，界面应立即更新显示最新结果。

#### Scenario: 实时更新
- **WHEN** 分析服务完成识别
- **THEN** UI 立即更新 HP、MP、等级显示
