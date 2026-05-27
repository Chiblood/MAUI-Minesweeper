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

        // Nautical symbols aligned with the MAUI implementation.
        public string DisplayText => _isMine && _isRevealed ? "⚓" :
                         _isFlagged ? "🚩" :
                         _isRevealed && _neighboringMines > 0 ? _neighboringMines.ToString() : "";

        public string CellColor
        {
            get
            {
                if (IsFlagged) return "#D4AC0D";
                if (!IsRevealed) return "#D6EAF8";
                if (IsMine) return "#C0392B";
                return NeighboringMines switch
                {
                    1 => "#1B4F72",
                    2 => "#17A589",
                    3 => "#E74C3C",
                    4 => "#0D1B2A",
                    5 => "#7B241C",
                    6 => "#0E7063",
                    7 => "#1A3050",
                    8 => "#717D7E",
                    _ => "#1A3050"
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
