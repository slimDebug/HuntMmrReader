using System.Globalization;

namespace HuntMmrReader.Models.BaseModels;

internal abstract class HuntBaseEntity
{
    internal HuntBaseEntity(ushort mmr, ushort id)
    {
        Mmr = mmr;
        Id = id;
    }

    internal HuntBaseEntity(string? mmr, ushort id)
    {
        Mmr = ushort.TryParse(mmr, NumberStyles.Integer, CultureInfo.InvariantCulture, out var parsedMmr)
            ? parsedMmr
            : ushort.MinValue;
        Id = id;
    }

    public ushort Mmr { get; }
    public ushort Id { get; }
}