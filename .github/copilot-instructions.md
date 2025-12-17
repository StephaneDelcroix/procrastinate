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
   - **Also update README.md** if features or screenshots changed

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

9. **Plan Party & Send Invites**
   - Create calendar event
   - Send Slack/Teams/email announcement
   - Prepare celebratory GIFs

10. **Attend the Party** 🎉
    - Show up (optional: on time)
    - Celebrate the release
    - Take credit for everyone's work
    - Procrastinate on the next release

## iOS Simulator Interaction

Use the Appium agent in `automation/appium_agent.py` for all simulator interactions:

```bash
# Start Appium server first
appium --relaxed-security &

# Navigate to tabs
python3 automation/appium_agent.py --bundle-id org.reblochon.procrastinate --tap Stats

# Click buttons
python3 automation/appium_agent.py --bundle-id org.reblochon.procrastinate --tap ShuffleBtn

# Get element text
python3 automation/appium_agent.py --bundle-id org.reblochon.procrastinate --get-text ExcuseLabel

# Take screenshot
python3 automation/appium_agent.py --bundle-id org.reblochon.procrastinate --screenshot output.png

# List buttons on current page
python3 automation/appium_agent.py --bundle-id org.reblochon.procrastinate --list-buttons

# Get page source XML (for debugging)
python3 automation/appium_agent.py --bundle-id org.reblochon.procrastinate --page-source
```

See `automation/README.md` for more examples and element IDs.

## Taking App Screenshots for Collage

1. Build and run: `dotnet build -f net10.0-ios && xcrun simctl install booted bin/Debug/net10.0-ios/iossimulator-arm64/procrastinate.app && xcrun simctl launch booted org.reblochon.procrastinate`
2. Use Appium agent to navigate and screenshot each tab
3. Create collage with Python/Pillow

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
