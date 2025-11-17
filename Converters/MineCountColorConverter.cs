using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using System;
using System.Globalization;

namespace MAUI_Minesweeper.Converters
{
    public class MineCountColorConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is int count)
            {
                return count switch
                {
                    -1 => Colors.Red,      // Mine
                    0 => Colors.Transparent,
                    1 => Colors.Blue,
                    2 => Colors.Green,
                    3 => Colors.Red,
                    4 => Colors.DarkBlue,
                    5 => Colors.DarkRed,
                    6 => Colors.Cyan,
                    7 => Colors.Black,
                    8 => Colors.Gray,
                    _ => Colors.Black
                };
            }
            return Colors.Black;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}