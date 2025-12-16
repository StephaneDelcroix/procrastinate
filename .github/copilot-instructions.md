# Copilot Instructions for Procrastinate

## iOS Simulator Interaction (macOS)

To click/tap in iOS Simulator when `simctl io tap` doesn't exist, use AppleScript:

```bash
# Get window position and size first
osascript -e '
tell application "Simulator" to activate
delay 0.3
tell application "System Events"
    tell process "Simulator"
        set windowPos to position of window 1
        set windowSize to size of window 1
        return {windowPos, windowSize}
    end tell
end tell
'

# Click at specific coordinates (absolute screen position)
osascript -e '
tell application "Simulator" to activate
delay 0.3
tell application "System Events"
    click at {windowX + relativeX, windowY + relativeY}
end tell
'
```

For tab bar navigation on iPhone 16 Pro (window ~456x972):
- Tab bar is ~60px from window bottom
- 4 tabs: centers at ~57, 171, 285, 399 from left edge of device area

Take screenshot: `xcrun simctl io <UDID> screenshot output.png`

## Taking App Screenshots for Collage

1. Build and run: `dotnet build -f net10.0-ios && xcrun simctl install <UDID> bin/Debug/net10.0-ios/iossimulator-arm64/procrastinate.app && xcrun simctl launch <UDID> com.companyname.procrastinate`
2. Use AppleScript clicks to navigate between tabs
3. Use `xcrun simctl io <UDID> screenshot` for each page
4. Create collage with Python/Pillow (see screenshots_collage.png)
