using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using System;
using System.Globalization;

namespace MAUI_Minesweeper.Converters
{
    public class MineCountColorConverter : IValueConverter
    {
        // Nautical-themed number colours — optimised for contrast on the sandy
        // parchment revealed-cell background (#EDE0C4).
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is int count)
            {
                return count switch
                {
                    -1 => Color.FromArgb("#C0392B"),  // mine  – coral red
                    0  => Colors.Transparent,
                    1  => Color.FromArgb("#1B4F72"),  // 1 – deep ocean blue
                    2  => Color.FromArgb("#17A589"),  // 2 – sea teal
                    3  => Color.FromArgb("#E74C3C"),  // 3 – coral
                    4  => Color.FromArgb("#0D1B2A"),  // 4 – deep navy
                    5  => Color.FromArgb("#7B241C"),  // 5 – dark crimson
                    6  => Color.FromArgb("#0E7063"),  // 6 – dark teal
                    7  => Color.FromArgb("#1A3050"),  // 7 – midnight blue
                    8  => Color.FromArgb("#717D7E"),  // 8 – sea grey
                    _  => Color.FromArgb("#0D1B2A")
                };
            }
            return Color.FromArgb("#0D1B2A");
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}