﻿using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using HuntMmrReader.DesignHelper;
using HuntMmrReader.Models;
using HuntMmrReader.Properties;
using HuntMmrReader.Views;
using Microsoft.Win32;

namespace HuntMmrReader.ViewModels;

internal class MainWindowViewModel : ViewModelBase
{
    internal const string BaseTitle = "Hunt MMR Reader";

    private readonly HuntReader _reader;
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
        AboutCommand = new RelayCommand<Window>(OpenAbout);
        _reader = new HuntReader(TimeSpan.FromSeconds(20));
        _reader.PropertyChanged += Reader_PropertyChanged;
        _reader.ExceptionRaised += Reader_ExceptionRaised;
        _filePath = "";
        _selectedDisplayException = new DisplayException(typeof(DisplayException), "default Exception", default);
        try
        {
            FilePath = Settings.Default.LastPath;
            if (CheckIfFileExists(FilePath))
                _reader.PrepareFileWatcher(FilePath);
        }
        catch (SettingsPropertyNotFoundException)
        {
            FilePath = "";
        }

        if (!string.IsNullOrEmpty(FilePath))
            _reader.Read(FilePath);
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

    public RelayCommand<Window> AboutCommand { get; }

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