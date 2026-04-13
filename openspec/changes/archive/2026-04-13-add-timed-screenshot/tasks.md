## 1. 配置管理模块

- [x] 1.1 创建 `ScreenshotSettings` 类，定义配置数据结构（TargetWindowTitle, TargetWindowClassName, IntervalSeconds, IsEnabled）
- [x] 1.2 创建 `ConfigManager` 类，实现配置的读写逻辑
- [x] 1.3 实现配置路径管理（`%APPDATA%/CQHelper/config.json`）
- [x] 1.4 实现配置目录自动创建功能
- [x] 1.5 添加 JSON 序列化/反序列化支持（使用 `System.Text.Json`）

## 2. 窗口查找功能

- [x] 2.1 创建 `WindowFinder` 类或在现有 `WindowEnumerator` 中扩展查找方法
- [x] 2.2 实现根据窗口标题和类名查找窗口句柄的方法
- [x] 2.3 实现窗口标题前缀匹配逻辑
- [x] 2.4 添加窗口不存在时的返回值处理（null 或 IntPtr.Zero）

## 3. 定时截图服务

- [x] 3.1 创建 `TimedScreenshotService` 类，封装定时器逻辑
- [x] 3.2 使用 `System.Windows.Forms.Timer` 实现定时触发
- [x] 3.3 实现启动/停止定时器方法
- [x] 3.4 实现截图前窗口存在性验证
- [x] 3.5 实现窗口不存在时的停止和弹窗提示逻辑
- [x] 3.6 实现静默截图（无额外 UI 交互）
- [x] 3.7 集成现有 `WindowCapturer` 和 `ScreenshotSaver` 类

## 4. UI 扩展

- [x] 4.1 修改 `MainForm.Designer.cs`，添加定时间隔输入框（NumericUpDown）
- [x] 4.2 添加定时状态标签（Label）和下次截图时间显示
- [x] 4.3 修改"截图"按钮为"开始定时截图"/"停止定时截图"切换按钮，或添加独立按钮
- [x] 4.4 添加定时状态指示器（已停止/运行中）
- [x] 4.5 实现下拉框加载已保存的窗口信息（程序启动时）

## 5. 集成与生命周期

- [x] 5.1 在 `MainForm` 构造函数或 Load 事件中加载配置
- [x] 5.2 在窗口选择变化时保存配置
- [x] 5.3 在定时间隔变化时保存配置
- [x] 5.4 在程序关闭时保存最终配置
- [x] 5.5 在定时器运行时关闭窗口，正确停止定时器

## 6. 测试与验证

- [x] 6.1 测试配置保存和加载功能
- [x] 6.2 测试定时截图基本流程（设置间隔 → 启动 → 验证截图生成）
- [x] 6.3 测试窗口关闭后的停止和弹窗提示
- [x] 6.4 测试程序重启后配置恢复
- [x] 6.5 验证手动截图功能未受影响
