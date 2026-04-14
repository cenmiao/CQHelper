# Game Image Analysis

## Purpose

游戏图像分析能力，在每次定时截图后自动分析截图内容，识别游戏中的关键信息（血量、蓝量、等级），并将结果提供给 UI 显示和日志记录。

## Requirements

### Requirement: 系统应在每次截图后自动调用分析服务

定时截图服务完成截图后，应自动调用游戏图像分析服务对截图进行分析。

#### Scenario: 定时截图触发分析
- **WHEN** 定时截图服务完成一次截图
- **THEN** 系统自动调用 `GameAnalysisService` 对截图 Bitmap 进行分析

### Requirement: 分析服务应协调多个分析器并行工作

游戏图像分析服务应协调血量/蓝量分析器和等级分析器并行工作，提高分析效率。

#### Scenario: 并行分析
- **WHEN** 分析服务接收到截图
- **THEN** 系统同时调用 HealthBarAnalyzer 和 LevelAnalyzer 进行识别

### Requirement: 分析结果应汇总为统一的数据模型

分析服务应将各分析器的结果汇总为 `GameInfo` 数据模型，包含 HP、MP、等级信息。

#### Scenario: 结果汇总
- **WHEN** 所有分析器完成识别
- **THEN** 系统生成 `GameInfo` 对象，包含 `CurrentHp`、`MaxHp`、`CurrentMp`、`MaxMp`、`Level`

### Requirement: 分析服务应支持启用/禁用开关

用户应能够启用或禁用游戏图像分析功能，禁用时仅截图不分析。

#### Scenario: 禁用分析
- **WHEN** 用户在 UI 中关闭"启用分析"开关
- **THEN** 定时截图继续运行，但不再调用分析服务

### Requirement: 分析失败时不应影响定时截图

当分析服务抛出异常时，系统应捕获异常并记录日志，但不影响定时截图服务的正常运行。

#### Scenario: 分析异常处理
- **WHEN** 分析过程中发生未处理异常
- **THEN** 系统记录错误日志，定时截图服务继续运行
