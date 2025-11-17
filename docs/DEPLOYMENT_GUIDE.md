# ?? Quick Start Guide - Deploying to GitHub Pages

Follow these steps to deploy your Blazor Minesweeper game to GitHub Pages.

## ? Prerequisites

- [x] Git installed
- [x] GitHub account
- [x] Repository pushed to GitHub
- [x] .NET 9 SDK installed (for local testing)

## ?? Step-by-Step Deployment

### 1. Enable GitHub Pages

1. Go to your repository on GitHub: `https://github.com/Chiblood/MAUI-Minesweeper`
2. Click **Settings** (top navigation)
3. Scroll down to **Pages** (left sidebar)
4. Under **Source**, select **GitHub Actions**
5. Click **Save**

### 2. Verify Repository Name

Your GitHub Actions workflow is configured for repository name: `MAUI-Minesweeper`

**If your repository has a different name**, update these files:

#### File 1: `BlazorMinesweeper/wwwroot/index.html`
```html
<base href="/YOUR-REPO-NAME/" />
```

#### File 2: `BlazorMinesweeper/wwwroot/404.html`
```html
<base href="/YOUR-REPO-NAME/" />
```

#### File 3: `.github/workflows/deploy-blazor.yml`
```yaml
- name: Change base tag in index.html
  run: sed -i 's/<base href="\/" \/>/<base href="\/YOUR-REPO-NAME\/" \/>/g' release/wwwroot/index.html
```

### 3. Push to GitHub

```bash
# Stage all changes
git add .

# Commit changes
git commit -m "Add Blazor WebAssembly version with GitHub Pages deployment"

# Push to GitHub (main or master branch)
git push origin main
```

### 4. Monitor Deployment

1. Go to the **Actions** tab in your repository
2. You'll see a workflow named "Deploy Blazor WASM to GitHub Pages"
3. Click on it to see the progress
4. Wait for the workflow to complete (usually 2-3 minutes)

### 5. Access Your Game

Once deployment is complete, your game will be live at:

**https://chiblood.github.io/MAUI-Minesweeper/**

*(Replace with your username/repo if different)*

## ?? Test Locally First

Before deploying, test the Blazor version locally:

```bash
# Navigate to Blazor project
cd BlazorMinesweeper

# Run locally
dotnet run --project BlazorMinesweeper/BlazorMinesweeper.csproj

# Open browser to:
# https://localhost:5001
```

## ?? Troubleshooting

### Issue: 404 Not Found after deployment

**Solution:** Make sure the `base href` in `index.html` matches your repository name:
```html
<base href="/MAUI-Minesweeper/" />
```

### Issue: Workflow fails with "permission denied"

**Solution:** 
1. Go to **Settings** ? **Actions** ? **General**
2. Scroll to **Workflow permissions**
3. Select **Read and write permissions**
4. Check **Allow GitHub Actions to create and approve pull requests**
5. Click **Save**

### Issue: Changes not appearing on site

**Solution:**
1. Clear browser cache (Ctrl+F5 or Cmd+Shift+R)
2. Wait a few minutes for CDN to update
3. Check if workflow completed successfully in Actions tab

### Issue: Build fails in GitHub Actions

**Solution:**
1. Check the Actions log for specific error
2. Verify all files are committed and pushed
3. Ensure `Directory.Packages.props` includes Blazor packages
4. Try building locally first: `dotnet build BlazorMinesweeper.sln`

## ?? Updating Your Game

To deploy updates:

```bash
# Make your changes to Blazor files

# Commit and push
git add .
git commit -m "Description of changes"
git push origin main

# GitHub Actions will automatically redeploy
```

## ?? Next Steps

- [ ] Add favicon and icons (icon-192.png, icon-512.png)
- [ ] Test on mobile devices
- [ ] Customize styles in `wwwroot/css/app.css`
- [ ] Add analytics (optional)
- [ ] Set up custom domain (optional)

## ?? Useful Links

- [GitHub Pages Documentation](https://docs.github.com/pages)
- [Blazor WebAssembly Deployment](https://learn.microsoft.com/aspnet/core/blazor/host-and-deploy/webassembly)
- [GitHub Actions for .NET](https://docs.github.com/actions/automating-builds-and-tests/building-and-testing-net)

## ? Success Checklist

After successful deployment, verify:

- [ ] Game loads at your GitHub Pages URL
- [ ] All three difficulty levels work
- [ ] Timer counts up during gameplay
- [ ] Flag counter decrements when placing flags
- [ ] Right-click to flag works
- [ ] Win condition works (reveal all non-mine cells)
- [ ] Lose condition works (click a mine)
- [ ] New Game button works
- [ ] Game is responsive on mobile

---

**?? Congratulations!** Your Minesweeper game is now live on GitHub Pages!

Share your link:
```
https://chiblood.github.io/MAUI-Minesweeper/
```
