using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using System;
using System.Globalization;

namespace MAUI_Minesweeper.Converters
{
    public class CellBackgroundConverter : IValueConverter
    {
        // Nautical theme: ocean-blue for unrevealed, sandy parchment for revealed.
        // These values mirror CellUnrevealed / CellRevealed in Colors.xaml so
        // that a designer can adjust both files in tandem.  Referencing XAML
        // resources at converter construction time is avoided deliberately to
        // keep the converter testable without a running MAUI application host.
        private static readonly Color Unrevealed = Color.FromArgb("#1B4F72"); // deep ocean blue
        private static readonly Color Revealed    = Color.FromArgb("#EDE0C4"); // sandy parchment

        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is bool isRevealed)
            {
                return isRevealed ? Revealed : Unrevealed;
            }
            return Unrevealed;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}