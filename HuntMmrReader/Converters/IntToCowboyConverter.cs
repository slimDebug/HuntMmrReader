using System;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Data;

namespace HuntMmrReader.Converters;

internal class IntToCowboyConverter : IValueConverter
{
    private const string CowboyEmoticon = "\uD83E\uDD20";

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is int intValue)
            return new StringBuilder(CowboyEmoticon.Length * intValue).Insert(0, CowboyEmoticon, intValue).ToString();

        return string.Empty;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string tempStringValue)
            return Regex.Matches(tempStringValue, CowboyEmoticon).Count;
        return 0;
    }
}