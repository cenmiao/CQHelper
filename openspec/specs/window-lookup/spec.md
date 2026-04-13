# Window Lookup

## Purpose

Provides window discovery and matching capabilities, allowing the system to find and identify target windows by title and class name for screenshot capture.

## Requirements

### Requirement: 根据窗口标题和类名查找窗口
系统应能够根据窗口标题和类名查找并获取窗口句柄。

#### Scenario: 查找匹配的窗口
- **WHEN** 程序启动或需要验证窗口存在性时
- **THEN** 系统遍历所有顶层窗口，找到标题和类名匹配的窗口并返回句柄

### Requirement: 窗口不存在时返回空值
当没有窗口匹配时，查找函数应返回空值。

#### Scenario: 窗口已关闭
- **WHEN** 目标窗口已关闭或不存在
- **THEN** 查找函数返回 null 或 IntPtr.Zero

### Requirement: 支持模糊匹配窗口标题
窗口标题匹配应支持子串匹配，因为窗口标题可能包含动态内容。

#### Scenario: 窗口标题包含额外内容
- **WHEN** 保存的标题是"Chrome 浏览器"，实际窗口标题是"Chrome 浏览器 - 标签页 1"
- **THEN** 系统能够匹配成功（保存的标题是实际标题的前缀）
