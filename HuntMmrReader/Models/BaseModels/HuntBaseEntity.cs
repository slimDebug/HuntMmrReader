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
        Mmr = ParseUInt16(mmr);
        Id = id;
    }

    public ushort Mmr { get; }
    public ushort Id { get; }

    protected static bool ParseBoolean(string? input)
    {
        return bool.TryParse(input, out var parsedInput) && parsedInput;
    }

    protected static ushort ParseUInt16(string? input)
    {
        return ushort.TryParse(input, NumberStyles.Integer, CultureInfo.InvariantCulture, out var parsedInput)
            ? parsedInput
            : ushort.MinValue;
    }

    protected static ulong ParseUInt64(string? input)
    {
        return ulong.TryParse(input, NumberStyles.Integer, CultureInfo.InvariantCulture, out var parsedInput)
            ? parsedInput
            : ulong.MinValue;
    }
}