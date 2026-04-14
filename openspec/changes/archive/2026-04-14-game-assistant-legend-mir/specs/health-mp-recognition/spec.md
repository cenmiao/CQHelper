# Health MP Recognition

## Purpose

血量/蓝量识别能力，从游戏窗口截图的左下角区域识别当前血量/最大血量、当前蓝量/最大蓝量的数值。

## Requirements

### Requirement: 系统应从左下角 ROI 识别血量数值

系统应从截图的左下角区域（相对坐标：x 1.5%, y 93%, width 7%, height 4%）识别血量数值。

#### Scenario: 识别血量
- **WHEN** 分析服务调用 HealthBarAnalyzer
- **THEN** 系统从 HP ROI 区域识别出 "当前值/最大值" 格式的血量数值

### Requirement: 系统应从左下角 ROI 识别蓝量数值

系统应从截图的左下角区域（相对坐标：x 7%, y 93%, width 7%, height 4%）识别蓝量数值。

#### Scenario: 识别蓝量
- **WHEN** 分析服务调用 HealthBarAnalyzer
- **THEN** 系统从 MP ROI 区域识别出 "当前值/最大值" 格式的蓝量数值

### Requirement: 识别结果应解析为数值类型

识别到的字符串应解析为整数类型，便于后续计算和比较。

#### Scenario: 解析数值
- **WHEN** 模板匹配识别出字符串 "536/536"
- **THEN** 系统解析为 `CurrentHp = 536`, `MaxHp = 536`

### Requirement: ROI 坐标应支持配置微调

用户应能够在配置文件中调整 ROI 的相对坐标百分比，以适应不同的 UI 布局。

#### Scenario: 调整 ROI 配置
- **WHEN** 用户发现识别不准时修改配置中的 ROI 百分比
- **THEN** 系统使用新的百分比重新计算 ROI 区域进行识别

### Requirement: 识别失败时应返回错误信息

当 ROI 区域无法识别到有效数值时，系统应返回错误信息而非抛出异常。

#### Scenario: 识别失败处理
- **WHEN** ROI 区域内无有效数字或图像质量差
- **THEN** 系统返回识别失败错误，日志记录 "HP 识别失败"
