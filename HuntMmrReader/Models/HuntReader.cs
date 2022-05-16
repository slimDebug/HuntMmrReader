using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using HuntMmrReader.DesignHelper;

namespace HuntMmrReader.Models;

internal class HuntReader : ObservableObject, IDisposable
{
    internal const ushort MaxPlayers = 12;
    internal const ushort MaxTeams = MaxPlayers;

    private readonly TimeSpan _readTimeout;

    private readonly BlockingCollection<(DateTime modifiedDateTime, bool alreadyTried)> _readXmlQueue =
        new(new ConcurrentQueue<(DateTime modifieDateTime, bool alreadyTried)>());

    private readonly object _syncLastReadTimeObject = new();
    private readonly object _syncPathObject = new();
    private readonly ConcurrentBag<HuntTeam> _teams = new();
    private DateTime _lastReadTime;
    private FileSystemWatcher? _watcher;

    internal HuntReader(TimeSpan readTimeout)
    {
        _lastReadTime = DateTime.Now;
        _readTimeout = readTimeout;
        new Thread(ProcessFileChanges)
        {
            IsBackground = true
        }.Start();
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
        _readXmlQueue.Dispose();
        _watcher?.Dispose();
    }


    private void ProcessFileChanges()
    {
        while (true)
        {
            var (modifiedDateTime, alreadyTried) = _readXmlQueue.Take();
            if (string.IsNullOrEmpty(FilePath))
                return;
            try
            {
                Thread.Sleep(_readTimeout);
                Read(FilePath);
                LastReadTime = modifiedDateTime;
            }
            catch (Exception e)
            {
                if (!alreadyTried)
                    _readXmlQueue.Add((DateTime.Now, true));
                else
                    OnExceptionRaised(e);
            }
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
            XAttribute? skillBasedMatchMakingEnabledElement = default;
            try
            {
                var baseTempTeamString = $"MissionBagTeam_{teamCounter.ToString(CultureInfo.InvariantCulture)}_";
                teamMmrElement = GetAttributeByName(doc,
                    $"{baseTempTeamString}mmr");
                inviteTeamElement = GetAttributeByName(doc, $"{baseTempTeamString}isinvite");
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
                var tempPlayerName = $"{baseTempPlayerString}blood_line_name";
                var tempPlayerMmr = $"{baseTempPlayerString}mmr";
                var tempPlayerHadBounty = $"{baseTempPlayerString}hadbounty";
                var tempPlayerHadWellSpring = $"{baseTempPlayerString}hadWellspring";
                var tempPlayerKilledByMe = $"{baseTempPlayerString}killedbyme";
                var tempPlayerKilledMe = $"{baseTempPlayerString}killedme";
                var tempPlayerSkillBasedMatchMakingEnabled = $"{baseTempPlayerString}skillbased";
                XAttribute? nameElement;
                XAttribute? mmrElement;
                XAttribute? hadBountyElement;
                XAttribute? hadWellSpringElement;
                XAttribute? killedByMeElement;
                XAttribute? killedMeElement;
                try
                {
                    nameElement = GetAttributeByName(doc, tempPlayerName);
                    mmrElement = GetAttributeByName(doc, tempPlayerMmr);
                    hadBountyElement = GetAttributeByName(doc, tempPlayerHadBounty);
                    hadWellSpringElement = GetAttributeByName(doc, tempPlayerHadWellSpring);
                    killedByMeElement = GetAttributeByName(doc, tempPlayerKilledByMe);
                    killedMeElement = GetAttributeByName(doc, tempPlayerKilledMe);
                    skillBasedMatchMakingEnabledElement =
                        GetAttributeByName(doc, tempPlayerSkillBasedMatchMakingEnabled);
                }
                catch (Exception e)
                {
                    OnExceptionRaised(e);
                    return teams;
                }

                if (nameElement != default && mmrElement != default && hadBountyElement != default &&
                    hadWellSpringElement != default && killedByMeElement != default && killedMeElement != default)
                    players.Add(new HuntPlayer(nameElement.Value, mmrElement.Value, hadBountyElement.Value,
                        hadWellSpringElement.Value,
                        killedByMeElement.Value, killedMeElement.Value, (ushort) (i + 1)));
            }

            teams.Add(new HuntTeam(teamMmrElement?.Value, (ushort) (teamCounter + 1), inviteTeamElement?.Value,
                skillBasedMatchMakingEnabledElement?.Value, players));
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

        SetPath(path);
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
        _readXmlQueue.Add((DateTime.Now, false));
    }

    internal event EventHandler<Exception>? ExceptionRaised;

    protected virtual void OnExceptionRaised(Exception exception)
    {
        ExceptionRaised?.Invoke(this, exception);
    }
}