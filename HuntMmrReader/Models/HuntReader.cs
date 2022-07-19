using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using System.Xml.Linq;
using HuntMmrReader.DesignHelper;
using HuntMmrReader.Enums;
using HuntMmrReader.Extensions;

namespace HuntMmrReader.Models;

internal class HuntReader : ObservableObject, IDisposable
{
    internal const ushort MaxPlayers = 12;
    internal const ushort MaxTeams = MaxPlayers;
    private readonly TimeSpan _readDelay;
    private readonly Timer _readTimer;

    private readonly object _syncLastReadTimeObject = new();
    private readonly object _syncPathObject = new();
    private readonly ConcurrentBag<HuntTeam> _teams = new();
    private DateTime _lastModificationTime;
    private DateTime _lastReadTime;
    private bool _readSinceLastModification;
    private FileSystemWatcher? _watcher;

    internal HuntReader(TimeSpan readDelay)
    {
        _readSinceLastModification = false;
        _readDelay = readDelay;
        _lastReadTime = DateTime.Now;
        _lastModificationTime = _lastReadTime;
        _readTimer = new Timer {AutoReset = true, Interval = readDelay.TotalMilliseconds};
        _readTimer.Elapsed += ReadTimerElapsed;
        _readTimer.Start();
    }

    internal string? FilePath { get; private set; }

    internal IReadOnlyCollection<HuntTeam> Teams =>
        new List<HuntTeam>(_teams.OrderBy(team => team.Id)).AsReadOnly();

    internal DateTime LastReadTime
    {
        get => _lastReadTime;
        set
        {
            lock (_syncLastReadTimeObject)
            {
                _lastReadTime = value;
                OnPropertyChanged();
            }
        }
    }

    public void Dispose()
    {
        _readTimer.Dispose();
        _watcher?.Dispose();
    }

    private void ReadTimerElapsed(object? sender, ElapsedEventArgs e)
    {
        if (string.IsNullOrEmpty(FilePath) || _lastModificationTime.Add(_readDelay) >= DateTime.Now ||
            _readSinceLastModification)
            return;
        try
        {
            Read(FilePath);
            LastReadTime = DateTime.Now;
            _readSinceLastModification = true;
        }
        catch (Exception ex)
        {
            OnExceptionRaised(ex);
        }
    }


