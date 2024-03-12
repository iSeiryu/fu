using FileInformation.Cli.Commands;
using FluentAssertions;
using Spectre.Console.Testing;

namespace FileInformation.Cli.Tests;
public class CountAllFilesTests {
    [Fact]
    public void CountAllFiles_without_agrs_runs_successfully() {
        var commandTester = new CommandAppTester();
        commandTester.SetDefaultCommand<CountAllFiles>();
        var result = commandTester.Run();
        var settings = result.Settings.As<CountAllFiles.Settings>();

        result.ExitCode.Should().Be(0);
        settings.SearchPath.Should().Be(null);
        settings.SearchPattern.Should().Be(null);
        settings.Head.Should().Be(0);
        settings.RecurseSubdirectories.Should().BeFalse();
        settings.IncludeHidden.Should().BeFalse();
    }
}
