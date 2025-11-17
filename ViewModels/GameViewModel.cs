using MAUI_Minesweeper.Models;
using MAUI_Minesweeper.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;

namespace MAUI_Minesweeper.ViewModels
{
    public partial class GameViewModel : ObservableObject // INHERIT FROM ObservableObject
    {
        private CellModel[,]? _board;
        private int _rows;
        private int _cols;
        private int _mineCount;
        private bool _isGameOver;

        private int _flagsRemaining;
        public int FlagsRemaining
        {
            get => _flagsRemaining;
            set => SetProperty(ref _flagsRemaining, value);
        }

        private string _gameStatus = string.Empty;
        public string GameStatus
        {
            get => _gameStatus;
            set => SetProperty(ref _gameStatus, value);
        }

        private int _elapsedTime;
        public int ElapsedTime
        {
            get => _elapsedTime;
            set => SetProperty(ref _elapsedTime, value);
        }

        private double _cellSize = 40;
        public double CellSize
        {
            get => _cellSize;
            set => SetProperty(ref _cellSize, value);
        }

        public ObservableCollection<CellModel> Cells { get; set; }
        public ICommand CellTappedCommand { get; }
        public ICommand CellFlaggedCommand { get; }
        public ICommand NewGameCommand { get; }

        public int Rows => _rows;
        public int Cols => _cols;

        private System.Threading.Timer? _gameTimer;

        public GameViewModel(int rows = 10, int cols = 10, int mineCount = 15)
        {
            System.Diagnostics.Debug.WriteLine($"GameViewModel constructor - IsMainThread: {Microsoft.Maui.ApplicationModel.MainThread.IsMainThread}");
            System.Diagnostics.Debug.WriteLine($"GameViewModel constructor - Thread ID: {Environment.CurrentManagedThreadId}");
            
            _rows = rows;
            _cols = cols;
            _mineCount = mineCount;

            Cells = new ObservableCollection<CellModel>();
            CellTappedCommand = new Command<CellModel>(OnCellTapped);
            CellFlaggedCommand = new Command<CellModel>(OnCellFlagged);
            NewGameCommand = new Command(StartNewGame);

            StartNewGame();
        }

