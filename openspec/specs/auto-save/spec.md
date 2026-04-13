# Capability: Auto Save

## Purpose

Automatically save screenshots to a designated directory with timestamp-based filenames in PNG format.

## Requirements

### Requirement: 自动保存截图
系统应当自动将截图保存到指定的 screenshot 目录。

#### Scenario: 成功保存
- **WHEN** 截图完成后
- **THEN** 截图应自动保存到/screenshot 目录

#### Scenario: 文件命名
- **WHEN** 保存截图文件时
- **THEN** 文件名应使用时间戳格式（screenshot_yyyyMMdd_HHmmss.png）

#### Scenario: 保存格式
- **WHEN** 保存截图文件时
- **THEN** 文件应以 PNG 格式保存

#### Scenario: 目录不存在处理
- **WHEN** screenshot 目录不存在时
- **THEN** 系统应自动创建该目录

#### Scenario: 保存状态提示
- **WHEN** 截图保存成功后
- **THEN** 界面应显示已保存文件的名称
