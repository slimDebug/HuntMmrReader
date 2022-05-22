using System;
using System.Globalization;
using HuntMmrReader.Models.BaseModels;

namespace HuntMmrReader.Converters;

internal class MmrToMmrRangeString : MmrConverterBase
{
    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not ushort ushortValue)
            return string.Empty;
        var stars = GetStarsFromMmr(ushortValue);
        var baseString =
            $"This is a {stars.ToString(CultureInfo.InvariantCulture)} star hunter.{Environment.NewLine}The MMR range of a {stars.ToString(CultureInfo.InvariantCulture)} star hunter goes from ";
        return stars switch
        {
            1 =>
                $"{baseString}{HuntBaseEntity.OneStarMinimumMmr.ToString(CultureInfo.InvariantCulture)}-{(HuntBaseEntity.TwoStarMinimumMmr - 1).ToString(CultureInfo.InvariantCulture)}.",
            2 =>
                $"{baseString}{HuntBaseEntity.TwoStarMinimumMmr.ToString(CultureInfo.InvariantCulture)}-{(HuntBaseEntity.ThreeStarMinimumMmr - 1).ToString(CultureInfo.InvariantCulture)}.",
            3 =>
                $"{baseString}{HuntBaseEntity.ThreeStarMinimumMmr.ToString(CultureInfo.InvariantCulture)}-{(HuntBaseEntity.FourStarMinimumMmr - 1).ToString(CultureInfo.InvariantCulture)}.",
            4 =>
                $"{baseString}{HuntBaseEntity.FourStarMinimumMmr.ToString(CultureInfo.InvariantCulture)}-{(HuntBaseEntity.FiveStarMinimumMmr - 1).ToString(CultureInfo.InvariantCulture)}.",
            5 =>
                $"{baseString}{HuntBaseEntity.FiveStarMinimumMmr.ToString(CultureInfo.InvariantCulture)}-{(HuntBaseEntity.SixStarMinimumMmr - 1).ToString(CultureInfo.InvariantCulture)}.",
            _ => $"{baseString}{HuntBaseEntity.SixStarMinimumMmr.ToString(CultureInfo.InvariantCulture)}-{'\u221E'}."
        };
    }

    public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}