        private void StartNewGame()
        {
            try
            {
                _isGameOver = false;
                FlagsRemaining = _mineCount;
                GameStatus = "Playing";
                ElapsedTime = 0;

                _gameTimer?.Dispose();
                _gameTimer = new System.Threading.Timer(_ =>
                {
                    if (!_isGameOver)
                    {
                        try
                        {
                            // Update on UI thread
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

                _board = GameBoardService.GenerateBoard(_rows, _cols, _mineCount);

                // Build the complete list of cells first
                var newCells = new ObservableCollection<CellModel>();
                for (int row = 0; row < _rows; row++)
                {
                    for (int col = 0; col < _cols; col++)
                    {
                        newCells.Add(_board[row, col]);
                    }
                }

                // Update UI on main thread
                if (Microsoft.Maui.ApplicationModel.MainThread.IsMainThread)
                {
                    // Replace the entire collection to avoid ScrollIntoView issues
                    Cells = newCells;
                    OnPropertyChanged(nameof(Cells));
                    OnPropertyChanged(nameof(Cols));
                }
                else
                {
                    Microsoft.Maui.ApplicationModel.MainThread.BeginInvokeOnMainThread(() =>
                    {
                        // Replace the entire collection to avoid ScrollIntoView issues
                        Cells = newCells;
                        OnPropertyChanged(nameof(Cells));
                        OnPropertyChanged(nameof(Cols));
                    });
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"StartNewGame error: {ex.GetType().Name}: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                throw;
            }
        }

        private void OnCellTapped(CellModel? cell)
        {
            try
            {
                if (cell == null || _isGameOver || cell.IsFlagged || cell.IsRevealed)
                    return;

                System.Diagnostics.Debug.WriteLine($"Cell tapped - IsMainThread: {Microsoft.Maui.ApplicationModel.MainThread.IsMainThread}");

                GameBoardService.RevealCell(cell);

                // Check if it's a mine (game over)
                if (cell.IsMine)
                {
                    HandleGameOver(false);
                    return;
                }

                // If cell has no neighboring mines, reveal adjacent cells (flood fill)
                if (cell.NeighboringMines == 0)
                {
                    RevealAdjacentCells(cell);
                }

                CheckWinCondition();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"OnCellTapped error: {ex.GetType().Name}: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                throw;
            }
        }

        private void OnCellFlagged(CellModel? cell)
        {
            if (cell == null || _isGameOver)
                return;
                
            bool wasFlagged = cell.IsFlagged;
            GameBoardService.ToggleFlag(cell);
            
            // UPDATE FLAG COUNTER
            FlagsRemaining += cell.IsFlagged ? -1 : 1;
            
            CheckWinCondition();
        }

        // Recursive Flood fill to reveal adjacent cells with 0 neighboring mines
        private void RevealAdjacentCells(CellModel cell)
        {
            if (_board == null)
                return;

            // Find the position of the cell in the board
            (int row, int col) = FindCellPosition(cell);
            if (row == -1 || col == -1)
                return;

            // Check all 8 neighboring cells
            for (int dr = -1; dr <= 1; dr++)
            {
                for (int dc = -1; dc <= 1; dc++)
                {
                    if (dr == 0 && dc == 0)
                        continue;

                    int newRow = row + dr;
                    int newCol = col + dc;

                    // Check bounds
                    if (newRow >= 0 && newRow < _rows && newCol >= 0 && newCol < _cols)
                    {
                        var adjacentCell = _board[newRow, newCol];
                        
                        // Only reveal if not already revealed and not flagged
                        if (!adjacentCell.IsRevealed && !adjacentCell.IsFlagged)
                        {
                            GameBoardService.RevealCell(adjacentCell);

                            // Recursively reveal if this cell also has no neighboring mines
                            if (adjacentCell.NeighboringMines == 0)
                            {
                                RevealAdjacentCells(adjacentCell);
                            }
                        }
                    }
                }
            }
        }

        // Find the position of a cell in the 2D board array
        private (int row, int col) FindCellPosition(CellModel cell)
        {
            if (_board == null)
                return (-1, -1);

            for (int row = 0; row < _rows; row++)
            {
                for (int col = 0; col < _cols; col++)
                {
                    if (_board[row, col] == cell)
                        return (row, col);
                }
            }
            return (-1, -1);
        }

        private void CheckWinCondition()
        {
            int revealedCount = 0;
            int totalNonMineCells = _rows * _cols - _mineCount;

            foreach (var cell in Cells)
            {
                if (cell.IsRevealed && !cell.IsMine) // CHANGE FROM: && cell.NeighboringMines != -1
                    revealedCount++;
            }

            // Win if all non-mine cells are revealed
            if (revealedCount == totalNonMineCells)
            {
                HandleGameOver(true);
            }
        }

        private void HandleGameOver(bool won)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"HandleGameOver - IsMainThread: {Microsoft.Maui.ApplicationModel.MainThread.IsMainThread}");
    
                _isGameOver = true;
                _gameTimer?.Dispose(); // STOP TIMER
                GameStatus = won ? "Won!" : "Lost!"; // UPDATE STATUS
    
                // Reveal all cells on main thread
                if (Microsoft.Maui.ApplicationModel.MainThread.IsMainThread)
                {
                    foreach (var cell in Cells)
                    {
                        if (!cell.IsRevealed)
                            cell.IsRevealed = true;
                    }
                }
                else
                {
                    Microsoft.Maui.ApplicationModel.MainThread.BeginInvokeOnMainThread(() =>
                    {
                        foreach (var cell in Cells)
                        {
                            if (!cell.IsRevealed)
                                cell.IsRevealed = true;
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"HandleGameOver error: {ex.GetType().Name}: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                throw;
            }
        }

        public enum Difficulty { Easy, Medium, Hard, Custom }

        public void SetDifficulty(Difficulty difficulty)
        {
            switch (difficulty)
            {
                case Difficulty.Easy:
                    _rows = 8; _cols = 8; _mineCount = 10;
                    break;
                case Difficulty.Medium:
                    _rows = 16; _cols = 16; _mineCount = 40;
                    break;
                case Difficulty.Hard:
                    _rows = 16; _cols = 30; _mineCount = 99;
                    break;
            }
            StartNewGame();
        }

        public void CalculateCellSize(double availableWidth, double availableHeight)
        {
            // Don't calculate if dimensions are invalid
            if (availableWidth <= 0 || availableHeight <= 0)
                return;

            // Calculate cell size based on available space
            const double minCellSize = 25;
            const double maxCellSize = 60;
            const double headerFooterHeight = 200; // Approximate space for header and footer
            const double pagePadding = 20; // Grid padding
            const double scrollBarBuffer = 20; // Space for potential scrollbars
            
            // Calculate usable space
            double usableWidth = availableWidth - pagePadding - scrollBarBuffer;
            double usableHeight = availableHeight - headerFooterHeight - pagePadding - scrollBarBuffer;
            
            // Calculate cell size based on grid dimensions
            double cellSizeByWidth = usableWidth / _cols;
            double cellSizeByHeight = usableHeight / _rows;
            
            // Use the smaller of the two to ensure the grid fits
            double calculatedSize = Math.Min(cellSizeByWidth, cellSizeByHeight);
            
            // Clamp between min and max
            double newCellSize = Math.Max(minCellSize, Math.Min(maxCellSize, calculatedSize));
            
            // Only update if there's a significant change (avoid excessive updates)
            if (Math.Abs(newCellSize - _cellSize) > 0.5)
            {
                CellSize = newCellSize;
                System.Diagnostics.Debug.WriteLine($"CellSize updated to: {CellSize:F2} (Width: {availableWidth:F0}, Height: {availableHeight:F0})");
            }
        }
    }
}
