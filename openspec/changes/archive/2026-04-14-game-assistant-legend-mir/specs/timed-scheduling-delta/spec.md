# Timed Scheduling (Delta Spec)

## MODIFIED Requirements

### Requirement: 截图完全静默执行

定时触发截图时，系统不应打扰用户，完全在后台执行。**扩展：支持截图后回调分析服务**。

#### Scenario: 静默截图（原有）
- **WHEN** 定时器触发截图
- **THEN** 系统不弹出任何对话框或通知，直接保存截图

#### Scenario: 截图后调用分析（新增）
- **WHEN** 定时器触发截图且游戏分析功能已启用
- **THEN** 系统完成截图后自动调用 `GameAnalysisService.Analyze()` 方法，不弹出任何对话框

## ADDED Requirements

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
