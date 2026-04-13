# Capability: Window Capture

## Purpose

Capture screenshots of specified desktop windows with support for delayed capture and automatic window minimization.

## Requirements

### Requirement: 窗口截图
系统应当能够对用户选定的窗口进行截图。

#### Scenario: 成功截图
- **WHEN** 用户选择一个窗口并点击截图按钮
- **THEN** 系统应在延时 1 秒后对目标窗口进行截图

#### Scenario: 自动最小化
- **WHEN** 用户点击截图按钮
- **THEN** 工具窗口应立即最小化，避免遮挡目标窗口

#### Scenario: 截图后恢复
- **WHEN** 截图完成后
- **THEN** 工具窗口应恢复到正常状态

#### Scenario: 无窗口选择处理
- **WHEN** 用户未选择任何窗口就点击截图按钮
- **THEN** 系统应提示用户先选择一个窗口

#### Scenario: 截图失败处理
- **WHEN** 截图操作失败时
- **THEN** 系统应显示错误信息提示用户
