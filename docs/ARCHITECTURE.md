# MAUI Minesweeper - Architecture Documentation

## Overview
This document describes the architecture, design patterns, and technical implementation details of the MAUI Minesweeper application.

## Table of Contents
1. [High-Level Architecture](#high-level-architecture)
2. [MVVM Pattern Implementation](#mvvm-pattern-implementation)
3. [Project Structure](#project-structure)
4. [Data Flow](#data-flow)
5. [Threading Model](#threading-model)
6. [State Management](#state-management)
7. [Platform-Specific Considerations](#platform-specific-considerations)

---

## High-Level Architecture

```
┌─────────────────────────────────────────────────────────┐
│                      Presentation Layer                 │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐   │
│  │ GamePage.xaml│  │   Converters │  │   Resources  │   │
│  │   (View)     │  │ (UI Helpers) │  │   (Styles)   │   │
│  └──────────────┘  └──────────────┘  └──────────────┘   │
└─────────────────────────────────────────────────────────┘
                           ↕ Data Binding
┌─────────────────────────────────────────────────────────┐
│                     ViewModel Layer                     │
│  ┌──────────────────────────────────────────────────┐   │
│  │          GameViewModel                           │   │
│  │  - Cells (ObservableCollection)                  │   │
│  │  - Commands (CellTappedCommand, etc.)            │   │
│  │  - Properties (FlagsRemaining, GameStatus)       │   │
│  │  - Game Timer Management                         │   │
│  └──────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────┘
                           ↕ Orchestration
┌─────────────────────────────────────────────────────────┐
│                      Business Logic Layer               │
│  ┌──────────────────────────────────────────────────┐   │
│  │          GameBoardService                        │   │
│  │  - GenerateBoard()                               │   │
│  │  - RevealCell()                                  │   │
│  │  - ToggleFlag()                                  │   │
│  │  - Mine placement logic                          │   │
│  │  - Adjacent mine counting                        │   │
│  └──────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────┘
                           ↕ Updates
┌─────────────────────────────────────────────────────────┐
│                        Model Layer                      │
│  ┌──────────────────────────────────────────────────┐   │
│  │          CellModel (ObservableObject)            │   │
│  │  - IsRevealed, IsFlagged, IsMine                 │   │
│  │  - NeighboringMines                              │   │
│  │  - DisplayText (computed property)               │   │
│  └──────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────┘
```

---

## MVVM Pattern Implementation

### Models (`Models/`)
**Purpose**: Represent the data structure for game board cells.

**CellModel**:
- Inherits from `ObservableObject` (CommunityToolkit.Mvvm)
- Properties notify UI of changes via `INotifyPropertyChanged`
- Contains cell state: `IsRevealed`, `IsFlagged`, `IsMine`, `NeighboringMines`
- `DisplayText` is a computed property (returns emoji or number)

```csharp
// Pattern used throughout Models
public bool IsRevealed
{
    get => _isRevealed;
    set => SetProperty(ref _isRevealed, value);
}
private bool _isRevealed;
```

**Key Design Decisions**:
- Models are observable to enable real-time UI updates
- No business logic in models (kept in Services)
- DisplayText computed on-the-fly (no caching needed for small board)

---

### ViewModels (`ViewModels/`)
**Purpose**: Orchestrate user interactions, manage UI state, coordinate with Services.

**GameViewModel**:
- Central orchestrator for game state and user interactions
- Manages game timer (`System.Threading.Timer`)
- Exposes commands: `CellTappedCommand`, `CellFlaggedCommand`, `NewGameCommand`
- Properties: `Cells`, `FlagsRemaining`, `GameStatus`, `ElapsedTime`, `Rows`, `Cols`

**Responsibilities**:
1. **Game Initialization**: Call `GameBoardService.GenerateBoard()`, populate `Cells` collection
2. **User Input Handling**: Execute commands when user taps/flags cells
3. **Game Logic Coordination**: Call service methods, check win/lose conditions
4. **UI State Management**: Update flags remaining, game status, timer
5. **Threading**: Ensure UI updates happen on main thread

**Key Implementation Details**:
- Uses `ObservableCollection<CellModel>` for `Cells` to enable CollectionView binding
- Timer updates `ElapsedTime` every second on main thread via `MainThread.BeginInvokeOnMainThread`
- Flood-fill algorithm for revealing adjacent cells (0 neighboring mines)
- Win condition: All non-mine cells revealed
- Lose condition: Mine cell revealed

**Thread Safety Pattern**:
```csharp
// All UI property updates wrapped in MainThread check
Microsoft.Maui.ApplicationModel.MainThread.BeginInvokeOnMainThread(() =>
{
    ElapsedTime++;
});
```

---

### Views (`Views/`)
**Purpose**: XAML-based UI with minimal code-behind.

**GamePage.xaml**:
- Uses compiled bindings (`x:DataType="viewmodels:GameViewModel"`)
- CollectionView with `GridItemsLayout` for board grid
- Header displays: Flags remaining, Game status, Timer
- Footer contains: Difficulty picker, New Game button

**Binding Contexts**:
- **ContentPage**: `GameViewModel` (set in code-behind constructor)
- **DataTemplate (ItemTemplate)**: `CellModel` (automatic per-item binding)
- **Commands from DataTemplate**: Use `RelativeSource` to access `GameViewModel.CellTappedCommand`

```xaml
<!-- Accessing ViewModel command from DataTemplate -->
<TapGestureRecognizer 
    Command="{Binding Source={RelativeSource AncestorType={x:Type ContentPage}}, 
                      Path=BindingContext.CellTappedCommand}"
    CommandParameter="{Binding .}" />
```

**GamePage.xaml.cs**:
- Dependency injection: Constructor receives `GameViewModel`
- Sets `BindingContext`
- Handles `OnDifficultyChanged` event (calls `ViewModel.SetDifficulty`)
- Handles `OnCellPointerPressed` for right-click/long-press flagging

---

### Services (`Services/`)
**Purpose**: Pure business logic, no UI dependencies, stateless.

**GameBoardService** (static class):
- `GenerateBoard(rows, cols, mineCount)`: Creates 2D array of `CellModel`, places mines randomly
- `RevealCell(cell)`: Sets `IsRevealed = true`
- `ToggleFlag(cell)`: Toggles `IsFlagged` state
- Mine placement uses `Random` with shuffle algorithm
- Adjacent mine counting uses 8-directional neighbor checks

**Design Rationale**:
- Static service (no state to manage)
- Testable in isolation (no UI dependencies)
- Could be converted to interface (`IGameBoardService`) if needed for DI/mocking

---

### Converters (`Converters/`)
**Purpose**: Transform data for XAML binding display.

**CellBackgroundConverter**:
- Converts `IsRevealed` boolean to background color
- Revealed: Light gray, Unrevealed: Dark gray

**MineCountColorConverter**:
- Converts `NeighboringMines` count to color
- 1 = Blue, 2 = Green, 3 = Red, 4+ = Purple

**Why Converters?**:
- Keep ViewModel free of UI-specific color logic
- Reusable across views
- Declarative in XAML

---

## Project Structure

```
MAUI Minesweeper/
├── Models/
│   └── CellModel.cs               # Cell data model (observable)
├── ViewModels/
│   └── GameViewModel.cs           # Main game orchestrator
├── Views/
│   ├── GamePage.xaml              # Main game UI
│   └── GamePage.xaml.cs           # Code-behind (minimal)
├── Services/
│   └── GameBoardService.cs        # Game logic (board generation, reveal, flag)
├── Converters/
│   ├── CellBackgroundConverter.cs # IsRevealed → Color
│   └── MineCountColorConverter.cs # NeighboringMines → Color
├── Resources/
│   ├── Styles/                    # App-wide styles
│   ├── Images/                    # Icons, graphics
│   └── Fonts/                     # Custom fonts
├── Platforms/
│   ├── Android/
│   ├── iOS/
│   ├── Windows/
│   └── MacCatalyst/
├── App.xaml                       # Application resources
├── App.xaml.cs                    # Application entry
├── AppShell.xaml                  # Shell navigation
├── MauiProgram.cs                 # DI, configuration
└── MAUI Minesweeper.csproj        # Project file
```

---

## Data Flow

### Game Initialization Flow
```
1. User opens app
   ↓
2. App.xaml.cs → CreateWindow() → AppShell
   ↓
3. AppShell navigates to GamePage
   ↓
4. GamePage constructor receives GameViewModel (DI)
   ↓
5. GameViewModel constructor calls StartNewGame()
   ↓
6. StartNewGame() calls GameBoardService.GenerateBoard()
   ↓
7. Board (CellModel[,]) created, Cells collection populated
   ↓
8. CollectionView binds to Cells, renders grid
   ↓
9. Timer starts (updates ElapsedTime every second)
```

### Cell Tap Flow (Reveal)
```
1. User taps cell in CollectionView
   ↓
2. TapGestureRecognizer fires
   ↓
3. CellTappedCommand executes in GameViewModel
   ↓
4. OnCellTapped(CellModel) method called
   ↓
5. Check: IsGameOver? IsFlagged? IsRevealed? → Exit if true
   ↓
6. GameBoardService.RevealCell(cell) → Sets IsRevealed = true
   ↓
7. If cell.IsMine → HandleGameOver(false) → Reveal all cells
   ↓
8. If cell.NeighboringMines == 0 → RevealAdjacentCells (flood-fill)
   ↓
9. CheckWinCondition() → If all non-mines revealed → HandleGameOver(true)
   ↓
10. UI updates automatically via property change notifications
```

### Cell Flag Flow
```
1. User right-clicks or long-presses cell
   ↓
2. PointerGestureRecognizer fires OnCellPointerPressed
   ↓
3. Code-behind calls CellFlaggedCommand.Execute(cell)
   ↓
4. OnCellFlagged(CellModel) method called
   ↓
5. GameBoardService.ToggleFlag(cell) → Toggles IsFlagged
   ↓
6. FlagsRemaining updated: += (IsFlagged ? -1 : 1)
   ↓
7. CheckWinCondition() (in case all cells correctly flagged)
   ↓
8. UI updates automatically
```

### New Game Flow
```
1. User clicks "New Game" button
   ↓
2. NewGameCommand executes
   ↓
3. StartNewGame() called
   ↓
4. Timer stopped, state reset
   ↓
5. New board generated
   ↓
6. Cells collection replaced (triggers CollectionView refresh)
   ↓
7. OnPropertyChanged(nameof(Cells)) ensures UI updates
   ↓
8. New timer started
```

---

## Threading Model

### Main Thread (UI Thread)
- All UI property updates must occur on main thread
- MAUI provides `Microsoft.Maui.ApplicationModel.MainThread` helper
- Pattern used throughout:

```csharp
if (Microsoft.Maui.ApplicationModel.MainThread.IsMainThread)
{
    // Update UI property directly
    ElapsedTime++;
}
else
{
    Microsoft.Maui.ApplicationModel.MainThread.BeginInvokeOnMainThread(() =>
    {
        ElapsedTime++;
    });
}
```

### Background Timer Thread
- `System.Threading.Timer` runs callback on thread pool thread
- Must marshal UI updates to main thread
- Timer disposed when game ends or new game starts

**Timer Implementation**:
```csharp
_gameTimer = new System.Threading.Timer(_ =>
{
    if (!_isGameOver)
    {
        MainThread.BeginInvokeOnMainThread(() => ElapsedTime++);
    }
}, null, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1));
```

### Why Not Use Dispatcher.Timer or UI Timer?
- Platform-specific inconsistencies
- `System.Threading.Timer` is cross-platform and reliable
- Explicit main thread marshalling gives better control

---

## State Management

### Game State Properties
| Property | Type | Purpose |
|----------|------|---------|
| `Cells` | `ObservableCollection<CellModel>` | All cells in board (flat list for CollectionView) |
| `_board` | `CellModel[,]` | 2D array for flood-fill navigation |
| `_isGameOver` | `bool` | Prevents further input after win/loss |
| `FlagsRemaining` | `int` | Number of flags available to place |
| `GameStatus` | `string` | "Playing", "Won!", "Lost!" |
| `ElapsedTime` | `int` | Seconds since game started |
| `_gameTimer` | `System.Threading.Timer` | Updates ElapsedTime |

### State Transitions
```
[Initial] → StartNewGame() → [Playing]
                                  ↓
                    Cell Tapped (Mine) → [Lost]
                                  ↓
                    All Non-Mines Revealed → [Won]
                                  ↓
                    NewGameCommand → [Playing]
```

### Resource Cleanup
- Timer disposed in `HandleGameOver()` and `StartNewGame()`
- No explicit cleanup for ObservableCollection (GC handles it)
- Could implement `IDisposable` on ViewModel for production apps

---

## Platform-Specific Considerations

### Windows
- Right-click for flagging works natively
- Pointer events supported via `PointerGestureRecognizer`

### Android
- Long-press for flagging
- Touch gestures handled via `PointerGestureRecognizer`
- May need larger touch targets (adjust CellSize resource)

### iOS
- Long-press for flagging
- Gesture recognizers work cross-platform
- Test on physical devices (simulator may not reflect touch accuracy)

### macOS Catalyst
- Right-click support like Windows
- Retina display considerations for cell sizing

### Cross-Platform Best Practices
1. Use `PointerGestureRecognizer` for unified gesture handling
2. Test on multiple platforms (different screen sizes, DPIs)
3. Use `StaticResource CellSize` for responsive layout adjustments
4. Avoid platform-specific APIs in shared code

---

## Performance Considerations

### CollectionView Performance
- Uses `GridItemsLayout` for efficient grid rendering
- Compiled bindings (`x:DataType`) improve performance
- Cell virtualization handled automatically by CollectionView

### Board Size Limits
- Current implementation tested up to 16x30 (Hard difficulty)
- Larger boards may need:
  - Cell virtualization optimization
  - Smaller cell sizes
  - Lazy loading or chunked rendering

### Flood-Fill Optimization
- Recursive flood-fill can be slow on large boards
- Current implementation: Recursion depth limited by board size
- Potential improvement: Use iterative approach with queue

### Memory Management
- Timer disposed on game end
- ObservableCollection replaced (not cleared and re-added) to avoid CollectionView bugs
- No memory leaks detected in current implementation

---

## Dependency Injection (Future Enhancement)

Currently, GameViewModel is instantiated directly. For scalability:

**MauiProgram.cs** (future):
```csharp
builder.Services.AddSingleton<IGameBoardService, GameBoardService>();
builder.Services.AddTransient<GameViewModel>();
builder.Services.AddTransient<GamePage>();
```

**Benefits**:
- Testable (mock `IGameBoardService`)
- Loosely coupled
- Easier to add features (e.g., save/load game state)

---

## Extension Points

### Adding New Features
1. **Difficulty Levels**: Already supported (Easy, Medium, Hard)
2. **Leaderboard**: Add persistence layer (SQLite, Preferences)
3. **Themes**: Implement theme service, update converters
4. **Animations**: Add `Behaviors` for cell reveal animations
5. **Sound Effects**: Platform-specific audio service
6. **Hints**: Service method to reveal safe cell

### Adding New Views
1. Create XAML view in `Views/`
2. Create corresponding ViewModel in `ViewModels/`
3. Register in DI container (if using DI)
4. Add route to `AppShell.xaml`

---

## Testing Strategy

### Unit Tests (Recommended)
- Test `GameBoardService` logic (mine placement, reveal, flag)
- Test `GameViewModel` state transitions
- Mock `IGameBoardService` for ViewModel tests

### UI Tests (Recommended)
- Test user interactions (tap, flag, new game)
- Test win/lose conditions
- Test difficulty changes

### Manual Testing Checklist
- [ ] Tap reveals cell
- [ ] Right-click/long-press toggles flag
- [ ] Flags remaining updates correctly
- [ ] Timer increments every second
- [ ] Flood-fill reveals adjacent cells
- [ ] Mine reveals all cells and shows "Lost!"
- [ ] All non-mines revealed shows "Won!"
- [ ] New game resets board
- [ ] Difficulty picker changes board size
- [ ] Works on Windows/Android/iOS/macOS

---

## Known Limitations

1. **No Undo/Redo**: Game state not tracked
2. **No Persistence**: Game state lost on app close
3. **No Multiplayer**: Single-player only
4. **Fixed Cell Size**: May not scale well on tablets
5. **No Accessibility**: Screen reader support not implemented
6. **No Analytics**: No tracking of game stats

---

## References

- [.NET MAUI Documentation](https://learn.microsoft.com/en-us/dotnet/maui/)
- [CommunityToolkit.Mvvm](https://learn.microsoft.com/en-us/dotnet/communitytoolkit/mvvm/)
- [MVVM Pattern Guide](https://learn.microsoft.com/en-us/dotnet/architecture/maui/mvvm)

---

**Last Updated**: 2024-01-15  
**Maintained By**: Jack Taylor
