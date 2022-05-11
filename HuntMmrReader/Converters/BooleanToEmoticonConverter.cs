using System;
using System.Globalization;
using System.Windows.Data;

namespace HuntMmrReader.Converters;

internal class BooleanToEmoticonConverter : IValueConverter
{
    private const char CheckMarkEmoticon = '\u2705';
    private const char CrossEmoticon = '\u274C';

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool boolValue) return boolValue ? CheckMarkEmoticon : CrossEmoticon;

        return CrossEmoticon;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is char tempCharValue)
            return tempCharValue == CheckMarkEmoticon;
        return false;
    }
}