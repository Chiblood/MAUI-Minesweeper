# 💣 Minesweeper - Multi-Platform Game

A classic Minesweeper game implementation with both **native mobile/desktop apps** (.NET MAUI) and a **web browser version** (Blazor WebAssembly).

## 🎮 Play Online

**Web Version**: [https://chiblood.github.io/MAUI-Minesweeper/](https://chiblood.github.io/MAUI-Minesweeper/)

## 📦 Repository Structure

This repository contains **two separate solutions**:

```
MAUI-Minesweeper/
├── 📱 MAUI Minesweeper/          # Native app for Windows, Android, iOS, macOS
│   ├── MAUI Minesweeper.sln
│   ├── Models/
│   ├── ViewModels/
│   ├── Views/
│   └── Services/
│
├── 🌐 BlazorMinesweeper/         # Web browser version
│   ├── BlazorMinesweeper.sln
│   ├── BlazorMinesweeper/        # Main Blazor WebAssembly project
│   └── BlazorMinesweeper.Shared/ # Shared models
│
└── .github/workflows/            # CI/CD for GitHub Pages deployment
```

## 🚀 Getting Started

### Option 1: Play in Browser (Easiest)
Just visit: **[https://chiblood.github.io/MAUI-Minesweeper/](https://chiblood.github.io/MAUI-Minesweeper/)**

### Option 2: Run Native App Locally

**Requirements:**
- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- Visual Studio 2022 (for Windows/Android development)
- Xcode (for iOS/macOS development on Mac)

**Steps:**
```bash
# Clone the repository
git clone https://github.com/Chiblood/MAUI-Minesweeper.git
cd MAUI-Minesweeper

# Open the MAUI solution
# Windows: Double-click "MAUI Minesweeper.sln"
# Or use command line:
dotnet build "MAUI Minesweeper.csproj"
```

Then select your target platform (Windows, Android, iOS, macOS) and run.

### Option 3: Run Blazor Version Locally

**Requirements:**
- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)

**Steps:**
```bash
# Clone the repository
git clone https://github.com/Chiblood/MAUI-Minesweeper.git
cd MAUI-Minesweeper

# Navigate to Blazor project
cd BlazorMinesweeper

# Run the app
dotnet run --project BlazorMinesweeper/BlazorMinesweeper.csproj
```

Then open your browser to `https://localhost:5001`

## ✨ Features

### Common Features (Both Versions)
- 💣 Classic Minesweeper gameplay
- 🎯 Three difficulty levels (Easy, Medium, Hard)
- ⏱️ Game timer
- 🚩 Flag counter
- 🏆 Win/Loss detection
- 🎨 Color-coded numbers

### MAUI App Exclusive
- ✅ Native performance
- ✅ Works offline
- ✅ Platform-specific optimizations
- ✅ Mobile touch gestures

### Blazor Web Exclusive
- ✅ No installation required
- ✅ Cross-platform (any device with a browser)
- ✅ PWA support (install as app)
- ✅ Automatic updates via GitHub Pages

## 🎯 How to Play

1. **Objective**: Reveal all cells that don't contain mines
2. **Controls**:
   - **Left Click / Tap**: Reveal a cell
   - **Right Click / Long Press**: Place or remove a flag
3. **Numbers**: Show how many mines are adjacent to that cell
4. **Win**: Reveal all non-mine cells
5. **Lose**: Click on a mine

## 🛠️ Technology Stack

### MAUI Version
- **Framework**: .NET MAUI (.NET 9)
- **Language**: C# 13.0
- **Architecture**: MVVM (Model-View-ViewModel)
- **UI**: XAML with CommunityToolkit.Mvvm
- **Platforms**: Windows, Android, iOS, macOS Catalyst

### Blazor Version
- **Framework**: Blazor WebAssembly (.NET 9)
- **Language**: C# 13.0
- **UI**: Razor Components
- **Deployment**: GitHub Pages
- **CI/CD**: GitHub Actions

## 📂 Solution Structure

### Why Two Solutions?

This repository uses **two separate solution files** for clean separation:

1. **MAUI Minesweeper.sln** - For native app development
   - Includes platform-specific projects
   - Optimized for Visual Studio 2022

2. **BlazorMinesweeper.sln** - For web development
   - Includes WebAssembly project
   - Faster build times for web-only changes

**Both share the same Git repository** for:
- ✅ Unified issue tracking
- ✅ Single deployment workflow
- ✅ Shared documentation

## 🚀 Deployment

### Blazor Web Version (GitHub Pages)

Automatic deployment is configured via GitHub Actions:
- Every push to `master` branch triggers deployment
- Build and publish happens automatically
- Site updates in ~2 minutes

**Manual Deployment:**
```bash
cd BlazorMinesweeper
dotnet publish -c Release -o ./publish
# Deploy contents of publish/wwwroot to GitHub Pages
```

### MAUI Native Apps

For app store distribution:
- **Microsoft Store**: Package as MSIX (Windows)
- **Google Play**: Generate APK/AAB (Android)
- **App Store**: Build IPA via Xcode (iOS)

See [MAUI deployment docs](https://learn.microsoft.com/dotnet/maui/deployment/) for details.

## 📖 Documentation

- **[Blazor README](BlazorMinesweeper/README.md)** - Web version setup and deployment
- **[Deployment Guide](docs/DEPLOYMENT_GUIDE.md)** - Step-by-step deployment instructions
- **[Deployment Checklist](docs/DEPLOYMENT_CHECKLIST.md)** - Quick deployment checklist
- **[Copilot Instructions](.github/copilot-instructions.md)** - Development guidelines

## 🤝 Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

### Development Guidelines
1. Follow MVVM pattern for MAUI code
2. Use Razor components for Blazor code
3. Maintain separation between MAUI and Blazor projects
4. Test on multiple platforms before submitting
5. Update documentation for new features

## 🐛 Known Issues

- None at the moment! Report issues on the [GitHub Issues](https://github.com/Chiblood/MAUI-Minesweeper/issues) page.

## 📄 License

This project is open source and available under the [MIT License](LICENSE).

## 🙏 Acknowledgments

- Built with [.NET MAUI](https://dotnet.microsoft.com/apps/maui) and [Blazor](https://dotnet.microsoft.com/apps/aspnet/web-apps/blazor)
- Deployed on [GitHub Pages](https://pages.github.com/)
- Inspired by the classic Windows Minesweeper game

## 📱 Screenshots

### Web Version (Blazor)
*Coming soon - add screenshots here*

### Mobile/Desktop Version (MAUI)
*Coming soon - add screenshots here*

---

**Choose Your Platform:**
- 🌐 [Play in Browser](https://chiblood.github.io/MAUI-Minesweeper/) - No installation needed
- 📱 Download Native App - Better performance, offline play

**Made with ❤️ and C#**
