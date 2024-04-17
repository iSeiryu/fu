using System.Diagnostics.CodeAnalysis;
using Spectre.Console;
using Spectre.Console.Cli;

namespace FileInformation.Cli.Commands.Count;
internal sealed class CountWords : Command<CountWords.Settings> {
    public sealed class Settings : CountCommandSettings { }

    public override int Execute([NotNull] CommandContext context, [NotNull] Settings settings) {
        AnsiConsole
            .Status()
            .Spinner(Spinner.Known.Clock)
            .SpinnerStyle(Style.Parse("green"))
            .Start("Working...", ctx => {
                AnsiConsole.MarkupLine($"Searching files in [green]{settings.FormattedSearchPath}[/]");
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
        var files = new DirectoryInfo(settings.FormattedSearchPath).EnumerateFiles(searchPattern, searchOptions);

        var groupped = files
            .SelectMany(fileInfo => File.ReadLines(fileInfo.FullName))
            .SelectMany(line => line.Split(' ', StringSplitOptions.RemoveEmptyEntries))
            .GroupBy(word => word)
            .Select(group => (Word: group.Key, Count: group.Count()))
            .OrderByDescending(x => x.Count)
            .ToList();

        var totalCount = groupped.Sum(x => x.Count);

        if (settings.Head > 0) {
            groupped = groupped.Take(settings.Head).ToList();
        }

        foreach (var group in groupped) {
            AnsiConsole.MarkupLine($"[green]{group.Word.EscapeMarkup()}[/]: {group.Count}");
        }

        AnsiConsole.MarkupLine($"Total: [green]{totalCount}[/]");
    }
}
