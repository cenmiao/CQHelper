# Level Recognition

## Purpose

等级识别能力，从游戏窗口截图的右下角区域识别角色等级数值。

## Requirements

### Requirement: 系统应从右下角 ROI 识别等级数值

系统应从截图的右下角区域（相对坐标：x 91%, y 63%, width 5%, height 3%）识别等级数值。

#### Scenario: 识别等级
- **WHEN** 分析服务调用 LevelAnalyzer
- **THEN** 系统从 Level ROI 区域识别出等级数字（如 "44"）

### Requirement: 识别结果应解析为整数类型

识别到的字符串应解析为整数类型，便于显示和后续使用。

#### Scenario: 解析等级
- **WHEN** 模板匹配识别出字符串 "44"
- **THEN** 系统解析为 `Level = 44`

### Requirement: ROI 坐标应支持配置微调

用户应能够在配置文件中调整 ROI 的相对坐标百分比，以适应不同的 UI 布局。

#### Scenario: 调整 ROI 配置
- **WHEN** 用户发现识别不准时修改配置中的 ROI 百分比
- **THEN** 系统使用新的百分比重新计算 ROI 区域进行识别

### Requirement: 识别失败时应返回错误信息

当 ROI 区域无法识别到有效数值时，系统应返回错误信息而非抛出异常。

#### Scenario: 识别失败处理
- **WHEN** ROI 区域内无有效数字或图像质量差
- **THEN** 系统返回识别失败错误，日志记录 "等级识别失败"
