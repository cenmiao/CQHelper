# Design - Game Assistant (Legend MIR)

## Context

现有窗口截图工具已具备定时截图功能（`TimedScreenshotService`），用户可以对任意窗口进行定时截图。本设计在此基础上增加图像分析能力，将工具改造为热血传奇游戏辅助。

**技术约束：**
- 不能依赖 Python 运行时（必须纯 .NET 方案）
- 识别精度要求高（游戏字体较小）
- 游戏窗口固定 1920x1080 分辨率
- 血量/蓝量字体与等级字体不同，需独立模板

**现有架构：**
- 分层架构：Data Layer → Service Layer → Presentation Layer
- 依赖注入：服务通过构造函数接收依赖
- 事件驱动：`TimedScreenshotService` 暴露事件用于扩展

---

## Goals / Non-Goals

**Goals:**
- 实现血量/蓝量自动识别（左下角数字区域）
- 实现等级自动识别（右下角数字区域）
- 模板首次启动提取 + 持久化缓存
- UI 使用 Tab 布局，不破坏现有功能
- 日志输出识别结果（最多 100 条，支持清空）
- ROI 使用相对坐标（百分比），支持配置微调
- ROI 坐标先实现后调，默认值先行

**Non-Goals:**
- 自动喝药等自动化操作（预留接口，后续实现）
- 角色位置识别（已简化需求）
- 任务信息识别（已简化需求）
- 多游戏支持（仅支持热血传奇）
- 非 1920x1080 分辨率支持（后续扩展）

---

## Decisions

### 1. 图像识别库选择：OpenCvSharp4

**决策：** 使用 `OpenCvSharp4` 而非 `System.Drawing` 或 Tesseract OCR

**理由：**
- OpenCvSharp 是 OpenCV 的 C# 封装，提供完整 CV 算法库
- 模板匹配准确率 98%+，速度 <20ms
- 纯.NET 调用，无需 Python 依赖
- 支持 HSV 颜色空间转换（比 RGB 更稳定）

**替代方案：**
- System.Drawing：基础图像操作，无高级 CV 算法
- Tesseract OCR：需要训练数据，对游戏字体识别率不稳定（85-90%）

### 2. 模板管理：首次提取 + 持久化缓存

**决策：** 首次启动时从截图提取模板，保存到 `templates/` 目录，后续使用缓存

**理由：**
- 避免每次截图都提取模板（开销 50-100ms）
- 缓存后启动时间 <5ms
- 用户无需永久保留 HP-MP.png / LEVEL.png

**流程：**
```
首次启动：HP-MP.png → 提取 → 保存到 templates/ → 加载到内存
后续启动：templates/ → 加载到内存（无需原图）
```

### 3. ROI 定位：相对坐标（百分比）

**决策：** 使用相对坐标（百分比）而非绝对像素坐标

**理由：**
- 用户无法提供精确坐标
- 代码自适应不同分辨率（保持 16:9 比例）
- 可通过配置微调（如识别不准）

**默认 ROI（基于 1920x1080）：**
```csharp
// HP/MP 区域（左下角）
hpRoi = (x: 1.5%, y: 93%, width: 7%, height: 4%)
mpRoi = (x: 7%, y: 93%, width: 7%, height: 4%)

// 等级区域（右下角）
levelRoi = (x: 91%, y: 63%, width: 5%, height: 3%)
```

**调整策略：**
- 首次实现使用默认值
- 后续根据实际测试结果通过配置文件微调
- ROI 配置保存到 `config.json`

### 4. 两套独立模板

**决策：** 血量/蓝量 和 等级 使用独立模板目录

**理由：**
- 字体不同，不能共用模板
- HP/MP 字体：较小，白色带黑色描边
- 等级字体：可能与 HP/MP 不同

**结构：**
```
templates/
├── hp_mp/      # 血量/蓝量字体模板
│   ├── char_0.png ~ char_9.png
│   └── char_slash.png
└── level/      # 等级字体模板
    └── char_0.png ~ char_9.png
```

### 5. UI 布局：Tab 分页

**决策：** 使用 Tab 布局分离"基础设置"和"游戏辅助"

**理由：**
- 现有界面已拥挤，新增内容需要空间
- Tab 布局保持界面整洁
- 原有功能不受影响

**Tab 结构：**
- Tab 1 "基础设置"：现有窗口选择、截图预览、定时控制
- Tab 2 "游戏辅助"：识别结果显示、日志输出框

### 6. 模板 - 数字映射：自动分析 + 用户确认

**决策：** 首次启动时弹出对话框确认数值，建立模板映射

**理由：**
- 完全自动可能误判（风险高）
- 用户手动输入最可靠
- 单次对话框，后续无需再问

**流程：**
```
1. 从 HP-MP.png 提取字符 → "536/536 545/545"
2. 弹出对话框："检测到血量 536/536，蓝量 545/545，是否正确？"
3. 用户确认 → 建立 模板→数字 映射
4. 等级同理
```

### 7. 截图后回调机制

**决策：** `TimedScreenshotService` 新增 `ScreenshotCaptured` 事件，支持截图后回调

**理由：**
- 解耦定时截图服务与游戏分析服务
- 保持单一职责原则
- 便于未来扩展其他截图后处理逻辑

**实现方式：**
```csharp
// TimedScreenshotService 新增事件
public event Action<Bitmap>? ScreenshotCaptured;

// 在 PerformScreenshot() 中触发
private void PerformScreenshot()
{
    using var bitmap = _capturer.Capture(_targetWindowHandle);
    // ... 保存截图 ...
    
    // 触发回调（游戏分析服务订阅）
    ScreenshotCaptured?.Invoke(bitmap);
}
```

