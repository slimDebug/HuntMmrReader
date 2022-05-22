using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace HuntMmrReader.Converters;

internal class IsEqualOrGreaterThanUshortVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is ushort ushortValue && parameter is ushort ushortParameter)
            return ushortValue >= ushortParameter ? Visibility.Visible : Visibility.Hidden;

        return Visibility.Hidden;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}