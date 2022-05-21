using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using HuntMmrReader.Models.BaseModels;

namespace HuntMmrReader.Converters;

internal abstract class MmrConverterBase : IValueConverter
{
    private static readonly Brush CloseToUpRankBrush = Brushes.Green;
    private static readonly Brush CloseToDeRankBrush = Brushes.Red;
    private static readonly Brush NeutralRankBrush = Brushes.Orange;

    public abstract object Convert(object value, Type targetType, object parameter, CultureInfo culture);
    public abstract object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture);

    protected static ushort GetStarsFromMmr(ushort mmr)
    {
        return mmr switch
        {
            >= HuntBaseEntity.SixStarMinimumMmr => 6,
            >= HuntBaseEntity.FiveStarMinimumMmr => 5,
            >= HuntBaseEntity.FourStarMinimumMmr => 4,
            >= HuntBaseEntity.ThreeStarMinimumMmr => 3,
            >= HuntBaseEntity.TwoStarMinimumMmr => 2,
            _ => 1
        };
    }

    protected static Brush GetBrushPrognosis(ushort mmr)
    {
        var stars = GetStarsFromMmr(mmr);
        return stars switch
        {
            1 => CloseToUpRankBrush,
            2 => HuntBaseEntity.ThreeStarMinimumMmr - mmr < mmr - HuntBaseEntity.TwoStarMinimumMmr
                ? CloseToUpRankBrush
                : HuntBaseEntity.ThreeStarMinimumMmr - mmr > mmr - HuntBaseEntity.TwoStarMinimumMmr
                    ? CloseToDeRankBrush
                    : NeutralRankBrush,
            3 => HuntBaseEntity.FourStarMinimumMmr - mmr < mmr - HuntBaseEntity.ThreeStarMinimumMmr
                ? CloseToUpRankBrush
                : HuntBaseEntity.FourStarMinimumMmr - mmr > mmr - HuntBaseEntity.ThreeStarMinimumMmr
                    ? CloseToDeRankBrush
                    : NeutralRankBrush,
            4 => HuntBaseEntity.FiveStarMinimumMmr - mmr < mmr - HuntBaseEntity.FourStarMinimumMmr
                ? CloseToUpRankBrush
                : HuntBaseEntity.FiveStarMinimumMmr - mmr > mmr - HuntBaseEntity.FourStarMinimumMmr
                    ? CloseToDeRankBrush
                    : NeutralRankBrush,
            5 => HuntBaseEntity.SixStarMinimumMmr - mmr < mmr - HuntBaseEntity.FiveStarMinimumMmr
                ? CloseToUpRankBrush
                : HuntBaseEntity.SixStarMinimumMmr - mmr > mmr - HuntBaseEntity.FiveStarMinimumMmr
                    ? CloseToDeRankBrush
                    : NeutralRankBrush,
            _ => CloseToUpRankBrush
        };
    }

    protected static ushort GetUpRankDeRankPrognosis(ushort mmr)
    {
        var stars = GetStarsFromMmr(mmr);
        return stars switch
        {
            1 => (ushort) (HuntBaseEntity.TwoStarMinimumMmr - mmr),
            2 => (ushort) (HuntBaseEntity.ThreeStarMinimumMmr - mmr <= mmr - HuntBaseEntity.TwoStarMinimumMmr
                ? HuntBaseEntity.ThreeStarMinimumMmr - mmr
                : 1 + mmr - HuntBaseEntity.TwoStarMinimumMmr),
            3 => (ushort) (HuntBaseEntity.FourStarMinimumMmr - mmr <= mmr - HuntBaseEntity.ThreeStarMinimumMmr
                ? HuntBaseEntity.FourStarMinimumMmr - mmr
                : 1 + mmr - HuntBaseEntity.ThreeStarMinimumMmr),
            4 => (ushort) (HuntBaseEntity.FiveStarMinimumMmr - mmr <= mmr - HuntBaseEntity.FourStarMinimumMmr
                ? HuntBaseEntity.FiveStarMinimumMmr - mmr
                : 1 + mmr - HuntBaseEntity.FourStarMinimumMmr),
            5 => (ushort) (HuntBaseEntity.SixStarMinimumMmr - mmr <= mmr - HuntBaseEntity.FiveStarMinimumMmr
                ? HuntBaseEntity.SixStarMinimumMmr - mmr
                : 1 + mmr - HuntBaseEntity.FiveStarMinimumMmr),
            _ => (ushort) (1 + mmr - HuntBaseEntity.SixStarMinimumMmr)
        };
    }

    protected static ushort GetClosestRank(ushort mmr)
    {
        var stars = GetStarsFromMmr(mmr);
        return stars switch
        {
            1 => 2,
            2 => (ushort) (HuntBaseEntity.ThreeStarMinimumMmr - mmr < mmr - HuntBaseEntity.TwoStarMinimumMmr
                ? 3
                : HuntBaseEntity.ThreeStarMinimumMmr - mmr > mmr - HuntBaseEntity.TwoStarMinimumMmr
                    ? 2
                    : 3),
            3 => (ushort) (HuntBaseEntity.FourStarMinimumMmr - mmr < mmr - HuntBaseEntity.ThreeStarMinimumMmr
                ? 4
                : HuntBaseEntity.FourStarMinimumMmr - mmr > mmr - HuntBaseEntity.ThreeStarMinimumMmr
                    ? 3
                    : 4),
            4 => (ushort) (HuntBaseEntity.FiveStarMinimumMmr - mmr < mmr - HuntBaseEntity.FourStarMinimumMmr
                ? 5
                : HuntBaseEntity.FiveStarMinimumMmr - mmr > mmr - HuntBaseEntity.FourStarMinimumMmr
                    ? 4
                    : 5),
            5 => (ushort) (HuntBaseEntity.SixStarMinimumMmr - mmr < mmr - HuntBaseEntity.FiveStarMinimumMmr
                ? 6
                : HuntBaseEntity.SixStarMinimumMmr - mmr > mmr - HuntBaseEntity.FiveStarMinimumMmr
                    ? 5
                    : 6),
            _ => 5
        };
    }
}