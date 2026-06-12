# MAUI Minesweeper Interview Prep

## 1) 30-Second Project Pitch
I built a cross-platform Minesweeper implementation in C#/.NET 9 with two front ends: a native .NET MAUI app for desktop/mobile and a Blazor WebAssembly app deployed to GitHub Pages. The key engineering choice was separating UI from logic so I could reuse game rules, keep behavior consistent across platforms, and iterate quickly. I used MVVM in MAUI, command-based interactions, observable state updates, and CI/CD for automatic web deployment.

## 2) 2-Minute Technical Walkthrough
- Architecture:
  - Presentation: MAUI XAML page + Blazor Razor page
  - State orchestration: GameViewModel (MAUI) / component state (Blazor)
  - Game logic: GameBoardService (board generation, reveal, flag)
  - Model: CellModel with observable properties and computed display
- Core gameplay flow:
  1. StartNewGame initializes timer, status, counters, and board.
  2. User interaction triggers tap/flag handlers.
  3. Service reveals cells or toggles flags.
  4. Flood-fill reveals contiguous zero-neighbor regions.
  5. Win/loss check updates status and reveals board on completion.
- CI/CD:
  - GitHub Actions restores/builds/publishes Blazor app.
  - Workflow rewrites base href for GitHub Pages pathing and publishes artifact.

## 3) What To Highlight (Interview Value)
- Practical architecture tradeoffs:
  - MAUI side follows MVVM for clean separation.
  - Blazor side prioritizes delivery speed while keeping logic structured.
- Threading correctness:
  - MAUI timer updates UI-bound properties via MainThread marshaling.
- Algorithmic thinking:
  - Recursive flood-fill and adjacency counting with bounds checking.
- Product thinking:
  - Difficulty presets, timer, flag counter, and game-state UX.
- Delivery discipline:
  - Automated GitHub Pages deployment and reproducible build steps.

## 4) Likely Interview Questions + Strong Answers

### Q1: Why two apps (MAUI + Blazor) instead of one?
I wanted to demonstrate portability of domain logic and compare native vs web tradeoffs. MAUI gives native UX and offline device integration. Blazor gives instant distribution and no install friction. Keeping both in one repo showcases architecture decisions, not just UI implementation.

### Q2: How did you keep logic testable?
I isolated board rules in GameBoardService and kept UI concerns in view-level layers. Cell state is explicit in CellModel, and rules are deterministic for a given board state. This makes it straightforward to unit test reveal logic, win/loss checks, and edge cases without spinning up UI.

### Q3: How do you handle thread safety in MAUI?
The timer runs off the UI thread, so state updates that affect bindings are dispatched onto MainThread. That avoids cross-thread UI update issues and keeps property notifications safe.

### Q4: What are the most important edge cases?
- Prevent revealing flagged/revealed cells.
- Prevent flag toggles after game over.
- Ensure flood-fill only traverses valid in-bounds neighbors.
- Ensure win condition counts only non-mine revealed cells.
- Ensure resources like timers are disposed when restarting/ending games.

### Q5: Biggest improvement you would make next?
I would remove duplicated gameplay logic between MAUI and Blazor by extracting a fully shared game-engine library and adding deterministic tests with seeded randomness. That would improve confidence and reduce maintenance cost.

## 5) Honest Tradeoffs (Say These If Asked)
- Current mine generation uses non-seeded randomness; reproducibility is limited.
- Flood-fill uses recursion; iterative BFS/DFS would be safer for very large boards.
- Blazor page currently owns more UI/game state than ideal MVVM-like separation.
- There are opportunities for deeper automated tests and analytics.

## 6) Demo Script (5 Minutes)
1. Open web version and complete a quick easy game.
2. Show difficulty change and explain state reset behavior.
3. Trigger a loss and describe end-state handling.
4. Explain one code path end-to-end: click -> command/handler -> service -> state update -> render.
5. Show deployment workflow and explain why base-href rewrite is required on GitHub Pages.

## 7) Technical Deep Dives You Can Offer
- Flood-fill complexity and alternatives:
  - Current approach is recursive traversal over connected zero-neighbor region.
  - Time complexity is O(R*C) in worst case.
- Data structures:
  - Board stored as 2D array for neighbor math.
  - Flattened collection/list used for UI binding/render loops.
- UI architecture:
  - MAUI compiled bindings, commands, converters.
  - Blazor event handlers and component re-render lifecycle.

## 8) Red Flags To Avoid In Interview
- Do not claim it is fully DRY across MAUI and Blazor yet.
- Do not overstate test coverage.
- Do not say “works everywhere” without noting platform-specific testing constraints.

## 9) 60-Second Closing Statement
This project demonstrates how I structure real features from architecture to deployment, not just coding screens. I focused on clean separation of concerns, robust game-state handling, cross-platform thinking, and shipping discipline through CI/CD. If I extended it, I would prioritize a shared engine package, stronger automated tests, and performance-safe iterative flood-fill.

## 10) Last-Minute Rehearsal Checklist
- I can explain architecture in under 2 minutes.
- I can walk one click event from UI to game rule to render.
- I can describe one technical challenge and my fix.
- I can name 2 improvements with clear tradeoffs.
- I can explain deployment pipeline in plain language.
