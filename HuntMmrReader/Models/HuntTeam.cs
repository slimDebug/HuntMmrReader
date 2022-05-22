using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using HuntMmrReader.Models.BaseModels;

namespace HuntMmrReader.Models;

public class HuntTeam : HuntBaseEntity
{
    private readonly List<HuntPlayer> _members;

    public HuntTeam(ushort mmr, ushort id, bool inviteTeam, bool skillBasedMatchMakingEnabled,
        List<HuntPlayer> teamMembers) : base(mmr, id)
    {
        _members = teamMembers;
        RandomTeam = teamMembers.Count != 1 && !inviteTeam;
        SkillBasedMatchMakingEnabled = skillBasedMatchMakingEnabled;
    }

    public HuntTeam(string? mmr, ushort id, string? inviteTeam, string? skillBasedMatchMakingEnabled,
        List<HuntPlayer> teamMembers) : base(mmr, id)
    {
        _members = teamMembers;
        RandomTeam = !(bool.TryParse(inviteTeam, out var parsedInviteTeam) && parsedInviteTeam) &&
                     teamMembers.Count != 1;
        SkillBasedMatchMakingEnabled =
            bool.TryParse(skillBasedMatchMakingEnabled, out var parsedSkillBasedMatchMakingEnabled) &&
            parsedSkillBasedMatchMakingEnabled;
    }

    public bool SkillBasedMatchMakingEnabled { get; }

    public bool RandomTeam { get; }

    public IReadOnlyCollection<HuntPlayer> Members =>
        new List<HuntPlayer>(_members.OrderBy(member => member.Id)).AsReadOnly();

    public int TeamSize => _members.Count;

    public override string ToString()
    {
        return
            $"Team: {Id.ToString(CultureInfo.InvariantCulture),2} | MMR: {Mmr.ToString(CultureInfo.InvariantCulture),4} | Size: {TeamSize.ToString(CultureInfo.InvariantCulture)}";
    }
}