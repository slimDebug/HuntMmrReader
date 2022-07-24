using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using HuntMmrReader.DesignHelper;
using HuntMmrReader.Enums;
using HuntMmrReader.Models;
using HuntMmrReader.Properties;
using HuntMmrReader.Views;
using Microsoft.Win32;

namespace HuntMmrReader.ViewModels;

internal class MainWindowViewModel : ViewModelBase
{
    internal const string BaseTitle = "Hunt MMR Reader";

    private readonly HuntReader _reader;

    private PlayerOptions _displayOptions;
    private string _filePath;
    private DateTime _lastRefreshTime;
    private DisplayException _selectedDisplayException;
    private ObservableCollection<HuntTeam> _teams;
    private string _title = BaseTitle;

    public MainWindowViewModel()
    {
        _teams = new ObservableCollection<HuntTeam>();
        Exceptions = new ObservableCollection<DisplayException>();
        GetFilePathCommand = new RelayCommand<object>(SelectFile);
        ReadFileCommand = new RelayCommand<string>(FillHunters, CheckIfFileExists);
        CloseWindowCommand = new RelayCommand<Window>(CloseWindow);
        CloseEventCommand = new RelayCommand<object>(CloseEventHandling);
        ClearErrorsCommand = new RelayCommand<object>(ClearExceptions, _ => Exceptions.Any());
        OpenFolderCommand = new RelayCommand<string>(OpenFolder, CheckIfFileExists);
        ClipboardCopyCommand = new RelayCommand<string>(CopyToClipboard);
        SetUnsetDisplayOptionsCommand = new RelayCommand<PlayerOptions>(HandleOption);
        AboutCommand = new RelayCommand<Window>(OpenAbout);
        _reader = new HuntReader(TimeSpan.FromSeconds(20));
        _reader.PropertyChanged += Reader_PropertyChanged;
        _reader.ExceptionRaised += Reader_ExceptionRaised;
        _filePath = string.Empty;
        _selectedDisplayException = new DisplayException(typeof(DisplayException), "default Exception", default);
        LoadSettings();
        if (!string.IsNullOrEmpty(FilePath))
            _reader.Read(FilePath);
    }

    public PlayerOptions DisplayOptions
    {
        get => _displayOptions;
        private set => SetProperty(ref _displayOptions, value);
    }

    public DisplayException SelectedDisplayException
    {
        get => _selectedDisplayException;
        set => SetProperty(ref _selectedDisplayException, value);
    }

    public string Title
    {
        get => LastRefreshTime == default ? BaseTitle : $"{BaseTitle} | Last refresh time: {LastRefreshTime:G}";
        set => SetProperty(ref _title, value);
    }

    public string FilePath
    {
        get => _filePath;
        set
        {
            SetProperty(ref _filePath, value);
            _reader.SetPath(_filePath);
        }
    }

    public DateTime LastRefreshTime
    {
        get => _lastRefreshTime;
        set
        {
            SetProperty(ref _lastRefreshTime, value);
            OnPropertyChanged(nameof(Title));
        }
    }

    public ObservableCollection<HuntTeam> Teams
    {
        get => _teams;
        set => SetProperty(ref _teams, value);
    }

    public ObservableCollection<DisplayException> Exceptions { get; }

    public RelayCommand<object> GetFilePathCommand { get; }

    public RelayCommand<string> ReadFileCommand { get; }

    public RelayCommand<Window> CloseWindowCommand { get; }

    public RelayCommand<object> CloseEventCommand { get; }

    public RelayCommand<object> ClearErrorsCommand { get; }

    public RelayCommand<string> OpenFolderCommand { get; }

    public RelayCommand<string> ClipboardCopyCommand { get; }

    public RelayCommand<PlayerOptions> SetUnsetDisplayOptionsCommand { get; }

    public RelayCommand<Window> AboutCommand { get; }

    private void LoadSettings()
    {
        try
        {
            var options = (PlayerOptions) Settings.Default.DisplayOptions;
            _displayOptions = (PlayerOptions.All & options) == options ? options : PlayerOptions.None;
            FilePath = Settings.Default.LastPath;
            if (CheckIfFileExists(FilePath))
                _reader.PrepareFileWatcher(FilePath);
        }
        catch
        {
            FilePath = string.Empty;
            _displayOptions = PlayerOptions.None;
        }
    }

