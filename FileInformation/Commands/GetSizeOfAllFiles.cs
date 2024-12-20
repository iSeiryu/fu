﻿using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Spectre.Console;
using Spectre.Console.Cli;

namespace FileInformation.Cli.Commands;

internal sealed class GetSizeOfAllFiles : Command<GetSizeOfAllFiles.Settings> {
    public sealed class Settings : CommandSettings {
        [Description("Path to search. Defaults to current directory.")]
        [CommandArgument(0, "[searchPath]")]
        public string? SearchPath { get; init; }

        [CommandOption("-p|--pattern")] public string? SearchPattern { get; init; }

        [CommandOption("--head")]
        [DefaultValue(0)]
        public int Head { get; init; }

        [CommandOption("--tail")]
        [DefaultValue(0)]
        public int Tail { get; init; }

        [CommandOption("-r|--recurse")]
        [DefaultValue(false)]
        public bool RecurseSubdirectories { get; init; }

        [CommandOption("--hidden")]
        [DefaultValue(false)]
        public bool IncludeHidden { get; init; }
    }

    public override int Execute([NotNull] CommandContext context, [NotNull] Settings settings) {
        AnsiConsole
            .Status()
            .Spinner(FuSettings.Spinner)
            .SpinnerStyle(Style.Parse("green"))
            .Start("Working...", ctx => {
                AnsiConsole.MarkupLine($"Searching files in [green]{settings.SearchPath}[/]");
                var (searchPattern, searchPath) = SanitizeInput(settings);
                var files = FileSearcher.Search(searchPath, searchPattern, settings.IncludeHidden,
                    settings.RecurseSubdirectories);
                var (result, totalFileSize) = Aggregate(settings, files);

                PrintResults(settings, result, totalFileSize, searchPath, searchPattern);
            });

        return 0;
    }

    static (List<KeyValuePair<string, long>>, long) Aggregate(Settings settings, IEnumerable<FileInfo> files) {
        var grouped = files.Aggregate(
                new Dictionary<string, long>(),
                (acc, fileInfo) => {
                    var path = fileInfo.DirectoryName!;

                    if (fileInfo.Attributes == (FileAttributes)(-1)) {
                        return acc;
                    }

                    if (acc.ContainsKey(path)) {
                        acc[path] += fileInfo.Length;
                    }
                    else {
                        acc[path] = fileInfo.Length;
                    }

                    return acc;
                })
            .OrderByDescending(x => x.Value)
            .ToList();

        var result = grouped;
        var totalFileSize = result.Sum(x => x.Value);

        if (settings.Head > 0 || settings.Tail > 0) {
            result = [];

            if (settings.Head > 0)
                result.AddRange(grouped.Take(settings.Head));
            if (settings.Tail > 0)
                result.AddRange(grouped.TakeLast(settings.Tail));
        }

        return (result, totalFileSize);
    }

    static void PrintResults(Settings settings, IEnumerable<KeyValuePair<string, long>> result, long totalFileSize,
        string searchPath, string searchPattern) {
        var includingHidden = settings.IncludeHidden ? ", including hidden" : "";
        var includingSubdirectories = settings.RecurseSubdirectories ? ", including subdirectories" : "";

        foreach (var (key, value) in result) {
            AnsiConsole.WriteLine($"{value:N0}\t{key}");
        }

        AnsiConsole.MarkupLine(
            $"Total file size for [green]{searchPattern}[/] files in [green]{searchPath}[/]{includingHidden}{includingSubdirectories}");
        AnsiConsole.WriteLine($"{ConvertToReadableSize(totalFileSize)}");
    }

    static (string searchPattern, string searchPath) SanitizeInput(Settings settings) {
        var searchPattern = settings.SearchPattern ?? "*";
        var searchPath = PathHelper.BuildPath(settings.SearchPath);

        return (searchPattern, searchPath);
    }

    static string ConvertToReadableSize(long bytes) {
        const long kiloByte = 1000;
        const long megaByte = kiloByte * 1000;
        const long gigaByte = megaByte * 1000;

        return bytes switch {
            >= gigaByte => $"{(double)bytes / gigaByte:F2} GB",
            >= megaByte => $"{(double)bytes / megaByte:F2} MB",
            >= kiloByte => $"{(double)bytes / kiloByte:F2} KB",
            _ => $"{bytes} bytes"
        };
    }
}