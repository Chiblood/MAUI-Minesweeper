# MAUI Minesweeper - Troubleshooting Guide

## Table of Contents
1. [Common Build Errors](#common-build-errors)
2. [Runtime Errors](#runtime-errors)
3. [XAML Binding Issues](#xaml-binding-issues)
4. [Threading Issues](#threading-issues)
5. [Platform-Specific Issues](#platform-specific-issues)
6. [Performance Issues](#performance-issues)
7. [Documentation Issues](#documentation-issues)
8. [Debugging Tips](#debugging-tips)

---

## Common Build Errors

### Error: "Mismatch between specified x:DataType and current binding context" (Multiple Contexts)

**Symptom**: Multiple binding diagnostics errors showing mismatches between `GameViewModel` and `CellModel`, and between `GameViewModel` and `GamePage`

**Cause**: When using compiled bindings with `x:DataType` at the page level, the DataTemplate inside a CollectionView inherits the parent's type context, but the actual binding context is different (the collection item). This causes conflicts when trying to use both item properties and RelativeSource bindings to access parent ViewModel commands.

**Solution**: Explicitly opt out of compiled bindings in the DataTemplate using `x:DataType="{x:Null}"`:

```xaml
<!-- ContentPage has compiled bindings for GameViewModel -->
<ContentPage x:DataType="viewmodels:GameViewModel">
    <!-- ... -->
    
    <CollectionView ItemsSource="{Binding Cells}">
        <!-- Opt out of compiled bindings in DataTemplate to allow mixed contexts -->
        <CollectionView.ItemTemplate>
            <DataTemplate x:DataType="{x:Null}">
                <Border>
                    <Grid BackgroundColor="{Binding IsRevealed, Converter={StaticResource CellBackgroundConverter}}">
                        <Grid.GestureRecognizers>
                            <!-- Can now use RelativeSource to access parent ViewModel -->
                            <TapGestureRecognizer 
                                Command="{Binding Source={RelativeSource AncestorType={x:Type ContentPage}}, 
                                                  Path=BindingContext.CellTappedCommand}"
                                CommandParameter="{Binding .}" />
                        </Grid.GestureRecognizers>
                        
                        <!-- Can also bind to CellModel properties -->
                        <Label Text="{Binding DisplayText}" />
                    </Grid>
                </Border>
            </DataTemplate>
        </CollectionView.ItemTemplate>
    </CollectionView>
</ContentPage>
```

**Why this works**:
- `x:DataType="{x:Null}"` tells the XAML compiler to skip compiled bindings for that DataTemplate
- Runtime bindings are used instead, which can handle:
  - Binding to `CellModel` properties (the item context)
  - `RelativeSource` bindings to navigate up the visual tree to access `GameViewModel` commands
- The page-level compiled bindings remain active for the header/footer sections

**Trade-offs**:
- **Pros**: Fixes binding errors, allows flexible binding paths, required for mixed contexts
- **Cons**: Slightly slower performance for cell bindings (runtime vs. compiled), no compile-time binding validation inside the DataTemplate

---

### Error: "Mismatch between specified x:DataType and current binding context"

**Symptom**: 384 binding diagnostics warnings in Output window

**Cause**: Incorrect `x:DataType` in DataTemplate (set to `GameViewModel` when actual context is `CellModel`)

**Solution**:
```xaml
<!-- WRONG: x:DataType on TapGestureRecognizer -->
<TapGestureRecognizer 
    Command="{Binding CellTappedCommand}"
    x:DataType="viewmodels:GameViewModel"/>

<!-- CORRECT: Use RelativeSource to access ContentPage's BindingContext -->
<TapGestureRecognizer 
    Command="{Binding Source={RelativeSource AncestorType={x:Type ContentPage}}, 
                      Path=BindingContext.CellTappedCommand}"
    CommandParameter="{Binding .}" />
```

**Rule**: Inside a CollectionView DataTemplate, the binding context is the item (`CellModel`), NOT the page's ViewModel.

---

### Error: "The name 'InitializeComponent' does not exist in the current context"

**Symptom**: Build fails on XAML code-behind files

**Cause**: XAML file not compiled properly or `x:Class` mismatch

**Solution**:
1. Check `x:Class` attribute matches namespace and class name:
   ```xaml
   x:Class="MAUI_Minesweeper.Views.GamePage"
   ```
2. Clean and rebuild:
   ```
   dotnet clean
   dotnet build
   ```
3. Restart Visual Studio if issue persists

---

### Error: "Type or namespace name 'CommunityToolkit' could not be found"

**Symptom**: Build fails with missing toolkit references

**Cause**: NuGet package not installed or corrupted

**Solution**:
```bash
dotnet restore
dotnet add package CommunityToolkit.Mvvm
```

Or in Visual Studio:
- Tools ? NuGet Package Manager ? Manage NuGet Packages for Solution
- Install `CommunityToolkit.Mvvm`

---

### Error: "CS0246: The type or namespace name 'ObservableObject' could not be found"

**Symptom**: Models/ViewModels fail to build

**Cause**: Missing `using CommunityToolkit.Mvvm.ComponentModel;`

**Solution**:
```csharp
using CommunityToolkit.Mvvm.ComponentModel;

public partial class GameViewModel : ObservableObject
{
    // ...
}
```

---

## Runtime Errors

### Error: "System.InvalidOperationException: The CollectionView's ItemsSource must be set to null when resetting ItemsLayout"

**Symptom**: App crashes when starting new game

**Cause**: CollectionView doesn't handle `Cells.Clear()` + re-adding items well

**Solution**: Replace entire `Cells` collection instead of clearing:
```csharp
// WRONG
Cells.Clear();
foreach (var cell in newCells)
    Cells.Add(cell);

// CORRECT
Cells = new ObservableCollection<CellModel>(newCells);
OnPropertyChanged(nameof(Cells));
```

---

### Error: "System.NullReferenceException: Object reference not set to an instance of an object"

**Symptom**: App crashes when tapping cell or flagging

**Cause**: `CellModel` parameter is null or `_board` is null

**Solution**: Add null checks:
```csharp
private void OnCellTapped(CellModel? cell)
{
    if (cell == null || _isGameOver || cell.IsFlagged || cell.IsRevealed)
        return;
    
    // Continue processing...
}
```

**Debug Steps**:
1. Check if CommandParameter is set: `CommandParameter="{Binding .}"`
2. Verify cell is not null in debugger
3. Check `_board` is initialized before accessing

---

### Error: "System.InvalidOperationException: ScrollIntoView called on an element that is not part of the visual tree"

**Symptom**: Random crashes during board generation or UI updates

**Cause**: CollectionView trying to scroll to item during collection update

**Solution**: Replace entire `Cells` collection (see above) and ensure updates happen on main thread:
```csharp
Microsoft.Maui.ApplicationModel.MainThread.BeginInvokeOnMainThread(() =>
{
    Cells = newCells;
    OnPropertyChanged(nameof(Cells));
});
```

---

### Error: Timer keeps running after game ends

**Symptom**: `ElapsedTime` continues incrementing after win/loss

**Cause**: Timer not disposed properly

**Solution**:
```csharp
private void HandleGameOver(bool won)
{
    _isGameOver = true;
    _gameTimer?.Dispose(); // MUST dispose timer
    GameStatus = won ? "Won!" : "Lost!";
    // ...
}
```

---

## XAML Binding Issues

### Issue: Cell text and flags not appearing after click

**Symptom**: When clicking cells or flagging them, the cell background changes but no text (numbers, flags ??, or mines ??) appears.

**Root Cause**: The `DisplayText` property in `CellModel` is a **computed property** that depends on multiple backing fields (`_isRevealed`, `_isFlagged`, `_isMine`, `_neighboringMines`). When these backing fields change, `DisplayText` doesn't automatically notify the UI to re-evaluate the computed value.

**Why Background Color Works But Text Doesn't**:
- Background color binds directly to `IsRevealed` property: `BackgroundColor="{Binding IsRevealed, Converter={StaticResource CellBackgroundConverter}}`
- When `IsRevealed` changes, `SetProperty` fires `PropertyChanged("IsRevealed")`, converter re-evaluates ?
- Text binds to `DisplayText`: `Text="{Binding DisplayText}"""
- When `IsRevealed` changes, `DisplayText` is NOT notified to re-evaluate ?

**The Click Flow**:
1. User taps cell ? `TapGestureRecognizer` fires
2. `GameViewModel.OnCellTapped(cell)` runs
3. `GameBoardService.RevealCell(cell)` sets `cell.IsRevealed = true`
4. `CellModel.IsRevealed` setter calls `SetProperty(ref _isRevealed, value)`
5. PropertyChanged("IsRevealed") fires ? **background updates via converter**
6. PropertyChanged("DisplayText") **NEVER fires** ? **text stays blank**

**Solution**: Manually notify `DisplayText` when any of its dependencies change:

```csharp
public partial class CellModel : ObservableObject
{
    public bool IsRevealed
    {
        get => _isRevealed;
        set
        {
            if (SetProperty(ref _isRevealed, value))
            {
                OnPropertyChanged(nameof(DisplayText)); // ? Notify DisplayText
            }
        }
    }
    private bool _isRevealed;

    public bool IsFlagged
    {
        get => _isFlagged;
        set
        {
            if (SetProperty(ref _isFlagged, value))
            {
                OnPropertyChanged(nameof(DisplayText)); // ? Notify DisplayText
            }
        }
    }
    private bool _isFlagged;

    public int NeighboringMines
    {
        get => _neighboringMines;
        set
        {
            if (SetProperty(ref _neighboringMines, value))
            {
                OnPropertyChanged(nameof(DisplayText)); // ? Notify DisplayText
            }
        }
    }
    private int _neighboringMines;

    public bool IsMine
    {
        get => _isMine;
        set
        {
            if (SetProperty(ref _isMine, value))
            {
                OnPropertyChanged(nameof(DisplayText)); // ? Notify DisplayText
            }
        }
    }
    private bool _isMine;

    // Computed property - depends on above fields
    public string DisplayText => _isMine && _isRevealed ? "??" : 
                                 _isFlagged ? "??" : 
                                 _isRevealed && _neighboringMines > 0 
                                    ? _neighboringMines.ToString() : "";
}
```

**Key Pattern**: When a computed property depends on other properties, you must manually notify it when dependencies change. The pattern is:
```csharp
if (SetProperty(ref _backingField, value))
{
    OnPropertyChanged(nameof(ComputedProperty));
}
```

**Alternative Approach** (if using CommunityToolkit.Mvvm source generators):
```csharp
[ObservableProperty]
[NotifyPropertyChangedFor(nameof(DisplayText))]
private bool _isRevealed;

[ObservableProperty]
[NotifyPropertyChangedFor(nameof(DisplayText))]
private bool _isFlagged;

// ... etc
```

This uses attributes to automatically generate the notification code, but requires partial classes and source generators.

---

### Issue: Cell commands not firing

**Symptom**: Tapping cells does nothing

**Cause 1**: Incorrect binding path or missing RelativeSource

**Solution**:
```xaml
<!-- CORRECT: Use RelativeSource to climb to ContentPage -->
Command="{Binding Source={RelativeSource AncestorType={x:Type ContentPage}}, 
                  Path=BindingContext.CellTappedCommand}"
```

**Cause 2**: CommandParameter not set

**Solution**:
```xaml
CommandParameter="{Binding .}"
```

---

### Issue: Cell colors not updating

**Symptom**: Cells stay same color when revealed

**Cause**: Converter not registered or incorrect binding

**Solution**:
1. Verify converter is in App.xaml resources:
   ```xaml
   <Application.Resources>
       <converters:CellBackgroundConverter x:Key="CellBackgroundConverter"/>
   </Application.Resources>
   ```

2. Use StaticResource (not DynamicResource):
   ```xaml
   BackgroundColor="{Binding IsRevealed, 
                             Converter={StaticResource CellBackgroundConverter}}"
   ```

3. Check converter namespace:
   ```xaml
   xmlns:converters="clr-namespace:MAUI_Minesweeper.Converters"
   ```

---

### Issue: DisplayText not showing emojis

**Symptom**: Cells show blank instead of ?? or ??

**Cause**: Font doesn't support emojis (rare on modern platforms)

**Solution**:
1. Check `DisplayText` property implementation:
   ```csharp
   public string DisplayText => _isMine && _isRevealed ? "??" : 
                                _isFlagged ? "??" : 
                                _isRevealed && _neighboringMines > 0 
                                    ? _neighboringMines.ToString() : "";
   ```

2. Ensure Label FontFamily supports emojis (default should work)

3. Test on actual device (emulator may have font issues)

---

### Issue: GridItemsLayout not respecting Span

**Symptom**: Board doesn't display as grid, items stack vertically

**Cause**: `Span` binding not working or `Cols` property not updated

**Solution**:
```xaml
<GridItemsLayout Orientation="Vertical" 
                 Span="{Binding Cols}" />
```

Ensure `Cols` property exists in ViewModel:
```csharp
public int Cols => _cols;
```

And notify when changed:
```csharp
OnPropertyChanged(nameof(Cols));
```

---

## Threading Issues

### Issue: "System.InvalidOperationException: UIKit Consistency error: view is not part of hierarchy"

**Symptom**: Crash on iOS when updating UI from background thread

**Cause**: Timer callback updating UI property on thread pool thread

**Solution**: Always use MainThread for UI updates:
```csharp
_gameTimer = new System.Threading.Timer(_ =>
{
    if (!_isGameOver)
    {
        try
        {
            Microsoft.Maui.ApplicationModel.MainThread.BeginInvokeOnMainThread(() =>
            {
                ElapsedTime++;
            });
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Timer error: {ex.Message}");
        }
    }
}, null, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1));
```

---

### Issue: Checking if on main thread

**Symptom**: Need to verify thread context for debugging

**Solution**:
```csharp
bool isMainThread = Microsoft.Maui.ApplicationModel.MainThread.IsMainThread;
int threadId = Environment.CurrentManagedThreadId;

System.Diagnostics.Debug.WriteLine(
    $"IsMainThread: {isMainThread}, Thread ID: {threadId}");
```

---

### Issue: Race condition when resetting board

**Symptom**: Occasional crashes when starting new game rapidly

**Solution**: Ensure all updates happen atomically on main thread:
```csharp
private void StartNewGame()
{
    _isGameOver = true; // Immediately stop processing
    _gameTimer?.Dispose();
    
    // Generate board synchronously
    _board = GameBoardService.GenerateBoard(_rows, _cols, _mineCount);
    var newCells = new ObservableCollection<CellModel>();
    for (int row = 0; row < _rows; row++)
    {
        for (int col = 0; col < _cols; col++)
        {
            newCells.Add(_board[row, col]);
        }
    }
    
    // Update UI on main thread atomically
    Microsoft.Maui.ApplicationModel.MainThread.BeginInvokeOnMainThread(() =>
    {
        Cells = newCells;
        OnPropertyChanged(nameof(Cells));
        OnPropertyChanged(nameof(Cols));
        _isGameOver = false; // Re-enable input
    });
    
    // Restart timer
    _gameTimer = new System.Threading.Timer(...);
}
```

---

## Platform-Specific Issues

### Windows

**Issue**: Left-click vs right-click not working properly ? SOLVED

**Symptom**: `TappedEventArgs.Buttons` returns `0` instead of `Primary` or `Secondary`, making it impossible to distinguish between left-click and right-click in a single event handler. Both clicks might trigger the same action or fail to work correctly.

**Root Cause**: In .NET MAUI 9 on Windows, when using a single `TapGestureRecognizer` (with or without `Buttons` property set), the `TappedEventArgs.Buttons` property incorrectly returns `0` instead of the actual button that was pressed.

**Solution**: Use **two separate `TapGestureRecognizer` instances** with explicit button filters:

**XAML** (Working Solution):
```xaml
<Grid BackgroundColor="{Binding IsRevealed, Converter={StaticResource CellBackgroundConverter}}">
    <Grid.GestureRecognizers>
        <!-- Separate recognizers for left and right clicks -->
        <TapGestureRecognizer 
            Tapped="OnCellLeftClicked"
            Buttons="Primary" />
        <TapGestureRecognizer 
            Tapped="OnCellRightClicked"
            Buttons="Secondary" />
    </Grid.GestureRecognizers>
    <!-- Cell Content -->
    <Label Text="{Binding DisplayText}" />
</Grid>
```

**Code-behind** (Working Solution):
```csharp
private void OnCellLeftClicked(object sender, TappedEventArgs e)
{
    if (sender is Grid grid && grid.BindingContext is CellModel cell)
    {
        // Left-click: Reveal cell
        _viewModel.CellTappedCommand.Execute(cell);
    }
}

private void OnCellRightClicked(object sender, TappedEventArgs e)
{
    if (sender is Grid grid && grid.BindingContext is CellModel cell)
    {
        // Right-click: Toggle flag
        _viewModel.CellFlaggedCommand.Execute(cell);
    }
}
```

**Why This Works**:
- Each `TapGestureRecognizer` has an explicit `Buttons` filter (`Primary` or `Secondary`)
- The framework handles button detection internally and only fires the appropriate handler
- No need to check `e.Buttons` in code - the handler that fires determines which button was pressed
- Completely avoids the `Buttons: 0` issue

**Verified Behavior**:
- ? **Left-click** ? Only `OnCellLeftClicked` fires ? Cell reveals
- ? **Right-click** ? Only `OnCellRightClicked` fires ? Flag toggles ??
- ? No ambiguity or `Buttons: 0` errors

---

**Approaches That DON'T Work**:

**? Single TapGestureRecognizer with button detection in code**:
```xaml
<!-- DOESN'T WORK in .NET MAUI 9 on Windows -->
<TapGestureRecognizer 
    Tapped="OnCellTapped"
    Buttons="Primary, Secondary"/>
```
```csharp
// e.Buttons returns 0 instead of Primary/Secondary
private void OnCellTapped(object sender, TappedEventArgs e)
{
    if (e.Buttons == ButtonsMask.Secondary) // Never true
    {
        // Right-click handling
    }
    else if (e.Buttons == ButtonsMask.Primary) // Never true
    {
        // Left-click handling
    }
}
```

**? PointerGestureRecognizer approach**:
```csharp
// DOESN'T WORK: PointerEventArgs doesn't have Button property in .NET 9 MAUI
private void OnCellPointerPressed(object sender, PointerEventArgs e)
{
    if (e.Button == ButtonsMask.Secondary) // COMPILE ERROR
    {
        // ...
    }
}
```

**? Single TapGestureRecognizer without Buttons property**:
```xaml
<!-- DOESN'T WORK: e.Buttons returns 0 -->
<TapGestureRecognizer Tapped="OnCellTapped" />
```

---

**Known Limitation**: This is a Windows-specific .NET MAUI 9 issue. The `TappedEventArgs.Buttons` property should work according to Microsoft's documentation, but currently returns `0` in practice. The two-recognizer workaround is the most reliable solution.

---

**Issue**: Right-click opens context menu instead of flagging

**Solution**: The two-recognizer approach handles this automatically. If you still see a context menu, you can try:
```csharp
// Note: e.Handled may not work in all .NET MAUI versions
private void OnCellRightClicked(object sender, TappedEventArgs e)
{
    if (sender is Grid grid && grid.BindingContext is CellModel cell)
    {
        _viewModel.CellFlaggedCommand.Execute(cell);
        // Uncomment if context menu still appears:
        // e.Handled = true;
    }
}
```

---

### Android

**Issue**: Cells too small to tap accurately

**Solution**: Increase `CellSize` resource in App.xaml:
```xaml
<x:Double x:key="CellSize">50</x:Double> <!-- Increased from 40 -->
```

**Issue**: Long-press not registering

**Solution**: Ensure `PointerGestureRecognizer` is properly configured:
```xaml
<PointerGestureRecognizer PointerPressed="OnCellPointerPressed"/>
```

May need to adjust long-press duration (platform-specific behavior).

---

### iOS

**Issue**: App crashes with "Unhandled managed exception: Index was out of range"

**Cause**: Flood-fill recursion too deep on large boards

**Solution**: Convert flood-fill to iterative approach:
```csharp
private void RevealAdjacentCells(CellModel cell)
{
    var queue = new Queue<CellModel>();
    queue.Enqueue(cell);
    
    while (queue.Count > 0)
    {
        var current = queue.Dequeue();
        (int row, int col) = FindCellPosition(current);
        
        for (int dr = -1; dr <= 1; dr++)
        {
            for (int dc = -1; dc <= 1; dc++)
            {
                // ... check bounds, reveal, enqueue if needed
            }
        }
    }
}
```

---

### macOS Catalyst

**Issue**: Window too small for board

**Solution**: Set minimum window size in `Platforms/MacCatalyst/Info.plist`:
```xml
<key>UIApplicationSupportsIndirectInputEvents</key>
<true/>
<key>UIMinimumWindowSize</key>
<string>{800, 600}</string>
```

---

## Performance Issues

### Issue: Board generation slow on large boards

**Symptom**: Delay when starting Hard difficulty (16x30)

**Solution**:
1. Profile board generation code
2. Optimize mine placement algorithm
3. Consider async board generation:
   ```csharp
   private async Task StartNewGameAsync()
   {
       var board = await Task.Run(() => 
           GameBoardService.GenerateBoard(_rows, _cols, _mineCount));
       // ...
   }
   ```

---

### Issue: CollectionView scrolling laggy

**Symptom**: Janky scrolling on large boards

**Solution**:
1. Reduce cell size
2. Simplify cell template (remove complex converters)
3. Use virtualization (CollectionView does this automatically)
4. Profile rendering with .NET MAUI profiling tools

---

### Issue: Memory usage increasing over multiple games

**Symptom**: App uses more memory after each new game

**Cause**: Timer or event handlers not disposed

**Solution**:
1. Ensure timer disposal:
   ```csharp
   _gameTimer?.Dispose();
   _gameTimer = null;
   ```

2. Unsubscribe from events if any

3. Run memory profiler to detect leaks

---

## Documentation Issues

### Issue: Emojis showing as question marks (??) in README files

**Symptom**: When viewing README.md or other markdown files in certain editors or browsers, emojis (??, ??, ??, etc.) appear as `??` question marks instead of the intended icons.

**Root Cause**: File encoding mismatch. The files were either:
1. Not saved with UTF-8 encoding
2. Saved with incorrect encoding (Windows-1252, ANSI, etc.)
3. Contained corrupted emoji characters that were saved with wrong encoding

**Solution**: Re-save files with proper UTF-8 encoding

#### In Visual Studio:

**Method 1: Advanced Save Options**
1. Open the affected file (e.g., `README.md`)
2. Go to **File** ? **Advanced Save Options...**
   - If option not visible: **Tools** ? **Customize...** ? **Commands** ? **Menu bar: File** ? **Add Command...** ? **File** ? **Advanced Save Options...**
3. Select **"Unicode (UTF-8 without signature) - Codepage 65001"**
4. Click **OK**
5. Save the file (`Ctrl+S`)

**Method 2: Save As with Encoding**
1. Open the affected file
2. **File** ? **Save As...**
3. Click dropdown arrow next to **Save** button
4. Select **"Save with Encoding..."**
5. Choose **"Unicode (UTF-8 without signature) - Codepage 65001"**
6. Click **OK**

#### In VS Code:
1. Open the file
2. Look at **bottom-right corner** (shows current encoding, e.g., "UTF-8" or "Windows-1252")
3. Click on the encoding name
4. Select **"Save with Encoding"**
5. Choose **"UTF-8"**
6. Save the file

#### In Notepad++:
1. Open the file
2. Go to **Encoding** menu
3. Select **"Encode in UTF-8"** (NOT "UTF-8 with BOM")
4. Save the file

#### PowerShell Batch Conversion:
If you need to convert multiple files:

```powershell
# Navigate to repository root
cd "C:\Path\To\MAUI-Minesweeper"

# Convert README files to UTF-8 without BOM
$files = @(
    "README.md",
    "BlazorMinesweeper\README.md",
    "docs\DEPLOYMENT_GUIDE.md"
    # Add more files as needed
)

foreach ($file in $files) {
    if (Test-Path $file) {
        $content = Get-Content $file -Raw
        [System.IO.File]::WriteAllText($file, $content, [System.Text.UTF8Encoding]::new($false))
        Write-Host "? Converted: $file"
    } else {
        Write-Host "? Not found: $file"
    }
}
```

**Why UTF-8 without BOM?**

| Encoding Type | Description | For Markdown/README |
|---------------|-------------|---------------------|
| **UTF-8 without BOM** ? | Clean UTF-8, no byte order mark | **Recommended** - Works everywhere |
| UTF-8 with BOM | Has `EF BB BF` bytes at start | Can cause issues with some tools |
| ANSI / Windows-1252 | Limited character set (Western European) | ? Emojis don't work |
| Unicode (UTF-16) | Uses 2 bytes per character | ? Not suitable for text files |

**BOM (Byte Order Mark)**: A special 3-byte sequence (`EF BB BF`) at the start of UTF-8 files. While optional for UTF-8, it can cause issues with:
- Git diffs
- Shell scripts
- Some parsers
- GitHub rendering (rare)

For Markdown/README files, **UTF-8 without BOM** is the universal standard.

**Verification**:

After re-saving, check if emojis render correctly:

1. **In Visual Studio**: Close and reopen the file
2. **On GitHub**: Commit and push, then view on GitHub.com
3. **In Git diff**: `git diff README.md` should show emoji changes
4. **File size**: UTF-8 files with emojis are slightly larger than ANSI

**Common Emojis Used in This Project**:

| Emoji | Unicode | Meaning |
|-------|---------|---------|
| ?? | U+1F4A3 | Bomb/mine |
| ?? | U+1F3AE | Game controller |
| ?? | U+1F680 | Launch/start |
| ?? | U+1F6A9 | Flag |
| ? | U+2705 | Check mark |
| ?? | U+1F4E6 | Package |
| ?? | U+1F310 | Globe/web |
| ?? | U+1F3AF | Target/goal |
| ?? | U+23F1 | Stopwatch |
| ??? | U+1F6E0 | Tools |
| ?? | U+2764 | Heart |

All these emojis are part of Unicode and will render correctly when files are saved as UTF-8.

**Prevention**:

To prevent future encoding issues:

1. **Set Visual Studio default encoding**:
   - **Tools** ? **Options**
   - Navigate to **Environment** ? **Documents**
   - Check: **"Save documents as Unicode (UTF-8 without signature) when data cannot be saved in codepage"**
   - Click **OK**

2. **Use VS Code for Markdown**: VS Code has better emoji and UTF-8 support out of the box

3. **Check Git settings**:
   ```bash
   # Ensure Git doesn't mess with encoding
   git config --global core.autocrlf true  # Windows
   git config --global core.safecrlf warn
   ```

4. **Add to .editorconfig**:
   ```ini
   # .editorconfig
   [*.md]
   charset = utf-8
   ```

**Fixed Files** (as of 2025-01-17):
- ? `README.md` - Root repository README
- ? `BlazorMinesweeper/README.md` - Blazor project README

Both files now properly display:
- ?? Minesweeper title
- ?? Play Online section
- ?? Getting Started
- ? Features with checkmarks
- All other emojis throughout documentation

**Issue Status**: ? **RESOLVED**

---

## Debugging Tips

### Enable Detailed XAML Diagnostics

**Method 1**: Check binding diagnostics in Output window
- View ? Output ? Show output from: "XAML Binding Failures"

**Method 2**: Add debug logging to converters:
```csharp
public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
{
    System.Diagnostics.Debug.WriteLine($"CellBackgroundConverter: value={value}");
    // ...
}
```

---

### Add Logging to ViewModel Methods

```csharp
private void OnCellTapped(CellModel? cell)
{
    System.Diagnostics.Debug.WriteLine($"Cell tapped: IsMainThread={MainThread.IsMainThread}");
    System.Diagnostics.Debug.WriteLine($"Cell: IsMine={cell?.IsMine}, IsRevealed={cell?.IsRevealed}");
    
    // ...
}
```

---

### Breakpoint Best Practices

1. **Check thread context**: Look at Threads window (Debug ? Windows ? Threads)
2. **Watch expressions**: Add `MainThread.IsMainThread`, `_isGameOver`, `cell != null`
3. **Conditional breakpoints**: Right-click breakpoint ? Conditions ? `cell.IsMine == true`

---

### Common Debug Watches

```csharp
// ViewModel
_isGameOver
Cells.Count
FlagsRemaining
GameStatus
_board != null

// CellModel
IsRevealed
IsFlagged
IsMine
NeighboringMines
DisplayText

// Thread context
Microsoft.Maui.ApplicationModel.MainThread.IsMainThread
Environment.CurrentManagedThreadId
```

---

### Reproduce Issues Reliably

1. **Rapid tapping**: Click cells very quickly to test race conditions
2. **Spam New Game**: Click New Game button repeatedly
3. **Test win/loss**: Use Easy difficulty (8x8, 10 mines) for quick test cycles
4. **Test edge cases**:
   - Tap first cell (may be mine)
   - Flag all cells, then unflag
   - Win game, then click cells (should be disabled)

---

### Clean Build Process

When strange errors occur, try:
```bash
# 1. Clean solution
dotnet clean

# 2. Delete bin and obj folders manually
Remove-Item -Recurse -Force bin, obj

# 3. Restore packages
dotnet restore

# 4. Rebuild
dotnet build
```

In Visual Studio:
- Build ? Clean Solution
- Build ? Rebuild Solution

---

## Error Logging Best Practices

Always include exception details:
```csharp
try
{
    // Code
}
catch (Exception ex)
{
    System.Diagnostics.Debug.WriteLine($"Error in StartNewGame: {ex.GetType().Name}");
    System.Diagnostics.Debug.WriteLine($"Message: {ex.Message}");
    System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
    throw; // Re-throw to see in debugger
}
```

---

## Getting Help

### Steps Before Asking for Help
1. Check Output window for XAML binding errors
2. Check Debug output for exception details
3. Verify binding contexts (`x:DataType` matches actual context)
4. Test on multiple platforms if possible
5. Isolate issue (does it happen on new game? specific action?)

### Information to Provide
- Platform (Windows, Android, iOS, macOS)
- .NET version (`dotnet --version`)
- MAUI workload version
- Steps to reproduce
- Error message (exact text)
- Stack trace
- Code snippet where error occurs

---

## Known Issues & Workarounds

### Issue: CollectionView sometimes doesn't update on Cells replacement
**Workaround**: Call `OnPropertyChanged(nameof(Cells))` explicitly after assignment

### Issue: Timer occasionally skips seconds
**Workaround**: Use `System.Threading.Timer` instead of UI-based timers

### Issue: Flood-fill can be slow on large boards
**Workaround**: Limit board size or convert to iterative algorithm

---

## Quick Reference: Common Fixes

| Issue | Quick Fix |
|-------|-----------|
| Binding mismatch errors | Check `x:DataType` matches actual BindingContext |
| Commands not firing | Use `RelativeSource` to access ViewModel commands |
| UI not updating | Ensure `SetProperty` called in property setters |
| Computed properties not updating | Call `OnPropertyChanged(nameof(ComputedProperty))` in dependent property setters |
| Cell text/flags not appearing | Add `OnPropertyChanged(nameof(DisplayText))` to all properties that `DisplayText` depends on |
| Thread exceptions | Wrap UI updates in `MainThread.BeginInvokeOnMainThread` |
| App crashes on new game | Replace `Cells` collection instead of clearing |
| Timer keeps running | Dispose timer in `HandleGameOver()` |
| Cells not clickable | Check `CommandParameter="{Binding .}"` is set |
| Converters not working | Register in App.xaml resources as `StaticResource` |

---

**Last Updated**: 2025-01-XX (Updated after fixing DisplayText notification issue)  
**Maintained By**: Jack Taylor

**Note**: If you encounter an issue not covered here, please add it to this document after resolving it!