    private void HandleOption(PlayerOptions option)
    {
        switch (option)
        {
            case PlayerOptions.None:
            case PlayerOptions.All:
                DisplayOptions = option;
                break;
            case PlayerOptions.BloodLineName:
            case PlayerOptions.BountyExtracted:
            case PlayerOptions.BountyPickedUp:
            case PlayerOptions.HadWellspring:
            case PlayerOptions.HadBounty:
            case PlayerOptions.IsPartner:
            case PlayerOptions.IsSoulSurvivor:
            case PlayerOptions.Mmr:
            case PlayerOptions.ProfileId:
            case PlayerOptions.Proximity:
            case PlayerOptions.ProximityToMe:
            case PlayerOptions.ProximityToTeammate:
            case PlayerOptions.SkillBased:
            case PlayerOptions.TeamExtraction:
                ToggleOption(option);
                break;
            // OverallKilledByMe option handling
            case PlayerOptions.DownedByMe:
            case PlayerOptions.KilledByMe:
                if (DisplayOptions.HasFlag(PlayerOptions.OverallKilledByMe))
                    DisplayOptions &= ~PlayerOptions.OverallKilledByMe;
                ToggleOption(option);
                break;
            case PlayerOptions.OverallKilledByMe:
                foreach (var opt in new[] {PlayerOptions.DownedByMe, PlayerOptions.KilledByMe})
                    if (DisplayOptions.HasFlag(opt))
                        DisplayOptions &= ~opt;
                ToggleOption(option);
                break;
            // OverallKilledByTeammate option handling
            case PlayerOptions.DownedByTeammate:
            case PlayerOptions.KilledByTeammate:
                if (DisplayOptions.HasFlag(PlayerOptions.OverallKilledByTeammate))
                    DisplayOptions &= ~PlayerOptions.OverallKilledByTeammate;
                ToggleOption(option);
                break;
            case PlayerOptions.OverallKilledByTeammate:
                foreach (var opt in new[] {PlayerOptions.DownedByTeammate, PlayerOptions.KilledByTeammate})
                    if (DisplayOptions.HasFlag(opt))
                        DisplayOptions &= ~opt;
                ToggleOption(option);
                break;
            // OverallKilledMe option handling
            case PlayerOptions.DownedMe:
            case PlayerOptions.KilledMe:
                if (DisplayOptions.HasFlag(PlayerOptions.OverallKilledMe))
                    DisplayOptions &= ~PlayerOptions.OverallKilledMe;
                ToggleOption(option);
                break;
            case PlayerOptions.OverallKilledMe:
                foreach (var opt in new[] {PlayerOptions.DownedMe, PlayerOptions.KilledMe})
                    if (DisplayOptions.HasFlag(opt))
                        DisplayOptions &= ~opt;
                ToggleOption(option);
                break;
            // OverallKilledTeammate option handling
            case PlayerOptions.DownedTeammate:
            case PlayerOptions.KilledTeammate:
                if (DisplayOptions.HasFlag(PlayerOptions.OverallKilledTeammate))
                    DisplayOptions &= ~PlayerOptions.OverallKilledTeammate;
                ToggleOption(option);
                break;
            case PlayerOptions.OverallKilledTeammate:
                foreach (var opt in new[] {PlayerOptions.DownedTeammate, PlayerOptions.KilledTeammate})
                    if (DisplayOptions.HasFlag(opt))
                        DisplayOptions &= ~opt;
                ToggleOption(option);
                break;
            default:
                AddException(new ArgumentOutOfRangeException(nameof(option), option,
                    $"{((int) option).ToString(CultureInfo.InvariantCulture)} is not a valid {nameof(option)} value."));
                break;
        }
    }

    private void ToggleOption(PlayerOptions option)
    {
        if (DisplayOptions.HasFlag(option))
            DisplayOptions &= ~option;
        else
            DisplayOptions |= option;
    }

    private void Reader_ExceptionRaised(object? sender, Exception e)
    {
        AddException(e);
    }


    private void Reader_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (string.IsNullOrEmpty(e.PropertyName))
            return;

        switch (e.PropertyName)
        {
            case nameof(_reader.Teams):
                Teams = new ObservableCollection<HuntTeam>(_reader.Teams);
                break;
            case nameof(_reader.LastReadTime):
                LastRefreshTime = _reader.LastReadTime;
                break;
            default:
                return;
        }
    }

    private async void SelectFile(object _)
    {
        var openFileDialog = new OpenFileDialog
        {
            Filter = "XML files (*.xml)|*.xml",
            RestoreDirectory = true
        };
        if (openFileDialog.ShowDialog() != true) return;
        FilePath = openFileDialog.FileName;

        await _reader.ReadAsync(FilePath).ConfigureAwait(false);
    }

    private static void CloseWindow(Window window)
    {
        window.Close();
    }

    private static void OpenAbout(Window window)
    {
        var aboutWindow = new AboutWindow {Owner = window};
        aboutWindow.ShowDialog();
    }

    private void CloseEventHandling(object _)
    {
        Settings.Default.LastPath = _filePath;
        Settings.Default.DisplayOptions = (int) DisplayOptions;
        Settings.Default.Save();
    }

    private async void FillHunters(string path)
    {
        await _reader.ReadAsync(FilePath).ConfigureAwait(false);
    }

    private void AddException(Exception ex)
    {
        Exceptions.Add(new DisplayException(ex.GetBaseException().GetType(), ex.Message, ex.StackTrace));
    }

    private void ClearExceptions(object _)
    {
        Exceptions.Clear();
    }

    private void OpenFolder(string path)
    {
        string? parentDirectoryPath = default;
        try
        {
            parentDirectoryPath = Directory.GetParent(path)?.FullName;
        }
        catch (Exception e)
        {
            AddException(e);
        }

        if (parentDirectoryPath == default)
            return;

        try
        {
            Process.Start("explorer.exe", parentDirectoryPath);
        }
        catch (Exception e)
        {
            AddException(e);
        }
    }

    private static void CopyToClipboard(string text)
    {
        Clipboard.SetText(text);
    }

    private static bool CheckIfFileExists(string path)
    {
        return !string.IsNullOrEmpty(path) && path.IndexOfAny(Path.GetInvalidPathChars()) == -1 && File.Exists(path);
    }
}