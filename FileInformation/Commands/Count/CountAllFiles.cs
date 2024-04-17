using System.Diagnostics.CodeAnalysis;
using Spectre.Console;
using Spectre.Console.Cli;

namespace FileInformation.Cli.Commands.Count;

internal sealed class CountAllFiles : Command<CountAllFiles.Settings> {
    public sealed class Settings : CountCommandSettings { }

    public override int Execute([NotNull] CommandContext context, [NotNull] Settings settings) {
        AnsiConsole
            .Status()
            .Spinner(Spinner.Known.Clock)
            .SpinnerStyle(Style.Parse("green"))
            .Start("Working...", ctx => {
                AnsiConsole.MarkupLine($"Searching files in [green]{settings.SearchPath}[/]");
                Count(settings);
            });

        return 0;
    }

    static void Count(Settings settings) {
        var searchOptions = new EnumerationOptions {
            AttributesToSkip = FileAttributes.System | FileAttributes.ReparsePoint,
            RecurseSubdirectories = settings.RecurseSubdirectories
        };

        if (!settings.IncludeHidden) {
            searchOptions.AttributesToSkip |= FileAttributes.Hidden;
        }

        var searchPattern = settings.SearchPattern ?? "*";
        var searchPath = PathService.BuildPath(settings.SearchPath);
        var files = new DirectoryInfo(searchPath).EnumerateFiles(searchPattern, searchOptions);

        var groupped = files
            .GroupBy(fileInfo => fileInfo.Extension)
            .Select(group => (Extension: group.Key, Count: group.Count()))
            .OrderByDescending(x => x.Count)
            .ToList();

        var totalCount = groupped.Sum(x => x.Count);
        if (settings.Head > 0) {
            groupped = groupped.Take(settings.Head).ToList();
        }

        foreach (var group in groupped) {
            AnsiConsole.MarkupLine($"[green]{group.Extension}[/]: {group.Count}");
        }

        AnsiConsole.MarkupLine($"Total: [green]{totalCount}[/]");
    }
}
