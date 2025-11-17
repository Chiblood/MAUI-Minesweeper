# ?? Deployment Checklist for GitHub Pages

Use this checklist to deploy your Blazor Minesweeper game to GitHub Pages.

## ? Pre-Deployment Checklist

### Local Testing
- [ ] Build succeeds: `dotnet build BlazorMinesweeper.sln`
- [ ] App runs locally: `dotnet run --project BlazorMinesweeper/BlazorMinesweeper.csproj`
- [ ] Game loads at `https://localhost:5001`
- [ ] All difficulty levels work
- [ ] Timer starts and counts correctly
- [ ] Flag counter works (decrements when placing flags)
- [ ] Left-click reveals cells
- [ ] Right-click places/removes flags
- [ ] Win condition works (reveal all non-mine cells)
- [ ] Lose condition works (click on mine)
- [ ] New Game button resets everything
- [ ] Responsive on mobile (test with browser dev tools)

### File Verification
- [ ] `BlazorMinesweeper.sln` exists
- [ ] `BlazorMinesweeper/BlazorMinesweeper.csproj` exists
- [ ] `BlazorMinesweeper.Shared/BlazorMinesweeper.Shared.csproj` exists
- [ ] `.github/workflows/deploy-blazor.yml` exists
- [ ] `BlazorMinesweeper/wwwroot/index.html` has correct base href
- [ ] `BlazorMinesweeper/wwwroot/404.html` has correct base href
- [ ] `BlazorMinesweeper/wwwroot/.nojekyll` file exists
- [ ] `Directory.Packages.props` includes Blazor packages

### Repository Configuration
- [ ] Repository name matches base href (default: `MAUI-Minesweeper`)
- [ ] All files committed to Git
- [ ] Pushed to GitHub (main or master branch)

## ?? GitHub Pages Setup

### 1. Enable GitHub Pages
- [ ] Go to repository **Settings**
- [ ] Navigate to **Pages** (left sidebar)
- [ ] Under **Source**, select **GitHub Actions**
- [ ] Click **Save**

### 2. Configure Workflow Permissions (if needed)
- [ ] Go to **Settings** ? **Actions** ? **General**
- [ ] Under **Workflow permissions**, select **Read and write permissions**
- [ ] Check **Allow GitHub Actions to create and approve pull requests**
- [ ] Click **Save**

## ?? Deployment

### Push to GitHub
```bash
# Stage all changes
git add .

# Commit
git commit -m "Add Blazor WebAssembly version with GitHub Pages deployment"

# Push
git push origin main  # or 'master' if that's your default branch
```

### Monitor Deployment
- [ ] Go to **Actions** tab in GitHub
- [ ] Click on "Deploy Blazor WASM to GitHub Pages" workflow
- [ ] Wait for workflow to complete (green checkmark, ~2-3 minutes)
- [ ] Check for any errors (red X)

## ? Post-Deployment Verification

### Access Your Game
- [ ] Open: `https://YOUR-USERNAME.github.io/MAUI-Minesweeper/`
  - Replace `YOUR-USERNAME` with your GitHub username
  - Replace `MAUI-Minesweeper` with your repository name (if different)

### Test All Features Online
- [ ] Page loads without 404 error
- [ ] Game board displays correctly
- [ ] All three difficulty levels work
- [ ] Timer starts on first click
- [ ] Flag counter updates correctly
- [ ] Cells reveal on left-click
- [ ] Flags toggle on right-click
- [ ] Numbers show correctly
- [ ] Mines appear as ??
- [ ] Flags appear as ??
- [ ] Win condition triggers
- [ ] Lose condition triggers
- [ ] New Game button works
- [ ] Difficulty selector changes board size

### Mobile Testing (Optional but Recommended)
- [ ] Test on actual mobile device or emulator
- [ ] Touch to reveal works
- [ ] Long-press to flag works
- [ ] Layout is responsive
- [ ] No horizontal scrolling issues
- [ ] All text is readable

### Browser Compatibility
Test in different browsers:
- [ ] Chrome/Edge (Chromium)
- [ ] Firefox
- [ ] Safari (Mac/iOS)
- [ ] Mobile browsers

## ?? Troubleshooting

### If you see 404 Not Found:
```bash
# Check base href in index.html
# Should be: <base href="/REPOSITORY-NAME/" />

# Update if needed
code BlazorMinesweeper/wwwroot/index.html
code BlazorMinesweeper/wwwroot/404.html

# Commit and push
git add .
git commit -m "Fix base href"
git push origin main
```

### If workflow fails:
- [ ] Check Actions log for specific error
- [ ] Verify all files are committed
- [ ] Try building locally: `dotnet build BlazorMinesweeper.sln`
- [ ] Check workflow permissions in repository settings

### If changes don't appear:
- [ ] Wait 2-3 minutes for deployment to complete
- [ ] Hard refresh browser (Ctrl+F5 or Cmd+Shift+R)
- [ ] Clear browser cache
- [ ] Check Actions tab to confirm deployment succeeded

## ?? Optional Enhancements

### Add Favicon and Icons
- [ ] Create `favicon.png` (any size, will be resized)
- [ ] Create `icon-192.png` (192x192 pixels)
- [ ] Create `icon-512.png` (512x512 pixels)
- [ ] Place in `BlazorMinesweeper/wwwroot/`
- [ ] Commit and push

### Add Custom Domain (Optional)
- [ ] Purchase a domain name
- [ ] Add CNAME file to `wwwroot/` with your domain
- [ ] Configure DNS settings with your domain provider
- [ ] Update base href to `/`
- [ ] See: https://docs.github.com/pages/configuring-a-custom-domain-for-your-github-pages-site

### Enable Analytics (Optional)
- [ ] Sign up for Google Analytics
- [ ] Add tracking code to `index.html`
- [ ] Commit and push

## ?? Success!

Once all checkboxes are complete, your game is live!

**Share your link:**
```
https://YOUR-USERNAME.github.io/MAUI-Minesweeper/
```

**Example (your actual URL):**
```
https://chiblood.github.io/MAUI-Minesweeper/
```

---

## ?? Additional Resources

- **[Deployment Guide](DEPLOYMENT_GUIDE.md)** - Detailed step-by-step instructions
- **[Blazor Conversion Summary](BLAZOR_CONVERSION_SUMMARY.md)** - What was created
- **[Root README](../README.md)** - Repository overview
- **[GitHub Pages Docs](https://docs.github.com/pages)** - Official documentation

---

**Last Updated:** 2025-01-17  
**Version:** 1.0  
**Repository:** https://github.com/Chiblood/MAUI-Minesweeper
