using System.ComponentModel;
using Spectre.Console.Cli;

namespace FileInformation.Cli.Commands.Count;

internal class CountCommandSettings : CommandSettings, IRecurseCommand {
    [Description("Path to search. Defaults to current directory.")]
    [CommandArgument(0, "[searchPath]")]
    public string? SearchPath { get; init; }
    public string FormattedSearchPath => PathHelper.BuildPath(SearchPath);

    [CommandOption("-p|--pattern")]
    public string? SearchPattern { get; init; }
    public string FormattedSearchPattern => SearchPattern ?? "*";

    [CommandOption("-r|--recurse")]
    [DefaultValue(false)]
    public bool RecurseSubdirectories { get; init; }

    [CommandOption("-d|--depth")]
    [DefaultValue(0)]
    public int Depth { get; init; }

    [CommandOption("--head")]
    [DefaultValue(0)]
    public int Head { get; init; }

    [CommandOption("--hidden")]
    [DefaultValue(false)]
    public bool IncludeHidden { get; init; }
}
