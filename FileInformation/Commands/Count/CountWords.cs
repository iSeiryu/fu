using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using Spectre.Console;
using Spectre.Console.Cli;

namespace FileInformation.Cli.Commands.Count;

internal sealed class CountWords : Command<CountWords.Settings> {
    public sealed class Settings : CountCommandSettings {
        [Description("A list of words to find. Can be comma or space separated. Each word/phrase can be surrounded by quotes.")]
        [CommandArgument(10, "[filter]")]
        public string[] Filter { get; init; } = [];

        [Description("Group by file")]
        [CommandOption("--group")]
        [DefaultValue(false)]
        public bool Group { get; init; }
    }

    readonly char[] _separators = [' ', '\t', ':', ';', '.', ',', '/', '\\'];
    const string wordPattern = @"[^\w]";

    public override int Execute([NotNull] CommandContext context, [NotNull] Settings settings) {
        AnsiConsole
            .Status()
            .Spinner(FuSettings.Spinner)
            .SpinnerStyle(Style.Parse("green"))
            .Start("Working...", ctx => {
                AnsiConsole.MarkupLine($"Searching files in [green]{settings.FormattedSearchPath}[/]");
                Count(settings);
            });

        return 0;
    }

    void Count(Settings settings) {
        var searchOptions = new EnumerationOptions {
            AttributesToSkip = FileAttributes.System | FileAttributes.ReparsePoint,
            RecurseSubdirectories = settings.RecurseSubdirectories
        };

        if (!settings.IncludeHidden) {
            searchOptions.AttributesToSkip |= FileAttributes.Hidden;
        }

        var files = new DirectoryInfo(settings.FormattedSearchPath).EnumerateFiles(settings.FormattedSearchPattern, searchOptions);
        var filter = settings.Filter.Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();

        if (settings.Group) {
            SearchAndGroupByFile(files, filter, settings);
        }
        else {
            Search(files, filter, settings);
        }
    }

    void Search(IEnumerable<FileInfo> files, string[] filter, Settings settings) {
        var groupped = files
            .AsParallel()
            .SelectMany(fileInfo => File.ReadLines(fileInfo.FullName))
            .SelectMany(line => line.Split(_separators, StringSplitOptions.RemoveEmptyEntries))
            .Select(word => Regex.Replace(word, wordPattern, "").ToLowerInvariant())
            .Where(word => filter.Length == 0 || filter.Any(f => word == f))
            .GroupBy(word => word)
            .Select(group => (Word: group.Key, Count: group.Count()))
            .OrderByDescending(x => x.Count)
            .ToList();

        var totalCount = groupped.Sum(x => x.Count);

        if (settings.Head > 0) {
            groupped = groupped.Take(settings.Head).ToList();
        }

        foreach (var word in groupped) {
            AnsiConsole.MarkupLine($"[green]{word.Word}[/]: {word.Count}");
        }

        AnsiConsole.MarkupLine($"Total: [green]{totalCount}[/]");
    }

    void SearchAndGroupByFile(IEnumerable<FileInfo> files, string[] filter, Settings settings) {
        var groupped = files
            .AsParallel()
            .Select(fileInfo => (
                fileName: fileInfo.FullName,
                words: File.ReadLines(fileInfo.FullName)
                    .SelectMany(line => line.Split(_separators, StringSplitOptions.RemoveEmptyEntries))
                    .Select(word => Regex.Replace(word, wordPattern, "").ToLowerInvariant())
                    .Where(word => filter.Length == 0 || filter.Any(f => word == f))
                    .GroupBy(word => word)
                    .Select(group => (Word: group.Key, Count: group.Count()))
                    .OrderByDescending(x => x.Count)
                    .ToList()
            ))
            .Where(x => x.words.Count > 0)
            .Select(x => (x.fileName, x.words, count: x.words.Sum(w => w.Count)))
            .OrderByDescending(x => x.count)
            .ToList();

        var totalCount = groupped.Sum(x => x.words.Sum(y => y.Count));

        if (settings.Head > 0) {
            groupped = groupped.Take(settings.Head).ToList();
        }

        foreach (var (fileName, words, count) in groupped) {
            AnsiConsole.MarkupLine($"[green]{count}[/]: {fileName.EscapeMarkup()}");

            foreach (var word in words) {
                AnsiConsole.MarkupLine($"[green]{word.Word}[/]: {word.Count}");
            }
        }

        AnsiConsole.MarkupLine($"Total: [green]{totalCount}[/]");
    }
}
