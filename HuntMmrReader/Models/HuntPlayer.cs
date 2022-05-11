using System.Globalization;
using HuntMmrReader.Models.BaseModels;

namespace HuntMmrReader.Models;

internal class HuntPlayer : HuntBaseEntity
{
    internal HuntPlayer(string? name, ushort mmr, bool hadBounty, bool hadWellSpring,
        ushort killedByMe, ushort killedMe,
        ushort playerId) : base(mmr, playerId)
    {
        Name = name;
        HadBounty = hadBounty;
        HadWellSpring = hadWellSpring;
        KilledByMe = killedByMe;
        KilledMe = killedMe;
    }

    internal HuntPlayer(string? name, string? mmr, string? hadBounty,
        string? hadWellSpring, string? killedByMe, string? killedMe, ushort playerId) : base(mmr, playerId)
    {
        Name = name;
        HadBounty = bool.TryParse(hadBounty, out var parsedHadBounty) && parsedHadBounty;
        HadWellSpring = bool.TryParse(hadWellSpring, out var parsedHadWellSpring) && parsedHadWellSpring;
        KilledByMe =
            ushort.TryParse(killedByMe, NumberStyles.Integer, CultureInfo.InvariantCulture, out var parsedKilledByMe)
                ? parsedKilledByMe
                : ushort.MinValue;
        KilledMe = ushort.TryParse(killedMe, NumberStyles.Integer, CultureInfo.InvariantCulture, out var parsedKilledMe)
            ? parsedKilledMe
            : ushort.MinValue;
    }

    public string? Name { get; }

    public bool HadBounty { get; }
    public bool HadWellSpring { get; }
    public ushort KilledByMe { get; }
    public ushort KilledMe { get; }

    public override string ToString()
    {
        return
            $"Player Name: {Name ?? "",-24} | MMR: {Mmr.ToString(CultureInfo.InvariantCulture),4}";
    }
}