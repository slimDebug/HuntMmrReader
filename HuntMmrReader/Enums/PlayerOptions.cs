using System;
using System.ComponentModel;

namespace HuntMmrReader.Enums;

/// <summary>
///     Player options
/// </summary>
[Flags]
internal enum PlayerOptions
{
    ///<summary>Default value.</summary>
    None = 0,

    ///<summary>This should be a string.</summary>
    [Description("blood_line_name")] BloodLineName = 1,

    ///<summary>This should be a ushort.</summary>
    [Description("bountyextracted")] BountyExtracted = 2,

    ///<summary>This should be a ushort.</summary>
    [Description("bountypickedup")] BountyPickedUp = 4,

    ///<summary>This should be a ushort.</summary>
    [Description("downedbyme")] DownedByMe = 8,

    ///<summary>This should be a ushort.</summary>
    [Description("downedbyteammate")] DownedByTeammate = 16,

    ///<summary>This should be a ushort.</summary>
    [Description("downedme")] DownedMe = 32,

    ///<summary>This should be a ushort.</summary>
    [Description("downedteammate")] DownedTeammate = 64,

    ///<summary>This should be a bool.</summary>
    [Description("hadWellspring")] HadWellspring = 128,

    ///<summary>This should be a bool.</summary>
    [Description("hadbounty")] HadBounty = 256,

    ///<summary>This should be a bool.</summary>
    [Description("ispartner")] IsPartner = 512,

    ///<summary>This should be a bool.</summary>
    [Description("issoulsurvivor")] IsSoulSurvivor = 1024,

    ///<summary>This should be a ushort.</summary>
    [Description("killedbyme")] KilledByMe = 2048,

    ///<summary>This should be a ushort.</summary>
    [Description("killedbyteammate")] KilledByTeammate = 4096,

    ///<summary>This should be a ushort.</summary>
    [Description("killedme")] KilledMe = 8192,

    ///<summary>This should be a ushort.</summary>
    [Description("killedteammate")] KilledTeammate = 16384,

    ///<summary>This should be a ushort.</summary>
    [Description("mmr")] Mmr = 32768,

    ///<summary>This should be a ulong.</summary>
    [Description("profileid")] ProfileId = 65536,

    ///<summary>This should be a bool.</summary>
    [Description("proximity")] Proximity = 131072,

    ///<summary>This should be a bool.</summary>
    [Description("proximitytome")] ProximityToMe = 262144,

    ///<summary>This should be a bool.</summary>
    [Description("proximitytoteammate")] ProximityToTeammate = 524288,

    ///<summary>This should be a bool.</summary>
    [Description("skillbased")] SkillBased = 1048576,

    ///<summary>This should be a bool.</summary>
    [Description("teamextraction")] TeamExtraction = 2097152,

    ///<summary>All values combined.</summary>
    All = BloodLineName | BountyExtracted | BountyPickedUp | DownedByMe | DownedByTeammate | DownedMe | DownedTeammate |
          HadWellspring | HadBounty | IsPartner | IsSoulSurvivor | KilledByMe | KilledByTeammate | KilledMe |
          KilledTeammate | Mmr | ProfileId | Proximity | ProximityToMe | ProximityToTeammate | SkillBased |
          TeamExtraction
}