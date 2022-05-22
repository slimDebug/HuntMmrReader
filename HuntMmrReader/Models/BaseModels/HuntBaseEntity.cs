using System.Globalization;

namespace HuntMmrReader.Models.BaseModels;

public abstract class HuntBaseEntity
{
    public const ushort OneStarMinimumMmr = 0;
    public const ushort TwoStarMinimumMmr = 2000;
    public const ushort ThreeStarMinimumMmr = 2300;
    public const ushort FourStarMinimumMmr = 2600;
    public const ushort FiveStarMinimumMmr = 2750;
    public const ushort SixStarMinimumMmr = 3000;

    protected HuntBaseEntity(ushort mmr, ushort id)
    {
        Mmr = mmr;
        Id = id;
    }

    protected HuntBaseEntity(string? mmr, ushort id)
    {
        Mmr = ushort.TryParse(mmr, NumberStyles.Integer, CultureInfo.InvariantCulture, out var parsedMmr)
            ? parsedMmr
            : ushort.MinValue;
        Id = id;
    }

    public ushort Mmr { get; }
    public ushort Id { get; }
}