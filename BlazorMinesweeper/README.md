# Blazor Minesweeper - Web Version

A browser-based Minesweeper game built with Blazor WebAssembly and .NET 9, deployed on GitHub Pages.

## 🎮 Play Now

Visit: **[https://chiblood.github.io/MAUI-Minesweeper/](https://chiblood.github.io/MAUI-Minesweeper/)**

## ✨ Features

- 💣 Classic Minesweeper gameplay
- 🎯 Three difficulty levels (Easy, Medium, Hard)
- ⏱️ Timer and flag counter
- 🎨 Responsive design for mobile and desktop
- 🚀 Progressive Web App (PWA) support
- 📱 Touch and mouse support (left-click to reveal, right-click to flag)

## 🛠️ Technology Stack

- **Framework**: Blazor WebAssembly
- **Language**: C# (.NET 9)
- **Hosting**: GitHub Pages
- **CI/CD**: GitHub Actions

## 🏗️ Project Structure

```
BlazorMinesweeper/
├── Pages/
│   └── Index.razor          # Main game page
├── Services/
│   └── GameBoardService.cs  # Game logic service
├── Shared/
│   └── MainLayout.razor     # Layout component
├── wwwroot/
│   ├── css/
│   │   └── app.css         # Styles
│   ├── index.html          # Entry point
│   ├── manifest.json       # PWA manifest
│   └── service-worker.js   # Service worker for offline support
└── Program.cs              # Application entry point

BlazorMinesweeper.Shared/
└── Models/
    └── CellModel.cs        # Cell data model
```

## 🚀 Local Development

### Prerequisites
- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)

### Run Locally

1. Clone the repository:
```bash
git clone https://github.com/Chiblood/MAUI-Minesweeper.git
cd MAUI-Minesweeper
```

2. Navigate to the Blazor project:
```bash
cd BlazorMinesweeper
```

3. Restore dependencies:
```bash
dotnet restore
```

4. Run the application:
```bash
dotnet run
```

5. Open your browser and navigate to `https://localhost:5001` (or the port shown in the terminal)

### Build for Production

```bash
dotnet publish -c Release -o ./publish
```

## 📦 Deployment to GitHub Pages

### Automatic Deployment (Recommended)

This project includes a GitHub Actions workflow that automatically builds and deploys to GitHub Pages on every push to `master` branch.

### Setup Steps:

1. **Enable GitHub Pages**:
   - Go to your repository on GitHub
   - Navigate to **Settings** → **Pages**
   - Under **Source**, select **GitHub Actions**

2. **Push Changes**:
   ```bash
   git add .
   git commit -m "Add Blazor WebAssembly project"
   git push origin master
   ```

3. **Monitor Deployment**:
   - Go to the **Actions** tab in your repository
   - Watch the workflow run
   - Once complete, your site will be live at: `https://yourusername.github.io/MAUI-Minesweeper/`

### Manual Deployment

If you prefer manual deployment:

1. Build the project:
```bash
dotnet publish BlazorMinesweeper/BlazorMinesweeper.csproj -c Release -o release
```

2. Update the base href in `release/wwwroot/index.html`:
```html
<base href="/MAUI-Minesweeper/" />
```

3. Copy `index.html` to `404.html`:
```bash
cp release/wwwroot/index.html release/wwwroot/404.html
```

4. Add `.nojekyll` file:
```bash
touch release/wwwroot/.nojekyll
```

5. Deploy the contents of `release/wwwroot/` to the `gh-pages` branch

## 🎯 How to Play

1. **Objective**: Reveal all cells that don't contain mines
2. **Controls**:
   - **Left Click / Tap**: Reveal a cell
   - **Right Click / Long Press**: Place or remove a flag
3. **Numbers**: Show how many mines are in adjacent cells
4. **Win**: Reveal all non-mine cells
5. **Lose**: Click on a mine

## 🔧 Configuration

### Change Repository Name

If your repository name is different from `MAUI-Minesweeper`, update the `base href` in:
1. `BlazorMinesweeper/wwwroot/index.html`
2. `BlazorMinesweeper/wwwroot/404.html`
3. `.github/workflows/deploy-blazor.yml`

Replace `/MAUI-Minesweeper/` with `/your-repo-name/`

### Difficulty Settings

You can modify difficulty settings in `BlazorMinesweeper/Pages/Index.razor`:

```csharp
case "Easy":
    Rows = 8; Cols = 8; MineCount = 10;
    break;
case "Medium":
    Rows = 16; Cols = 16; MineCount = 40;
    break;
case "Hard":
    Rows = 16; Cols = 30; MineCount = 99;
    break;
```

## 📝 Related Projects

- **MAUI Version**: This repository also contains a .NET MAUI version of Minesweeper for Windows, Android, iOS, and macOS in the root directory.

## 🤝 Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## 📄 License

This project is open source and available under the [MIT License](LICENSE).

## 🐛 Known Issues

- None at the moment! Report issues on the [GitHub Issues](https://github.com/Chiblood/MAUI-Minesweeper/issues) page.

## 🙏 Acknowledgments

- Built with [Blazor WebAssembly](https://dotnet.microsoft.com/apps/aspnet/web-apps/blazor)
- Deployed on [GitHub Pages](https://pages.github.com/)
- Inspired by the classic Windows Minesweeper game

---

**Made with ❤️ and C#**
