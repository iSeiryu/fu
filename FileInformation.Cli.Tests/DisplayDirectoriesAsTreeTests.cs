using FileInformation.Cli.Commands;
using FluentAssertions;
using Spectre.Console.Testing;

namespace FileInformation.Cli.Tests;

public class DisplayDirectoriesAsTreeTests {
    [Fact]
    public void DisplayDirectoriesAsTree_without_agrs_runs_successfully() {
        var commandTester = new CommandAppTester();
        commandTester.SetDefaultCommand<DisplayDirectoriesAsTree>();
        var result = commandTester.Run();
        var settings = result.Settings.As<DisplayDirectoriesAsTree.Settings>();

        result.ExitCode.Should().Be(0);
        settings.SearchPath.Should().Be(null);
        settings.Depth.Should().Be(3);
        settings.IncludeHidden.Should().BeFalse();
    }

    [Fact]
    public void DisplayDirectoriesAsTree_with_valid_agrs_parses_the_input_successfully() {
        var commandTester = new CommandAppTester();
        commandTester.SetDefaultCommand<DisplayDirectoriesAsTree>();
        var result = commandTester.Run(["~/", "-d", "4", "--hidden"]);
        var settings = result.Settings.As<DisplayDirectoriesAsTree.Settings>();

        result.ExitCode.Should().Be(0);
        settings.SearchPath.Should().Be("~/");
        settings.Depth.Should().Be(4);
        settings.IncludeHidden.Should().BeTrue();
    }
}