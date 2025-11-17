# GitHub Copilot Instructions for MAUI Minesweeper

## Project Overview
This is a .NET MAUI Minesweeper game application targeting multiple platforms (Android, iOS, Windows, macOS Catalyst).

## Technology Stack
- **Framework**: .NET 9
- **Language**: C# 13.0
- **UI Framework**: .NET MAUI
- **Architecture**: MVVM (Model-View-ViewModel)
- **Toolkit**: CommunityToolkit.Mvvm

## Project Structure
```
MAUI Minesweeper/
├── Models/              # Data models (CellModel, etc.)
├── ViewModels/          # View models with ObservableObject base
├── Views/               # XAML pages and code-behind
├── Services/            # Business logic (GameBoardService, etc.)
├── Converters/          # Value converters for XAML bindings
├── Resources/           # Images, fonts, styles
└── Platforms/           # Platform-specific code
```

## Coding Standards & Conventions

### General Guidelines
- Use C# 13.0 features where appropriate (primary constructors, collection expressions, etc.)
- Follow MVVM pattern strictly
- Use async/await for asynchronous operations
- Prefer dependency injection for services
- Use `Microsoft.Maui.ApplicationModel.MainThread` for UI thread operations

### Naming Conventions
- **Classes**: PascalCase (e.g., `GameViewModel`, `CellModel`)
- **Interfaces**: IPascalCase (e.g., `IGameService`)
- **Private fields**: _camelCase (e.g., `_gameTimer`, `_isGameOver`)
- **Properties**: PascalCase (e.g., `FlagsRemaining`, `GameStatus`)
- **Methods**: PascalCase (e.g., `StartNewGame`, `OnCellTapped`)
- **Commands**: PascalCase with "Command" suffix (e.g., `CellTappedCommand`, `NewGameCommand`)

### MVVM Pattern
- **Models**: Plain data classes with `ObservableObject` base for property change notifications
- **ViewModels**: Inherit from `ObservableObject`, use `SetProperty` for property setters
- **Views**: XAML files with minimal code-behind (event handlers only when necessary)
- Use `Command` and `Command<T>` for user interactions
- Keep business logic in Services, not ViewModels

### Property Implementation
Use `ObservableObject.SetProperty` pattern:
```csharp
private int _value;
public int Value
{
    get => _value;
    set => SetProperty(ref _value, value);
}
```

### XAML Guidelines
- Use compiled bindings with `x:DataType` for performance and type safety
- **Page-level bindings**: Set `x:DataType="viewmodels:GameViewModel"` on ContentPage
- **ItemTemplate bindings**: Set `x:DataType="models:CellModel"` on DataTemplate
- Access ViewModel commands from DataTemplate using:
  ```xaml
  Command="{Binding Source={RelativeSource AncestorType={x:Type ContentPage}}, Path=BindingContext.CommandName}"
  ```
- Use value converters for complex UI logic (color, visibility, etc.)
- Prefer resource dictionaries for reusable styles

### Threading Rules
- Always update UI properties on the main thread:
  ```csharp
  Microsoft.Maui.ApplicationModel.MainThread.BeginInvokeOnMainThread(() =>
  {
      // Update UI properties here
  });
  ```
- Use `System.Threading.Timer` for background timers, not UI-based timers
- Check `MainThread.IsMainThread` when debugging threading issues

### Error Handling
- Use try-catch blocks for operations that may fail
- Log errors using `System.Diagnostics.Debug.WriteLine`
- Include exception type, message, and stack trace in logs
- Add comments in Troubleshooting.md for common issues
- Don't swallow exceptions silently

## Platform-Specific Considerations
- Test on multiple platforms (Android, iOS, Windows) when possible
- Avoid Xamarin.Forms APIs—use .NET MAUI equivalents only
- Use conditional compilation (`#if ANDROID`, `#if IOS`, etc.) for platform-specific code
- Place platform-specific implementations in `Platforms/` folders

## Common Patterns in This Project

### ObservableCollection Updates
When replacing entire collections, notify property changed:
```csharp
Cells = newCells;
OnPropertyChanged(nameof(Cells));
```

### Command Parameters
Pass model instances as command parameters:
```xaml
Command="{Binding SomeCommand}"
CommandParameter="{Binding .}"
```

### Resource Cleanup
Dispose timers and subscriptions properly:
```csharp
_gameTimer?.Dispose();
```

## Dependencies
- `CommunityToolkit.Mvvm` - For MVVM helpers (ObservableObject, etc.)
- Add other packages as needed for the project

## Testing Guidelines
- Write unit tests for ViewModels and Services
- Test game logic independently of UI
- Mock dependencies in unit tests
- Test edge cases (win conditions, game over, flag counter, etc.)

## DO's and DON'Ts

### DO
✅Use `ObservableObject` and `SetProperty` for property notifications  
✅ Keep ViewModels testable and UI-independent  
✅ Use dependency injection for services  
✅ Handle thread safety for UI updates  
✅ Use `x:DataType` for compiled bindings  
✅ Dispose of resources (timers, subscriptions)  
✅ Follow MVVM separation of concerns  

### DON'T
❌ Don't manipulate UI elements directly from ViewModels  
❌ Don't use Xamarin.Forms APIs (use .NET MAUI equivalents)  
❌ Don't mix `x:DataType` contexts (e.g., ViewModel type in ItemTemplate with Model context)  
❌ Don't update UI properties from background threads without marshalling  
❌ Don't put business logic in code-behind files  
❌ Don't create memory leaks (unsubscribed events, undisposed timers)  

## Specific Project Context

### Game Logic
- Board is represented as a 2D array `CellModel[,]`
- Mines are randomly placed using `GameBoardService.GenerateBoard`
- Flood-fill algorithm reveals adjacent cells when a cell with 0 neighboring mines is clicked
- Win condition: all non-mine cells revealed
- Lose condition: mine cell revealed

### UI Interactions
- **Tap**: Reveal cell (handled by `CellTappedCommand`)
- **Right-click/Long-press**: Toggle flag (handled by `CellFlaggedCommand`)
- **New Game button**: Restart game with current difficulty
- **Difficulty picker**: Change board size and mine count

### Binding Contexts
- **ContentPage**: `GameViewModel`
- **CollectionView ItemTemplate**: `CellModel` (individual cell data)
- To access ViewModel commands from cell template, use `RelativeSource` to ContentPage

## Questions to Ask Before Coding
1. Does this follow MVVM pattern?
2. Am I updating UI properties on the main thread?
3. Is `x:DataType` correctly matching the BindingContext?
4. Are resources properly disposed?
5. Is this .NET MAUI (not Xamarin.Forms)?
6. Will this work on all target platforms?

## Additional Notes
- Game timer increments every second while game is active
- Flags remaining counter decrements when flags are placed
- All mines are revealed on game over (win or lose)
- Board supports custom sizes and mine counts

---

**Last Updated**: [Add date when you update this file]  
**Maintained By**: Jack Taylor
