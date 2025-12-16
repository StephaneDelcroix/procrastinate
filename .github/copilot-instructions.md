# Copilot Instructions for Procrastinate

## TODO List Location

The TODO list is at `.github/TODO.md` - always check there when asked to "work on the TODO".

## Working on TODO Tasks

When asked to work on the TODO list (`.github/TODO.md`):

1. Read the TODO.md file
2. Work on each task **one by one**
3. After completing each task:
   - Build and verify the fix works
   - Regenerate the collage (if UI changed)
   - Commit with a descriptive message
   - Push to remote
   - Strike through the completed task in TODO.md (use `~~task~~`)
4. Move to the next task

## Release Checklist

When asked to "do a release" or "release checklist":

1. **Build & Test iOS**
   - `dotnet build -f net10.0-ios -c Release`
   - Deploy to iOS Simulator and verify app works
   - Test on physical device if available

2. **Build & Test Android**
   - `dotnet build -f net10.0-android -c Release`
   - Deploy to Android Emulator and verify app works

3. **Bump Version**
   - Update `ApplicationDisplayVersion` in `procrastinate.csproj` (e.g., 1.0.0 → 1.1.0)
   - Update `ApplicationVersion` (increment build number)

4. **Update Changelog**
   - Add new section in `CHANGELOG.md` with version and date
   - List all changes since last release (use git log)
   - Categories: Added, Changed, Fixed, Removed

5. **Update Collage**
   - Take fresh screenshots of all 4 tabs
   - Regenerate `screenshots_collage.png` with new version number

6. **Commit & Tag**
   - `git add -A && git commit -m "Release vX.Y.Z"`
   - `git tag vX.Y.Z`
   - `git push && git push --tags`

7. **Create GitHub Release**
   - Use `gh release create vX.Y.Z --title "vX.Y.Z" --notes-file CHANGELOG.md` or with specific notes
   - Attach collage image and any build artifacts

8. **Publish to TestFlight** (iOS)
   - Build for App Store: `dotnet publish -f net10.0-ios -c Release`
   - Upload to App Store Connect via Transporter or `xcrun altool`
   - Add release notes in TestFlight

9. **Plan Party & Send Invites** 🎉
   - Create calendar event
   - Send Slack/Teams/email announcement
   - Prepare celebratory GIFs

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
4. Create collage with Python/Pillow

## Collage Style Preferences

- **Layout**: Horizontal, all 4 pages in a row
- **Background**: Dark blue-gray (#1A202C)
- **Header**: 
  - Title "Procrastinate" in golden orange
  - Subtitle "The Ultimate Anti-Productivity App"
  - Multi-language tagline (English, French, Spanish, Portuguese, Dutch & Czech)
  - Version number (read from procrastinate.csproj ApplicationDisplayVersion)
- **Screenshots**: All 4 tabs (Tasks, Games, Excuses, Stats) scaled ~20%
  - **Language**: English
  - **Zalgo mode**: OFF
- **Footer**:
  - Feature labels under each screenshot
  - Highlight AI features: "Cloud AI (Groq) or On-Device AI (Apple Intelligence)"
  - Funny user testimonial with 5-star rating
- **No emojis** (they don't render properly in Pillow with system fonts)
