using System.ComponentModel;

namespace BlazorMinesweeper.Shared.Models
{
    public class CellModel : INotifyPropertyChanged
    {
        private bool _isRevealed;
        private bool _isFlagged;
        private int _neighboringMines;
        private bool _isMine;

        public bool IsRevealed
        {
            get => _isRevealed;
            set
            {
                if (_isRevealed != value)
                {
                    _isRevealed = value;
                    OnPropertyChanged(nameof(IsRevealed));
                    OnPropertyChanged(nameof(DisplayText));
                }
            }
        }

        public bool IsFlagged
        {
            get => _isFlagged;
            set
            {
                if (_isFlagged != value)
                {
                    _isFlagged = value;
                    OnPropertyChanged(nameof(IsFlagged));
                    OnPropertyChanged(nameof(DisplayText));
                }
            }
        }

        public int NeighboringMines
        {
            get => _neighboringMines;
            set
            {
                if (_neighboringMines != value)
                {
                    _neighboringMines = value;
                    OnPropertyChanged(nameof(NeighboringMines));
                    OnPropertyChanged(nameof(DisplayText));
                }
            }
        }

        public bool IsMine
        {
            get => _isMine;
            set
            {
                if (_isMine != value)
                {
                    _isMine = value;
                    OnPropertyChanged(nameof(IsMine));
                    OnPropertyChanged(nameof(DisplayText));
                }
            }
        }

        // FOR UI CONVENIENCE
        public string DisplayText => _isMine && _isRevealed ? "??" :
                                     _isFlagged ? "??" :
                                     _isRevealed && _neighboringMines > 0 ? _neighboringMines.ToString() : "";

        public string CellColor
        {
            get
            {
                if (!IsRevealed) return "#aaa";
                if (IsMine) return "#f44336";
                return NeighboringMines switch
                {
                    1 => "#2196F3",
                    2 => "#4CAF50",
                    3 => "#FF9800",
                    4 => "#9C27B0",
                    5 => "#F44336",
                    6 => "#00BCD4",
                    7 => "#000000",
                    8 => "#757575",
                    _ => "#fff"
                };
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
