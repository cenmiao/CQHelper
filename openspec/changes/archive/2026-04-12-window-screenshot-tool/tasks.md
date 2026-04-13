# TDD 任务清单 - WindowScreenshot

## 核心原则

1. **RED**: 先写失败的测试
2. **GREEN**: 写最少的代码通过测试
3. **REFACTOR**: 重构，保持测试通过

---

## 1. 项目设置和测试基础设施

- [x] 1.1 创建 .NET 8 WinForm 项目
- [x] 1.2 添加 xUnit 测试项目
- [x] 1.3 配置测试项目引用主项目
- [x] 1.4 运行第一个空测试验证测试基础设施

---

## 2. 窗口枚举功能 (WindowEnumerator)

### RED-Green-Refactor 循环

#### 2.1 窗口数据结构
- [x] **RED**: 写测试 `WindowInfo_应包含句柄和标题`
- [x] **GREEN**: 创建 `WindowInfo` 结构体（Handle, Title 属性）
- [x] **REFACTOR**: 检查命名是否清晰

#### 2.2 枚举窗口
- [x] **RED**: 写测试 `EnumWindows_应返回非空列表`
- [x] **RED**: 写测试 `EnumWindows_应只返回有标题的窗口`
- [x] **RED**: 写测试 `EnumWindows_应排除工具窗口`
- [x] **GREEN**: 创建 `WindowEnumerator` 类，实现 `EnumWindows()` 方法
- [x] **REFACTOR**: 提取重复逻辑

#### 2.3 窗口列表格式化
- [x] **RED**: 写测试 `GetWindowDisplayName_无标题窗口返回占位文本`
- [x] **GREEN**: 实现 `GetWindowDisplayName()` 方法
- [x] **REFACTOR**: 检查常量提取

---

## 3. 截图功能 (WindowCapturer)

### RED-Green-Refactor 循环

#### 3.1 获取窗口边界
- [x] **RED**: 写测试 `GetWindowBounds_应返回有效的矩形区域`
- [x] **RED**: 写测试 `GetWindowBounds_无效句柄抛出异常`
- [x] **GREEN**: 创建 `WindowCapturer` 类，实现 `GetWindowBounds()` 方法
- [x] **REFACTOR**: 提取错误处理逻辑

#### 3.2 截图核心逻辑
- [x] **RED**: 写测试 `Capture_应返回非空 Bitmap`
- [x] **RED**: 写测试 `Capture_截图尺寸应与窗口一致`
- [x] **GREEN**: 实现 `Capture(IntPtr handle)` 方法
- [x] **REFACTOR**: 检查资源释放（Dispose）

#### 3.3 延时截图
- [x] **RED**: 写测试 `CaptureWithDelay_应等待指定时间后截图`
- [x] **GREEN**: 实现 `CaptureWithDelay(IntPtr handle, int delayMs)` 方法
- [x] **REFACTOR**: 检查 Timer 使用是否恰当

---

## 4. 保存功能 (ScreenshotSaver)

### RED-Green-Refactor 循环

#### 4.1 文件名生成
- [x] **RED**: 写测试 `GenerateFilename_应使用时间戳格式`
- [x] **RED**: 写测试 `GenerateFilename_文件名应唯一`
- [x] **GREEN**: 创建 `ScreenshotSaver` 类，实现 `GenerateFilename()` 方法
- [x] **REFACTOR**: 提取时间戳格式常量

#### 4.2 保存截图
- [x] **RED**: 写测试 `Save_应保存到 screenshot 目录`
- [x] **RED**: 写测试 `Save_目录不存在时应创建`
- [x] **RED**: 写测试 `Save_应使用 PNG 格式`
- [x] **GREEN**: 实现 `Save(Bitmap image, string filename)` 方法
- [x] **REFACTOR**: 检查路径处理逻辑

#### 4.3 完整截图流程
- [x] **RED**: 写测试 `CaptureAndSave_应返回保存的文件路径`
- [x] **GREEN**: 实现 `CaptureAndSave()` 方法
- [x] **REFACTOR**: 检查异常处理

---

## 5. 界面功能 (MainForm)

### 集成测试为主

#### 5.1 窗口列表绑定
- [x] **RED**: 写测试 `Form_加载时应填充窗口列表`
- [x] **GREEN**: 实现 `MainForm_Load()` 事件处理
- [x] **REFACTOR**: 检查 UI 线程调用

#### 5.2 刷新功能
- [x] **RED**: 写测试 `刷新按钮_点击后应更新窗口列表`
- [x] **GREEN**: 实现 `refreshButton_Click()` 事件处理
- [x] **REFACTOR**: 提取刷新逻辑到独立方法

#### 5.3 截图按钮状态
- [x] **RED**: 写测试 `截图按钮_未选择窗口时应禁用`
- [x] **RED**: 写测试 `截图按钮_选择窗口后应启用`
- [x] **GREEN**: 实现 `comboBox_SelectedIndexChanged()` 事件处理
- [x] **REFACTOR**: 检查状态管理

#### 5.4 预览功能
- [x] **RED**: 写测试 `截图后_预览区域应显示图像`
- [x] **GREEN**: 实现预览更新逻辑
- [x] **REFACTOR**: 检查图像缩放

---

## 6. 错误处理

### RED-Green-Refactor 循环

#### 6.1 无窗口选择
- [x] **RED**: 写测试 `截图_无窗口选择时应提示`
- [x] **GREEN**: 实现验证逻辑
- [x] **REFACTOR**: 提取验证方法

#### 6.2 截图失败
- [x] **RED**: 写测试 `截图_失败时应显示错误信息`
- [x] **RED**: 写测试 `截图_失败时应记录异常`
- [x] **GREEN**: 实现 try-catch 错误处理
- [x] **REFACTOR**: 检查异常类型处理

---

## 7. 集成验证

### 手动测试场景（编写集成测试）

- [x] 7.1 单显示器场景截图
- [x] 7.2 多显示器场景截图
- [x] 7.3 高 DPI 场景
- [x] 7.4 不同类型窗口（浏览器、编辑器、系统窗口）
- [x] 7.5 窗口最小化后恢复场景

---

## TDD 执行规则

1. **每个 RED 步骤必须运行测试并确认失败**
2. **每个 GREEN 步骤必须运行测试并确认通过**
3. **所有现有测试必须保持通过**
4. **不写生产代码除非有失败的测试**
5. **不添加测试未要求的功能**

---

## 验证清单

完成每个模块前确认：

- [x] 所有测试通过
- [x] 没有编译警告
- [x] 代码已重构（无重复、命名清晰）
- [x] 下一个模块的测试已编写（RED 状态）
