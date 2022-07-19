using System.Globalization;
using HuntMmrReader.Models.BaseModels;

namespace HuntMmrReader.Models;

public class HuntPlayer : HuntBaseEntity
{
    public HuntPlayer(ushort id, ushort mmr, string bloodLineName, ushort bountyExtracted, ushort bountyPickedUp,
        ushort downedByMe, ushort downedByTeammate, ushort downedMe, ushort downedTeammate, bool hadWellspring,
        bool hadBounty, bool isPartner, bool isSoulSurvivor, ushort killedByMe, ushort killedByTeammate,
        ushort killedMe, ushort killedTeammate, ulong profileId,
        bool proximity, bool proximityToMe, bool proximityToTeammate, bool skillBased, bool teamExtraction) : base(mmr,
        id)
    {
        BloodLineName = bloodLineName;
        BountyExtracted = bountyExtracted;
        BountyPickedUp = bountyPickedUp;
        DownedByMe = downedByMe;
        DownedByTeammate = downedByTeammate;
        DownedMe = downedMe;
        DownedTeammate = downedTeammate;
        HadWellspring = hadWellspring;
        HadBounty = hadBounty;
        IsPartner = isPartner;
        IsSoulSurvivor = isSoulSurvivor;
        KilledByMe = killedByMe;
        KilledByTeammate = killedByTeammate;
        KilledMe = killedMe;
        KilledTeammate = killedTeammate;
        ProfileId = profileId;
        Proximity = proximity;
        ProximityToMe = proximityToMe;
        ProximityToTeammate = proximityToTeammate;
        SkillBased = skillBased;
        TeamExtraction = teamExtraction;
    }

    public HuntPlayer(ushort id, string? mmr, string? bloodLineName, string? bountyExtracted, string? bountyPickedUp,
        string? downedByMe, string? downedByTeammate, string? downedMe, string? downedTeammate, string? hadWellspring,
        string? hadBounty, string? isPartner, string? isSoulSurvivor, string? killedByMe, string? killedByTeammate,
        string? killedMe, string? killedTeammate, string? profileId, string? proximity, string? proximityToMe,
        string? proximityToTeammate, string? skillBased, string? teamExtraction) : base(mmr, id)
    {
        BloodLineName = bloodLineName ?? string.Empty;
        BountyExtracted = ParseUInt16(bountyExtracted);
        BountyPickedUp = ParseUInt16(bountyPickedUp);
        DownedByMe = ParseUInt16(downedByMe);
        DownedByTeammate = ParseUInt16(downedByTeammate);
        DownedMe = ParseUInt16(downedMe);
        DownedTeammate = ParseUInt16(downedTeammate);
        HadWellspring = ParseBoolean(hadWellspring);
        HadBounty = ParseBoolean(hadBounty);
        IsPartner = ParseBoolean(isPartner);
        IsSoulSurvivor = ParseBoolean(isSoulSurvivor);
        KilledByMe = ParseUInt16(killedByMe);
        KilledByTeammate = ParseUInt16(killedByTeammate);
        KilledMe = ParseUInt16(killedMe);
        KilledTeammate = ParseUInt16(killedTeammate);
        ProfileId = ParseUInt64(profileId);
        Proximity = ParseBoolean(proximity);
        ProximityToMe = ParseBoolean(proximityToMe);
        ProximityToTeammate = ParseBoolean(proximityToTeammate);
        SkillBased = ParseBoolean(skillBased);
        TeamExtraction = ParseBoolean(teamExtraction);
    }

    public string BloodLineName { get; }
    public ushort BountyExtracted { get; }
    public ushort BountyPickedUp { get; }
    public ushort DownedByMe { get; }
    public ushort DownedByTeammate { get; }
    public ushort DownedMe { get; }
    public ushort DownedTeammate { get; }
    public bool HadWellspring { get; }
    public bool HadBounty { get; }
    public bool IsPartner { get; }
    public bool IsSoulSurvivor { get; }
    public ushort KilledByMe { get; }
    public ushort KilledByTeammate { get; }
    public ushort KilledMe { get; }
    public ushort KilledTeammate { get; }
    public ulong ProfileId { get; }
    public bool Proximity { get; }
    public bool ProximityToMe { get; }
    public bool ProximityToTeammate { get; }
    public bool SkillBased { get; }
    public bool TeamExtraction { get; }

    public override string ToString()
    {
        return $"Player Name: {BloodLineName,-24} | MMR: {Mmr.ToString(CultureInfo.InvariantCulture),4}";
    }
}