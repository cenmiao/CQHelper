# Configuration Management

## Purpose

Provides persistent configuration storage for the screenshot tool, allowing users to save and restore window selections and timing settings across application sessions.

## Requirements

### Requirement: 系统使用 JSON 格式存储配置
配置数据应以 JSON 格式保存到文件。

#### Scenario: 配置文件格式
- **WHEN** 系统保存配置时
- **THEN** 配置文件为有效的 JSON 格式

### Requirement: 配置文件存储在 APPDATA 目录
配置文件应存储在 `%APPDATA%/CQHelper/config.json` 路径。

#### Scenario: 配置文件路径
- **WHEN** 系统需要读取或写入配置时
- **THEN** 使用 `%APPDATA%/CQHelper/config.json` 路径

### Requirement: 保存目标窗口信息
系统应持久化存储用户选择的目标窗口信息（窗口标题、类名）。

#### Scenario: 保存窗口选择
- **WHEN** 用户从下拉框选择窗口后
- **THEN** 系统将窗口标题和类名保存到配置文件

### Requirement: 保存定时间隔设置
系统应持久化存储用户设置的定时间隔（秒）。

#### Scenario: 保存定时间隔
- **WHEN** 用户修改定时间隔输入框的值
- **THEN** 系统将间隔值保存到配置文件

### Requirement: 程序启动时加载配置
系统启动时应自动加载已保存的配置文件。

#### Scenario: 启动时恢复配置
- **WHEN** 程序启动时
- **THEN** 系统从配置文件加载窗口信息和定时间隔，并更新 UI

### Requirement: 配置目录不存在时自动创建
如果配置目录不存在，系统应自动创建。

#### Scenario: 首次运行创建目录
- **WHEN** 配置文件目录不存在且需要保存配置时
- **THEN** 系统自动创建目录结构