### 8. 实现方案：渐进式重构

**决策：** 按照依赖顺序分阶段实现，每一步都可独立测试

**阶段划分：**
1. Phase 1: 依赖与数据模型
2. Phase 2: 模板匹配系统
3. Phase 3: 游戏分析器实现
4. Phase 4: 游戏分析服务
5. Phase 5: 集成与回调机制
6. Phase 6: 配置扩展
7. Phase 7: UI 扩展
8. Phase 8: 测试与验证

---

## Architecture

### 新增组件

```
┌─────────────────────────────────────────────────────────────┐
│                      Presentation Layer                      │
│  ┌─────────────────────────────────────────────────────────┐ │
│  │  MainForm (Tab Layout)                                  │ │
│  │  ├─ Tab 1: 基础设置（原有功能）                          │ │
│  │  └─ Tab 2: 游戏辅助（新增）                              │ │
│  │      ├─ 识别结果面板 (HP, MP, Level)                     │ │
│  │      └─ 日志输出框 (GameLog)                             │ │
│  └─────────────────────────────────────────────────────────┘ │
└─────────────────────────────────────────────────────────────┘
                           ↕
┌─────────────────────────────────────────────────────────────┐
│                       Service Layer                          │
│  ┌──────────────────┐  ┌─────────────────────────────────┐  │
│  │ TimedScreenshot  │  │ GameAnalysisService             │  │
│  │ Service          │  │ ├─ IGameAnalyzer                │  │
│  │ ├─ Screenshot-   │  │ ├─ HealthBarAnalyzer            │  │
│  │    Captured 事件 │  │ └─ LevelAnalyzer                │  │
│  │ └─ 定时触发截图  │  └─────────────────────────────────┘  │
│  └──────────────────┘            ↑                          │
│         ↑                        │                          │
│         └────────────────────────┘                          │
│                   订阅事件                                   │
└─────────────────────────────────────────────────────────────┘
                           ↕
┌─────────────────────────────────────────────────────────────┐
│                       Data Layer                             │
│  ┌────────────────┐  ┌──────────────┐  ┌─────────────────┐  │
│  │ TemplateManager│  │  GameInfo    │  │   GameLog       │  │
│  │ ├─ 模板提取    │  │  ├─ CurrentHp│  │  ├─ 最多 100 条   │  │
│  │ ├─ 持久化缓存  │  │  ├─ MaxHp    │  │  ├─ 清空         │  │
│  │ └─ 模板识别    │  │  ├─ CurrentMp│  │  └─ 自动滚动     │  │
│  └────────────────┘  │  ├─ MaxMp    │  └─────────────────┘  │
│                      │  └─ Level    │                       │
│                      └──────────────┘                       │
└─────────────────────────────────────────────────────────────┘
```

### 数据流

```
定时截图触发
     ↓
TimedScreenshotService.Timer_Tick()
     ↓
PerformScreenshot()
     ├─→ 捕获截图 (Bitmap)
     ├─→ 保存到文件
     └─→ 触发 ScreenshotCaptured 事件
              ↓
         MainForm 回调订阅
              ↓
         GameAnalysisService.Analyze(Bitmap)
              ├─→ HealthBarAnalyzer.Analyze() → "536/536 545/545"
              └─→ LevelAnalyzer.Analyze()     → "44"
              ↓
         GameInfo 对象汇总
              ├─→ 更新 UI 显示
              └─→ 记录日志
```

---

## Risks / Trade-offs

| 风险 | 影响 | 缓解措施 |
|------|------|----------|
| 游戏 UI 变化（版本更新） | 模板失效 | 提供模板重建功能（删除缓存重新提取） |
| ROI 坐标准确性 | 识别失败 | 提供配置界面允许微调百分比 |
| 光线/特效干扰 | 误识别 | HSV 颜色空间 + 合理阈值 |
| 性能开销 | 分析耗时 | OpenCvSharp 性能优秀（单次<50ms），截图间隔>1 秒 |
| 文件依赖 | 首次运行需要截图 | 提供友好的错误提示 |
| 字体变化 | 模板不匹配 | 支持多套模板配置（未来扩展） |

---

## Migration Plan

### 部署步骤

1. **安装依赖**
   ```bash
   dotnet add package OpenCvSharp4
   dotnet add package OpenCvSharp4.Extensions
   dotnet add package OpenCvSharp4.runtime.win
   ```

2. **首次运行准备**
   - 确保 `screenshot/HP-MP.png` 和 `screenshot/LEVEL.png` 存在
   - 启动程序，完成模板初始化对话框
   - 验证 `templates/` 目录生成

3. **功能验证**
   - 选择游戏窗口
   - 启动定时截图
   - 观察识别结果和日志输出

### 回滚策略

如需回滚：
1. 删除新增的 Analyzers/ 目录
2. 删除 `GameAnalysisService.cs`、`GameLog.cs` 等新增文件
3. 还原 `MainForm.cs` 和 `MainForm.Designer.cs` 到 Tab 布局前
4. 从 `.csproj` 移除 OpenCvSharp4 依赖

---

## Open Questions

1. **ROI 精确坐标**：需要实际测试验证默认百分比是否准确，可能需要用户提供反馈后调整
2. **模板初始化体验**：是否需要提供"预览提取结果"功能，让用户确认模板正确性
3. **配置持久化**：ROI 百分比是否保存到 `config.json`（推荐保存，便于微调）
