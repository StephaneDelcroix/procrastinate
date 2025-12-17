# Changelog

All notable changes to Procrastinate will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [4.0.0] - 2025-12-17

### Added

- **TicTacToe AI Integration** - TicTacToe now uses real AI (Cloud AI via Groq or On-Device Apple Intelligence) to play against you
- **AI Engine Info Label** - Shows which AI engine was used and the response for each move
- **Minesweeper Flag Mode** - Long-press to flag suspected bombs with 🚩
- **Click Counter** - Stats page now tracks total button clicks across the app
- **Memory Game Emojis** - Memory match now uses fun emoji (🐶🐱🦊🐸🍎🍊🍇🍓) instead of letters

### Changed

- **Excuses Language Detection** - When using "System Default" language, excuses are now generated in the actual system language instead of defaulting to English
- **Copilot Instructions** - Updated to use Appium agent for iOS automation

### Fixed

- **Chart Bar Corners** - Fixed visual glitch in stats chart due to rounded corners
- **Deprecated Groq Models** - Removed deprecated models from the selection dropdown

## [3.0.0] - 2025-12-17

### Added

- **Nord Theme** - Complete redesign using the beautiful Nord color palette
- **Mac Catalyst Flyout Navigation** - Sidebar navigation on macOS while keeping bottom tabs on iOS
- **XAML Source Generation (XSG)** - Enabled for better compile-time performance
- **Reusable Styles** - Extracted common XAML patterns into Styles.xaml

### Changed

- **Theme System** - Now uses `AppThemeBinding` instead of runtime color overwriting for cleaner theme switching
- **Color System** - All colors now use official Nord naming (Nord0-Nord15)
- **Excuses Page** - Generator info label moved closer to excuse box; excuses never use zalgo effect
- **Settings Page** - Improved layout with Light Theme label restored
- **Splash Screen** - Updated to Nord Polar Night background (#2E3440)
- **App Icon** - Updated to Nord Frost accent (#5E81AC)

### Fixed

- **Share Button** - Now visible on Mac Catalyst
- **Game Text Readability** - "Enter your guess" placeholder now readable in dark theme
- **Stats Page** - Fixed crash caused by missing color references

### Developer

- Updated to .NET MAUI 10.0.20
- Cleaned up Styles.xaml - removed unused styles
- Moved AppStrings.cs from Resources/Strings to Services
- All `DynamicResource` color references replaced with `AppThemeBinding`

---

## [2.0.0] - 2025-12-16

### Added

- **On-Device AI Excuses** - Generate excuses using Apple Intelligence (Foundation Models) on iOS 26+
- **About Section** - Credits, app description, and GitHub link in Settings
- **System Default Language** - App now uses system locale by default, with manual override option
- **AI Model Picker** - Dropdown to select Groq AI model instead of text entry
- **Random Task Variations** - "Take a break" now randomly shows as "Take a nap", "Grab a coffee", "Stretch a bit", or "Stare at the ceiling"

### Changed

- **Whack-a-Mole** - Now uses emojis: 🕳️ (hole), 🐭 (mole), 💥 (hit)
- **Snake Game** - Snake now wraps around walls instead of dying (only dies when hitting itself)
- **Zalgo Mode** - Renamed to "Zalgo Randomly" - now applies chaos 15% of the time even when "off"
- **Settings UI** - Theme Preview moved into Accessibility section for cleaner layout
- **Cloud AI Errors** - Now shows actual error message instead of silently falling back to random

### Fixed

- Cloud AI no longer silently falls back to random generator on errors

### Developer

- Added release checklist to copilot instructions
- Updated collage to show version number and AI features

---

## [1.0.0] - 2025-12-16

### Added

**Tasks Page**
- Pre-populated list of important tasks you'll definitely do later
- Satisfying checkboxes (that secretly add more tasks when you complete them)
- Encouraging popup messages when you try to be productive
- Click anywhere on task to toggle - maximum laziness support

**Excuses Page**
- Random excuse generator with creative combinations
- AI-powered excuse generation via Groq Cloud API (Llama 3)
- Copy to clipboard functionality for quick excuse sharing
- Configurable excuse generation mode (Random vs AI)

**Mini-Games**
- Click Speed Test - prove your fingers work even if your motivation doesn't
- Simon Says - memory game with colored buttons and sound effects
- High score tracking for competitive procrastinators

**Stats Page**
- Procrastination statistics tracking
- Tasks completed vs tasks added metrics
- Excuses generated counter
- Game high scores display
- AI API usage tracking

**Settings**
- Multi-language support: English, French, Spanish, Portuguese, Dutch, Czech
- Zalgo mode for the truly chaotic procrastinator
- Light/Dark theme toggle
- Accessibility options
- Excuse generator mode selection (Random/Groq AI)
- Groq API key configuration

**General**
- Beautiful UI with dynamic theming
- Cross-platform support (iOS, Android, macOS, Windows)
- Persistent settings and statistics
- Tab-based navigation

### Work Successfully Avoided

- Actually being productive
- Meeting deadlines
- Responding to emails
- Finishing that important project
- Going to the gym
- Calling mom back
- Doing laundry
- Filing taxes (it's only December, plenty of time)
- Learning that new skill
- Organizing the desk
- Reading that book everyone recommended
- Starting the diet
- Cleaning the apartment
- Updating the resume
- Any form of adulting

### Technical Highlights

- Built with .NET MAUI for true cross-platform procrastination
- MVVM architecture (mostly)
- XAML-based UI with DynamicResource for theme switching
- Groq API integration for AI-powered excuses
- Plugin.Maui.Audio for Simon game sounds
- Localization via .resx resource files

### Known Issues

- You might accidentally become slightly productive
- Zalgo mode may summon eldritch horrors
- AI excuses are suspiciously convincing

---

*"Why do today what you can put off until tomorrow?"* - The Procrastinate Team
