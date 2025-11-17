using MAUI_Minesweeper.ViewModels;
using MAUI_Minesweeper.Models;
using Microsoft.Maui.Controls;

namespace MAUI_Minesweeper.Views
{
    public partial class GamePage : ContentPage
    {
        private readonly GameViewModel _viewModel;

        public GamePage(GameViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            BindingContext = _viewModel;
            
            System.Diagnostics.Debug.WriteLine("=== GamePage initialized ===");
            
            // Subscribe to size changes
            this.SizeChanged += OnPageSizeChanged;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            
            // Trigger initial size calculation when page appears
            if (this.Width > 0 && this.Height > 0)
            {
                _viewModel.CalculateCellSize(this.Width, this.Height);
            }
        }

        private void OnPageSizeChanged(object? sender, EventArgs e)
        {
            // Calculate cell size based on available space
            if (this.Width > 0 && this.Height > 0)
            {
                _viewModel.CalculateCellSize(this.Width, this.Height);
            }
        }

        private void OnDifficultyChanged(object sender, EventArgs e)
        {
            var picker = (Picker)sender;
            var selectedIndex = picker.SelectedIndex;

            if (selectedIndex != -1)
            {
                var difficulty = selectedIndex switch
                {
                    0 => GameViewModel.Difficulty.Easy,
                    1 => GameViewModel.Difficulty.Medium,
                    2 => GameViewModel.Difficulty.Hard,
                    _ => GameViewModel.Difficulty.Easy
                };

                _viewModel.SetDifficulty(difficulty);
            }
        }

        private void OnCellLeftClicked(object sender, TappedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("=== OnCellLeftClicked CALLED ===");
            System.Diagnostics.Debug.WriteLine($"Sender type: {sender?.GetType().Name}");
            System.Diagnostics.Debug.WriteLine($"TappedEventArgs - Buttons: {e.Buttons}");
            
            if (sender is Grid grid && grid.BindingContext is CellModel cell)
            {
                System.Diagnostics.Debug.WriteLine($"Cell found - IsMine: {cell.IsMine}, IsRevealed: {cell.IsRevealed}, IsFlagged: {cell.IsFlagged}");
                System.Diagnostics.Debug.WriteLine(">>> LEFT-CLICK - Executing CellTappedCommand (Reveal)");
                
                _viewModel.CellTappedCommand.Execute(cell);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("!!! ERROR: Sender is not Grid or BindingContext is not CellModel");
            }
            
            System.Diagnostics.Debug.WriteLine("=== OnCellLeftClicked COMPLETED ===\n");
        }

        private void OnCellRightClicked(object sender, TappedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("=== OnCellRightClicked CALLED ===");
            System.Diagnostics.Debug.WriteLine($"Sender type: {sender?.GetType().Name}");
            System.Diagnostics.Debug.WriteLine($"TappedEventArgs - Buttons: {e.Buttons}");
            
            if (sender is Grid grid && grid.BindingContext is CellModel cell)
            {
                System.Diagnostics.Debug.WriteLine($"Cell found - IsMine: {cell.IsMine}, IsRevealed: {cell.IsRevealed}, IsFlagged: {cell.IsFlagged}");
                System.Diagnostics.Debug.WriteLine(">>> RIGHT-CLICK - Executing CellFlaggedCommand (Toggle Flag)");
                
                _viewModel.CellFlaggedCommand.Execute(cell);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("!!! ERROR: Sender is not Grid or BindingContext is not CellModel");
            }
            
            System.Diagnostics.Debug.WriteLine("=== OnCellRightClicked COMPLETED ===\n");
        }
    }
}