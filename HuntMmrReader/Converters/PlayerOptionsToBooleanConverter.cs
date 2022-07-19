using System;
using System.Globalization;
using System.Windows.Data;
using HuntMmrReader.Enums;

namespace HuntMmrReader.Converters;

internal class PlayerOptionsToBooleanConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is PlayerOptions playerOptionsValue && parameter is PlayerOptions playerOptionsParameter)
            return playerOptionsValue.HasFlag(playerOptionsParameter);
        return false;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}