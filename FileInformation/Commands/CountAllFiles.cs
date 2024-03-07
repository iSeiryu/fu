using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Spectre.Console;
using Spectre.Console.Cli;

namespace FileInformation.Cli.Commands;

internal sealed class CountAllFiles : Command<CountAllFiles.Settings> {
    public sealed class Settings : CommandSettings {
        [Description("Path to search. Defaults to current directory.")]
        [CommandArgument(0, "[searchPath]")]
        public string? SearchPath { get; init; }

        [CommandOption("-p|--pattern")]
        public string? SearchPattern { get; init; }

        [CommandOption("-r|--recurse")]
        [DefaultValue(false)]
        public bool RecurseSubdirectories { get; init; }

        [CommandOption("--head")]
        [DefaultValue(0)]
        public int Head { get; init; }

        [CommandOption("--hidden")]
        [DefaultValue(true)]
        public bool IncludeHidden { get; init; }
    }

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
            AttributesToSkip = settings.IncludeHidden
                ? FileAttributes.System
                : FileAttributes.Hidden | FileAttributes.System,
            RecurseSubdirectories = settings.RecurseSubdirectories
        };

        var searchPattern = settings.SearchPattern ?? "*";
        var searchPath = PathService.BuildPath(settings.SearchPath);
        var files = new DirectoryInfo(searchPath).EnumerateFiles(searchPattern, searchOptions);

        var groupped = files
            .GroupBy(fileInfo => fileInfo.Extension)
            .Select(group => (Extension: group.Key, Count: group.Count()))
            .OrderByDescending(x => x.Count)
            .AsEnumerable();

        if (settings.Head > 0) {
            groupped = groupped.Take(settings.Head);
        }

        foreach (var group in groupped) {
            AnsiConsole.MarkupLine($"[green]{group.Extension}[/]: {group.Count}");
        }

        AnsiConsole.MarkupLine($"Total: [green]{groupped.Sum(x => x.Count)}[/]");
    }
}
