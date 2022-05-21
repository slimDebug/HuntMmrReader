using System;
using System.Globalization;

namespace HuntMmrReader.Converters;

internal class MmrToPrognosisStringConverter : MmrConverterBase
{
    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is not ushort ushortValue
            ? string.Empty
            : $"This hunter is only {GetUpRankDeRankPrognosis(ushortValue).ToString(CultureInfo.InvariantCulture)} MMR points away, before becoming a {GetClosestRank(ushortValue).ToString(CultureInfo.InvariantCulture)} star hunter.";
    }

    public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}