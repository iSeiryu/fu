﻿using System.Diagnostics.CodeAnalysis;
using Spectre.Console;
using Spectre.Console.Cli;

namespace FileInformation.Cli.Commands.Count;

internal sealed class CountAllFiles : Command<CountAllFiles.Settings> {
    public sealed class Settings : CountCommandSettings { }

    public override int Execute([NotNull] CommandContext context, [NotNull] Settings settings) {
        AnsiConsole
            .Status()
            .Spinner(FuSettings.Spinner)
            .SpinnerStyle(Style.Parse("green"))
            .Start("Working...", ctx => {
                AnsiConsole.MarkupLine($"Searching files in [green]{settings.SearchPath}[/]");
                Count(settings);
            });

        return 0;
    }

    static void Count(Settings settings) {
        var searchPattern = settings.SearchPattern ?? "*";
        var searchPath = PathHelper.BuildPath(settings.SearchPath);
        var files = FileSearcher.Search(searchPath, searchPattern, settings.IncludeHidden,
            settings.RecurseSubdirectories);

        var grouped = files
            .GroupBy(fileInfo => fileInfo.Extension)
            .Select(group => (Extension: group.Key, Count: group.Count()))
            .OrderByDescending(x => x.Count)
            .ToList();

        var totalCount = grouped.Sum(x => x.Count);
        if (settings.Head > 0) {
            grouped = grouped.Take(settings.Head).ToList();
        }

        foreach (var group in grouped) {
            AnsiConsole.MarkupLine($"[green]{group.Extension}[/]: {group.Count}");
        }

        AnsiConsole.MarkupLine($"Total: [green]{totalCount}[/]");
    }
}
