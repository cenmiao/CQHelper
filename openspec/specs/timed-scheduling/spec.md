# Timed Scheduling

## Purpose

Enables automated screenshot capture at user-defined intervals, allowing users to set up and manage timed screenshot tasks without manual intervention.

## Requirements

### Requirement: 用户可以启动定时截图任务
系统应允许用户启动一个定时截图任务，对选定的目标窗口按固定间隔自动截图。

#### Scenario: 用户启动定时截图
- **WHEN** 用户点击"开始定时截图"按钮
- **THEN** 系统启动定时器，开始按设定间隔自动截图

### Requirement: 用户可以停止定时截图任务
系统应允许用户手动停止正在运行的定时截图任务。

#### Scenario: 用户手动停止定时截图
- **WHEN** 用户点击"停止定时截图"按钮
- **THEN** 系统停止定时器，状态变为"已停止"

### Requirement: 系统按固定间隔触发截图
系统应按照用户设置的时间间隔（秒）自动触发截图操作。

#### Scenario: 定时触发截图
- **WHEN** 定时器运行中且到达设定间隔
- **THEN** 系统自动执行一次截图并保存

### Requirement: 定时精度为秒级
系统应支持秒级的定时精度设置。

#### Scenario: 设置秒级间隔
- **WHEN** 用户设置定时间隔为 N 秒（N >= 1）
- **THEN** 系统按照 N 秒间隔准确触发截图

### Requirement: 截图完全静默执行
定时触发截图时，系统不应打扰用户，完全在后台执行。**扩展：支持截图后回调分析服务**。

#### Scenario: 静默截图（原有）
- **WHEN** 定时器触发截图
- **THEN** 系统不弹出任何对话框或通知，直接保存截图

#### Scenario: 截图后调用分析（新增）
- **WHEN** 定时器触发截图且游戏分析功能已启用
- **THEN** 系统完成截图后自动调用 `GameAnalysisService.Analyze()` 方法，不弹出任何对话框

### Requirement: 窗口不存在时自动停止
当目标窗口不存在时，系统应自动停止定时截图并提示用户。

#### Scenario: 目标窗口已关闭
- **WHEN** 定时截图前检查发现目标窗口不存在
- **THEN** 系统停止定时器，并弹出弹窗提示用户窗口不存在

### Requirement: 定时截图应支持截图后回调
定时截图服务应提供回调机制，在每次截图完成后调用可选的回调函数。

#### Scenario: 注册回调
- **WHEN** 游戏分析服务启动时
- **THEN** 系统注册截图完成回调，每次截图后调用分析服务

### Requirement: 回调执行失败不应影响定时截图
当截图后回调执行抛出异常时，系统应捕获异常并记录日志，但不影响定时截图服务的正常运行。

#### Scenario: 回调异常处理
- **WHEN** 截图完成回调执行发生未处理异常
- **THEN** 系统记录错误日志，定时截图服务继续运行，不中断定时器
