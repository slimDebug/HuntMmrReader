using System;
using System.Globalization;
using System.Windows.Media;

namespace HuntMmrReader.Converters;

internal class MmrToColorConverter : MmrConverterBase
{
    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is ushort ushortValue)
            return GetBrushPrognosis(ushortValue);
        return Brushes.Orange;
    }

    public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}