    private IEnumerable<HuntTeam> GetPlayersFromDoc(XContainer doc)
    {
        var teams = new List<HuntTeam>();
        ushort teamCounter = 0;
        ushort? teamsLastMatch = default;
        while (teamCounter <= MaxTeams && (teamsLastMatch == default || teamCounter < teamsLastMatch))
        {
            if (teams.Count >= MaxPlayers)
                break;

            var tempTeamPlayerCount = $"MissionBagTeam_{teamCounter.ToString(CultureInfo.InvariantCulture)}_numplayers";

            ushort teamPlayersCount;
            try
            {
                if (teamsLastMatch == default)
                    try
                    {
                        var teamsAmountElement = GetAttributeByName(doc, "MissionBagNumTeams");
                        if (teamsAmountElement != default)
                        {
                            if (ushort.TryParse(teamsAmountElement.Value, NumberStyles.Integer,
                                    CultureInfo.InvariantCulture, out var tempTeamsLastMatch))
                                teamsLastMatch = tempTeamsLastMatch;
                        }
                        else
                        {
                            return teams;
                        }
                    }
                    catch (Exception e)
                    {
                        OnExceptionRaised(e);
                        return teams;
                    }

                var teamPlayersCountElement = GetAttributeByName(doc, tempTeamPlayerCount);
                if (teamPlayersCountElement != default &&
                    ushort.TryParse(teamPlayersCountElement.Value, NumberStyles.Integer, CultureInfo.InvariantCulture,
                        out var tempTeamPlayersCount))
                {
                    teamPlayersCount = tempTeamPlayersCount;
                }
                else
                {
                    teamCounter++;
                    continue;
                }
            }
            catch (Exception)
            {
                teamCounter++;
                continue;
            }

            var players = new List<HuntPlayer>();
            XAttribute? teamMmrElement;
            XAttribute? inviteTeamElement;
            XAttribute? ownTeamElement;
            try
            {
                var baseTempTeamString = $"MissionBagTeam_{teamCounter.ToString(CultureInfo.InvariantCulture)}_";
                teamMmrElement = GetAttributeByName(doc,
                    $"{baseTempTeamString}mmr");
                inviteTeamElement = GetAttributeByName(doc, $"{baseTempTeamString}isinvite");
                ownTeamElement = GetAttributeByName(doc, $"{baseTempTeamString}ownteam");
            }
            catch (Exception e)
            {
                OnExceptionRaised(e);
                return teams;
            }

            for (ushort i = 0; i < teamPlayersCount; i++)
            {
                var baseTempPlayerString =
                    $"MissionBagPlayer_{teamCounter.ToString(CultureInfo.InvariantCulture)}_{i.ToString(CultureInfo.InvariantCulture)}_";
                XAttribute? bloodLineNameElement;
                XAttribute? bountyExtractedElement;
                XAttribute? bountyPickedUpElement;
                XAttribute? downedByMeElement;
                XAttribute? downedByTeammateElement;
                XAttribute? downedMeElement;
                XAttribute? downedTeammateElement;
                XAttribute? hadWellspringElement;
                XAttribute? hadBountyElement;
                XAttribute? isPartnerElement;
                XAttribute? isSoulSurvivorElement;
                XAttribute? killedByMeElement;
                XAttribute? killedByTeammateElement;
                XAttribute? killedMeElement;
                XAttribute? killedTeammateElement;
                XAttribute? mmrElement;
                XAttribute? profileIdElement;
                XAttribute? proximityElement;
                XAttribute? proximityToMeElement;
                XAttribute? proximityToTeammateElement;
                XAttribute? skillBasedElement;
                XAttribute? teamExtractionElement;
                try
                {
                    bloodLineNameElement = GetAttributeByName(doc,
                        $"{baseTempPlayerString}{PlayerOptions.BloodLineName.GetDescription()}");
                    bountyExtractedElement = GetAttributeByName(doc,
                        $"{baseTempPlayerString}{PlayerOptions.BountyExtracted.GetDescription()}");
                    bountyPickedUpElement = GetAttributeByName(doc,
                        $"{baseTempPlayerString}{PlayerOptions.BountyPickedUp.GetDescription()}");
                    downedByMeElement = GetAttributeByName(doc,
                        $"{baseTempPlayerString}{PlayerOptions.DownedByMe.GetDescription()}");
                    downedByTeammateElement = GetAttributeByName(doc,
                        $"{baseTempPlayerString}{PlayerOptions.DownedByTeammate.GetDescription()}");
                    downedMeElement = GetAttributeByName(doc,
                        $"{baseTempPlayerString}{PlayerOptions.DownedMe.GetDescription()}");
                    downedTeammateElement = GetAttributeByName(doc,
                        $"{baseTempPlayerString}{PlayerOptions.DownedTeammate.GetDescription()}");
                    hadWellspringElement = GetAttributeByName(doc,
                        $"{baseTempPlayerString}{PlayerOptions.HadWellspring.GetDescription()}");
                    hadBountyElement = GetAttributeByName(doc,
                        $"{baseTempPlayerString}{PlayerOptions.HadBounty.GetDescription()}");
                    isPartnerElement = GetAttributeByName(doc,
                        $"{baseTempPlayerString}{PlayerOptions.IsPartner.GetDescription()}");
                    isSoulSurvivorElement = GetAttributeByName(doc,
                        $"{baseTempPlayerString}{PlayerOptions.IsSoulSurvivor.GetDescription()}");
                    killedByMeElement = GetAttributeByName(doc,
                        $"{baseTempPlayerString}{PlayerOptions.KilledByMe.GetDescription()}");
                    killedByTeammateElement = GetAttributeByName(doc,
                        $"{baseTempPlayerString}{PlayerOptions.KilledByTeammate.GetDescription()}");
                    killedMeElement = GetAttributeByName(doc,
                        $"{baseTempPlayerString}{PlayerOptions.KilledMe.GetDescription()}");
                    killedTeammateElement = GetAttributeByName(doc,
                        $"{baseTempPlayerString}{PlayerOptions.KilledTeammate.GetDescription()}");
                    mmrElement = GetAttributeByName(doc, $"{baseTempPlayerString}{PlayerOptions.Mmr.GetDescription()}");
                    profileIdElement = GetAttributeByName(doc,
                        $"{baseTempPlayerString}{PlayerOptions.ProfileId.GetDescription()}");
                    proximityElement = GetAttributeByName(doc,
                        $"{baseTempPlayerString}{PlayerOptions.Proximity.GetDescription()}");
                    proximityToMeElement = GetAttributeByName(doc,
                        $"{baseTempPlayerString}{PlayerOptions.ProximityToMe.GetDescription()}");
                    proximityToTeammateElement = GetAttributeByName(doc,
                        $"{baseTempPlayerString}{PlayerOptions.ProximityToTeammate.GetDescription()}");
                    skillBasedElement = GetAttributeByName(doc,
                        $"{baseTempPlayerString}{PlayerOptions.SkillBased.GetDescription()}");
                    teamExtractionElement = GetAttributeByName(doc,
                        $"{baseTempPlayerString}{PlayerOptions.TeamExtraction.GetDescription()}");
                }
                catch (Exception e)
                {
                    OnExceptionRaised(e);
                    return teams;
                }

                players.Add(new HuntPlayer((ushort) (i + 1), mmrElement?.Value, bloodLineNameElement?.Value,
                    bountyExtractedElement?.Value, bountyPickedUpElement?.Value, downedByMeElement?.Value,
                    downedByTeammateElement?.Value, downedMeElement?.Value, downedTeammateElement?.Value,
                    hadWellspringElement?.Value, hadBountyElement?.Value, isPartnerElement?.Value,
                    isSoulSurvivorElement?.Value, killedByMeElement?.Value, killedByTeammateElement?.Value, killedMeElement?.Value,
                    killedTeammateElement?.Value, profileIdElement?.Value, proximityElement?.Value, proximityToMeElement?.Value,
                    proximityToTeammateElement?.Value, skillBasedElement?.Value, teamExtractionElement?.Value));
            }

            teams.Add(new HuntTeam(teamMmrElement?.Value, (ushort) (teamCounter + 1), inviteTeamElement?.Value,
                ownTeamElement?.Value, players));
            teamCounter++;
        }

        return teams;
    }

