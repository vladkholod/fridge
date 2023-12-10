using FluentAssertions;
using Fridge.Commands;
using Fridge.Commands.Help;
using Fridge.Exceptions;
using Fridge.Models;
using Fridge.Services;
using Fridge.Unit.Fakes;
using Moq;
using Xunit;

namespace Fridge.Unit.Services;

public class ArgsParserTests
{
    private readonly Mock<ICommandRegistry> _commandRegistryMock = new();

    private readonly IArgsParser _argsParser;

    public ArgsParserTests()
    {
        _argsParser = new ArgsParser(_commandRegistryMock.Object);
    }

    [Fact]
    public void Parse_EmptyArgs_ReturnsHelpCommandDescriptor()
    {
        // Arrange
        var args = Array.Empty<string>();

        // Act
        _argsParser.Parse(args);

        // Assert
        _commandRegistryMock.Verify(
            registry => registry.Get(HelpCommand.CommandConfig.Name), Times.Once);
    }

    [Fact]
    public void Parse_ArgsWithUnsupportedCommand_ThrowsException()
    {
        // Arrange
        const string unsupportedCommandName = "unsupported-command";

        _commandRegistryMock
            .Setup(registry => registry.Get(unsupportedCommandName))
            .Throws(new Exception("Missing command with specified name."));

        var args = Arrange.CreateArgs(unsupportedCommandName);

        // Act
        var act = () => _argsParser.Parse(args);

        // Assert
        act.Should().Throw<Exception>();
    }

    [Fact]
    public void Parse_ArgsWithoutParameters_ReturnsCommandDescriptor()
    {
        // Arrange
        var fakeCommand = Arrange.CreateCommand("fake");

        _commandRegistryMock
            .Setup(registry => registry.Get(fakeCommand.Config.Name))
            .Returns(fakeCommand);

        var args = Arrange.CreateArgs(fakeCommand.Config.Name);

        // Act
        var result = _argsParser.Parse(args);

        // Assert
        result.Command.Should().Be(fakeCommand);
        result.Parameters.Should().BeEmpty();
    }

    [Fact]
    public void Parse_ArgsWithUnsupportedParameter_ThrowsException()
    {
        // Arrange
        var fakeCommand = Arrange.CreateCommand("fake");

        _commandRegistryMock
            .Setup(registry => registry.Get(fakeCommand.Config.Name))
            .Returns(fakeCommand);

        var args = Arrange.CreateArgs(fakeCommand.Config.Name, "--unsupportedArgument");

        // Act
        var act = () => _argsParser.Parse(args);

        // Assert
        act.Should().Throw<UnsupportedArgumentException>();
    }

    [Fact]
    public void Parse_ArgsWithParameterButValueIsMissing_ThrowsException()
    {
        // Arrange
        var optionalArguments = new[]
        {
            Arrange.CreateArgument("--optional", true)
        };
        var fakeCommand = Arrange.CreateCommand("fake", optionalArguments: optionalArguments);

        _commandRegistryMock
            .Setup(registry => registry.Get(fakeCommand.Config.Name))
            .Returns(fakeCommand);

        var args = Arrange.CreateArgs(fakeCommand.Config.Name, optionalArguments[0].Full);

        // Act
        var act = () => _argsParser.Parse(args);

        // Assert
        act.Should().Throw<MissingValueForArgumentException>();
    }

    [Fact]
    public void Parse_ArgsWithParametersAndValueIsMissingButContainsNextArgument_ThrowsException()
    {
        // Arrange
        var optionalArguments = new[]
        {
            Arrange.CreateArgument("--optional", true),
            Arrange.CreateArgument("--other-optional", false),
        };
        var fakeCommand = Arrange.CreateCommand("fake", optionalArguments: optionalArguments);

        _commandRegistryMock
            .Setup(registry => registry.Get(fakeCommand.Config.Name))
            .Returns(fakeCommand);

        var args = Arrange.CreateArgs(fakeCommand.Config.Name, optionalArguments[0].Full, optionalArguments[1].Full);

        // Act
        var act = () => _argsParser.Parse(args);

        // Assert
        act.Should().Throw<MissingValueForArgumentException>();
    }

    [Fact]
    public void Parse_ArgsWithOptionalParameter_ThrowsException()
    {
        // Arrange
        const string optionalValue = "test-value";

        var optionalArguments = new[]
        {
            Arrange.CreateArgument("--optional", true)
        };
        var fakeCommand = Arrange.CreateCommand("fake", optionalArguments: optionalArguments);

        _commandRegistryMock
            .Setup(registry => registry.Get(fakeCommand.Config.Name))
            .Returns(fakeCommand);

        var args = Arrange.CreateArgs(fakeCommand.Config.Name, optionalArguments[0].Full, optionalValue);

        // Act
        var result = _argsParser.Parse(args);

        // Assert
        result.Command.Should().Be(fakeCommand);
        result.Parameters.Any(parameter =>
                optionalArguments[0].Equals(parameter.Argument)
                && optionalValue.Equals(parameter.Value))
            .Should().BeTrue();
    }

    private static class Arrange
    {
        public static string[] CreateArgs(string command, params string[] arguments)
        {
            return new[] { command }
                .Concat(arguments)
                .ToArray();
        }

        public static FakeCommand CreateCommand(
            string commandName,
            Argument? requiredArgument = null,
            Argument[]? optionalArguments = null)
        {
            return new FakeCommand(new CommandConfig
            {
                Name = commandName,
                RequiredArguments = ArrayOrEmpty(requiredArgument),
                OptionalArguments = optionalArguments ?? Array.Empty<Argument>(),
            });

            static Argument[] ArrayOrEmpty(Argument? argument)
            {
                return argument is not null
                    ? new[] { argument }
                    : Array.Empty<Argument>();
            }
        }

        public static Argument CreateArgument(string name, bool withValue)
        {
            return new Argument
            {
                Full = name,
                RequiresValue = withValue
            };
        }
    }
}