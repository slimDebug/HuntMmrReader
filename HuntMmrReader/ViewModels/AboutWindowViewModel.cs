using System;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using HuntMmrReader.DesignHelper;

namespace HuntMmrReader.ViewModels;

internal class AboutWindowViewModel : ViewModelBase
{
    public AboutWindowViewModel()
    {
        OpenRepositoryCommand = new RelayCommand<string>(OpenRepository);
    }

    public static string Title => $"About {MainWindowViewModel.BaseTitle}";

    public static string CreationString => "slimDebug";
    public static string BuildDate => GetBuildDate(Assembly.GetExecutingAssembly()).ToString("F");
    public static Version ReaderVersion => Assembly.GetExecutingAssembly().GetName().Version ?? new Version();
    public static string RepositoryUrl => "https://github.com/slimDebug/HuntMmrReader";

    public RelayCommand<string> OpenRepositoryCommand { get; }

    private static DateTime GetBuildDate(Assembly assembly)
    {
        const string buildVersionMetadataPrefix = "+build";

        var attribute = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>();
        if (attribute?.InformationalVersion == default) return default;
        var value = attribute.InformationalVersion;
        var index = value.IndexOf(buildVersionMetadataPrefix, StringComparison.Ordinal);
        if (index <= 0) return default;
        value = value[(index + buildVersionMetadataPrefix.Length)..];

        return DateTime.TryParseExact(value, "yyyyMMddHHmmss", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal,
            out var result)
            ? result.ToLocalTime()
            : default;
    }

    private static void OpenRepository(string url)
    {
        var startInfo = new ProcessStartInfo(url)
        {
            UseShellExecute = true
        };
        Process.Start(startInfo);
    }
}