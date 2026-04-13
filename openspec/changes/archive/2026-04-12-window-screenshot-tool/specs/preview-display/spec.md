## ADDED Requirements

### Requirement: 截图预览
系统应当在界面内显示截图的预览图像。

#### Scenario: 显示预览
- **WHEN** 截图成功后
- **THEN** 预览区域应显示截图图像

#### Scenario: 预览尺寸适配
- **WHEN** 截图图像尺寸大于预览区域时
- **THEN** 图像应按比例缩放以适应预览区域

#### Scenario: 预览清除
- **WHEN** 用户开始新的截图时
- **THEN** 预览区域应清除之前的预览图像
