# Template Matching

## Purpose

模板匹配能力，支持数字模板的运行时提取、持久化缓存、以及基于模板的字符识别。血量/蓝量字体和等级字体使用两套独立的模板。

## Requirements

### Requirement: 系统应在首次启动时提取数字模板

首次启动时，系统应从 `screenshot/HP-MP.png` 和 `screenshot/LEVEL.png` 提取数字模板。

#### Scenario: 首次启动提取模板
- **WHEN** 程序首次启动且 `templates/` 目录不存在
- **THEN** 系统从 HP-MP.png 提取血量/蓝量字体模板，从 LEVEL.png 提取等级字体模板

### Requirement: 模板应持久化缓存到磁盘

提取的模板应保存到 `templates/` 目录，后续启动直接加载缓存。

#### Scenario: 模板缓存
- **WHEN** 模板提取完成
- **THEN** 系统保存模板到 `templates/hp_mp/` 和 `templates/level/` 目录

### Requirement: 后续启动应直接加载缓存模板

当模板缓存存在时，系统应直接加载缓存，无需重复提取。

#### Scenario: 加载缓存模板
- **WHEN** 程序启动且 `templates/` 目录存在
- **THEN** 系统从缓存加载模板到内存，启动时间 <100ms

### Requirement: 血量/蓝量和等级应使用独立模板

血量/蓝量字体和等级字体不同，系统应使用两套独立的模板。

#### Scenario: 独立模板
- **WHEN** 识别 HP/MP 时
- **THEN** 系统使用 `templates/hp_mp/` 中的模板
- **WHEN** 识别等级时
- **THEN** 系统使用 `templates/level/` 中的模板

### Requirement: 模板 - 数字映射应通过用户确认建立

首次提取模板时，系统应弹出对话框让用户确认数值，建立模板→数字的映射关系。

#### Scenario: 建立映射
- **WHEN** 从 HP-MP.png 提取字符后
- **THEN** 系统弹出对话框："检测到血量 536/536，蓝量 545/545，是否正确？"
- **WHEN** 用户确认
- **THEN** 系统建立模板字符到数字 0-9 的映射

### Requirement: 系统应支持模板重建

用户应能够删除缓存目录并重新提取模板。

#### Scenario: 模板重建
- **WHEN** 用户删除 `templates/` 目录并重启程序
- **THEN** 系统重新从 HP-MP.png / LEVEL.png 提取模板

### Requirement: 模板文件缺失时应提示用户

当首次启动且截图文件不存在时，系统应提示用户。

#### Scenario: 文件缺失提示
- **WHEN** `templates/` 不存在且 `screenshot/HP-MP.png` 不存在
- **THEN** 系统弹出错误提示："模板文件不存在，请确保 screenshot/HP-MP.png 和 screenshot/LEVEL.png 存在"
