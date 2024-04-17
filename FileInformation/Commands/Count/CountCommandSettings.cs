using System.ComponentModel;
using Spectre.Console.Cli;

namespace FileInformation.Cli.Commands.Count;

internal class CountCommandSettings : CommandSettings {
    [Description("Path to search. Defaults to current directory.")]
    [CommandArgument(0, "[searchPath]")]
    public string? SearchPath { get; init; }
    public string FormattedSearchPath => PathService.BuildPath(SearchPath);

    [CommandOption("-p|--pattern")]
    public string? SearchPattern { get; init; }

    [CommandOption("-r|--recurse")]
    [DefaultValue(false)]
    public bool RecurseSubdirectories { get; init; }

    [CommandOption("--head")]
    [DefaultValue(0)]
    public int Head { get; init; }

    [CommandOption("--hidden")]
    [DefaultValue(false)]
    public bool IncludeHidden { get; init; }
}
