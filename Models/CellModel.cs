using CommunityToolkit.Mvvm.ComponentModel;

namespace MAUI_Minesweeper.Models
{
    public partial class CellModel : ObservableObject
    {
        public bool IsRevealed
        {
            get => _isRevealed;
            set
            {
                if (SetProperty(ref _isRevealed, value))
                {
                    OnPropertyChanged(nameof(DisplayText));
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
                    OnPropertyChanged(nameof(DisplayText));
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
                    OnPropertyChanged(nameof(DisplayText));
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
                    OnPropertyChanged(nameof(DisplayText));
                }
            }
        }
        private bool _isMine;

        // Nautical theme display symbols.
        // ⚓ = anchor (mine / danger)   🚩 = flag (marker)
        // To replace with custom Image assets: expose IsMine/IsFlagged directly
        // and bind Image.Source in the DataTemplate instead of this Label text.
        public string DisplayText => _isMine && _isRevealed ? "⚓" : 
                                     _isFlagged ? "🚩" : 
                                     _isRevealed && _neighboringMines > 0 ? _neighboringMines.ToString() : "";
    }
}
