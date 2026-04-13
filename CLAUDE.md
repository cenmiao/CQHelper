# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Build and Test Commands

```bash
# Build the main project
dotnet build src/WindowScreenshot/WindowScreenshot.csproj

# Build the test project
dotnet build tests/WindowScreenshot.Tests/WindowScreenshot.Tests.csproj

# Run all tests
dotnet test tests/WindowScreenshot.Tests/WindowScreenshot.Tests.csproj

# Run specific test class
dotnet test tests/WindowScreenshot.Tests/WindowScreenshot.Tests.csproj --filter "ClassName"

# Run the application
dotnet run --project src/WindowScreenshot/WindowScreenshot.csproj
```

## Architecture Overview

This is a .NET 8.0 Windows Forms screenshot tool with a layered architecture:

### Project Structure

- `src/WindowScreenshot/` - Main application (WinForms)
- `tests/WindowScreenshot.Tests/` - xUnit test project

### Core Components (Layered Architecture)

**Data Layer:**
- `ScreenshotSettings.cs` - Configuration data structure with properties for target window, interval, and enabled state

**Service Layer:**
- `ConfigManager.cs` - JSON configuration persistence using System.Text.Json (stored at `%APPDATA%/CQHelper/config.json`)
- `WindowFinder.cs` - Window lookup by title (prefix matching) and class name using Win32 EnumWindows
- `WindowEnumerator.cs` - Enumerates visible desktop windows using Win32 API
- `WindowCapturer.cs` - Captures window screenshots using Graphics.CopyFromScreen
- `ScreenshotSaver.cs` - Generates timestamps and saves screenshots to `screenshot/` directory
- `TimedScreenshotService.cs` - Encapsulates timer logic for automated periodic screenshots

**Presentation Layer:**
- `MainForm.cs` / `MainForm.Designer.cs` - WinForms UI with window selector, preview panel, and timed screenshot controls

### Key Design Patterns

- **Separation of Concerns**: Screenshot logic split into Capturer (capture), Saver (file I/O), and Service (timer management)
- **Dependency Injection**: Services receive dependencies via constructor (e.g., `TimedScreenshotService` takes `WindowFinder`, `WindowCapturer`, `ScreenshotSaver`)
- **Event-driven**: `TimedScreenshotService` exposes `WindowNotFound` event for window lifecycle handling
- **Dispose Pattern**: `IDisposable` implemented for `WindowCapturer`, `TimedScreenshotService` for proper resource cleanup

### Data Flow

1. **Window Selection**: `WindowEnumerator.EnumWindows()` → populate `windowComboBox`
2. **Manual Screenshot**: User clicks button → minimize → delay 1s → `ScreenshotSaver.CaptureAndSave()` → show preview
3. **Timed Screenshot**: User starts timer → `TimedScreenshotService.Start()` → WinForms Timer ticks → capture → check window exists → event if missing
4. **Configuration**: On any UI change → `SaveConfiguration()` → `ConfigManager.Save()` → JSON at `%APPDATA%/CQHelper/config.json`

### Testing Strategy

- xUnit framework with project reference to main code
- `InternalsVisibleTo` configured for internal access
- Test classes mirror production classes (*Tests suffix)
- Integration tests verify end-to-end flows (config save/load, service start/stop)
- Tests use temp directories with Guid-based isolation and cleanup in `Dispose()`
