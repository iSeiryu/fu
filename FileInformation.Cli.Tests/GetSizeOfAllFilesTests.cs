using FileInformation.Cli.Commands;
using FluentAssertions;
using Spectre.Console.Testing;

namespace FileInformation.Cli.Tests;
public class GetSizeOfAllFilesTests {
    [Fact]
    public void GetSizeOfAllFiles_without_agrs_runs_successfully() {
        var commandTester = new CommandAppTester();
        commandTester.SetDefaultCommand<GetSizeOfAllFiles>();
        var result = commandTester.Run();
        var settings = result.Settings.As<GetSizeOfAllFiles.Settings>();

        result.ExitCode.Should().Be(0);
        settings.SearchPath.Should().Be(null);
        settings.SearchPattern.Should().Be(null);
        settings.Head.Should().Be(0);
        settings.Tail.Should().Be(0);
        settings.RecurseSubdirectories.Should().BeFalse();
        settings.IncludeHidden.Should().BeFalse();
    }
}