    private static XAttribute? GetAttributeByName(XContainer doc, string elementName)
    {
        return (from entry in doc.Descendants("Attr")
            where entry.Attribute("name")?.Value == elementName
            select entry.Attribute("value")).FirstOrDefault();
    }

    private void HandlePath(string path, IEnumerable<HuntTeam> players)
    {
        _teams.Clear();
        foreach (var player in players) _teams.Add(player);
        OnPropertyChanged(nameof(Teams));
        LastReadTime = DateTime.Now;
        PrepareFileWatcher(path);

        SetPath(path);
    }

    internal void PrepareFileWatcher(string path)
    {
        var parentDirectory = Directory.GetParent(path);
        var filename = Path.GetFileName(path);
        if (parentDirectory == default || string.IsNullOrEmpty(filename))
            return;
        if (_watcher == default)
        {
            _watcher = new FileSystemWatcher(parentDirectory.FullName, filename)
                {NotifyFilter = NotifyFilters.LastWrite};
            _watcher.Changed += Watcher_Changed;
            _watcher.EnableRaisingEvents = true;
        }
        else
        {
            _watcher.Path = parentDirectory.FullName;
        }
    }

    internal void Read(string path)
    {
        XDocument doc;
        try
        {
            using var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)
                {Position = 0};
            doc = XDocument.Load(fileStream, LoadOptions.PreserveWhitespace);
        }
        catch (Exception e)
        {
            OnExceptionRaised(e);
            return;
        }

        HandlePath(path, GetPlayersFromDoc(doc));
    }

    internal async Task ReadAsync(string path)
    {
        XDocument doc;
        try
        {
            await using var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)
                {Position = 0};
            doc = await XDocument.LoadAsync(fileStream, LoadOptions.PreserveWhitespace, default)
                .ConfigureAwait(false);
        }
        catch (Exception e)
        {
            OnExceptionRaised(e);
            return;
        }

        HandlePath(path, GetPlayersFromDoc(doc));
    }

    internal void SetPath(string? path)
    {
        lock (_syncPathObject)
        {
            FilePath = path;
        }
    }

    private void Watcher_Changed(object sender, FileSystemEventArgs e)
    {
        _lastModificationTime = DateTime.Now;
        _readSinceLastModification = false;
    }

    internal event EventHandler<Exception>? ExceptionRaised;

    protected virtual void OnExceptionRaised(Exception exception)
    {
        ExceptionRaised?.Invoke(this, exception);
    }
}