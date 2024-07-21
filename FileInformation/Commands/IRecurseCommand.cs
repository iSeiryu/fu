using System.ComponentModel;
using Spectre.Console.Cli;

namespace FileInformation.Cli.Commands;
internal interface IRecurseCommand {
    [CommandOption("-r|--recurse")]
    [DefaultValue(false)]
    bool RecurseSubdirectories { get; init; }

    [CommandOption("-d|--depth")]
    [DefaultValue(0)]
    int Depth { get; init; }
}
