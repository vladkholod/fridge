using FluentAssertions;
using Fridge.Commands;
using Fridge.Common;
using Fridge.Exceptions;
using Fridge.Models;
using Fridge.Services;
using Fridge.Unit.Fakes;
using Xunit;

namespace Fridge.Unit.Services;

public class CommandRunnerTests
{
    private readonly ICommandRunner _commandRunner = new CommandRunner();

    [Fact]
    public void Run_NoRequiredParameters_RunsSuccessfully()
    {
        // Arrange
        var fakeCommand = Arrange.CreateCommand("fake-command");

        var commandDescriptor = new CommandDescriptor(fakeCommand);

        // Act
        var act = () => _commandRunner.Run(commandDescriptor);

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void Run_RequiredParameterIsMissing_ThrowsException()
    {
        // Arrange
        var requiredArguments = new[]
        {
            Arrange.CreateArgument("--required-one", true),
            Arrange.CreateArgument("--required-two", true),
        };
        var fakeCommand = Arrange.CreateCommand("fake-command", requiredArguments: requiredArguments);

        var parameters = new []
        {
            Arrange.CreateParameter(requiredArguments[0]), 
        };
        var commandDescriptor = new CommandDescriptor(fakeCommand, parameters);

        // Act
        var act = () => _commandRunner.Run(commandDescriptor);

        // Assert
        act.Should().Throw<MissingRequiredArgumentsException>();
    }

    [Fact]
    public void Run_HelpParameterPassed_RunsSuccessfully()
    {
        // Arrange
        var fakeCommand = Arrange.CreateCommand("fake-command");
        var parameters = new[]
        {
            new Parameter(CommonArguments.Help, null)
        };

        var commandDescriptor = new CommandDescriptor(fakeCommand, parameters);

        // Act
        var act = () => _commandRunner.Run(commandDescriptor);

        // Assert
        act.Should().NotThrow();
    }
    
    private static class Arrange
    {
        public static FakeCommand CreateCommand(
            string commandName,
            Argument[]? requiredArguments = null,
            Argument[]? optionalArguments = null)
        {
            return new FakeCommand(new CommandConfig
            {
                Name = commandName,
                RequiredArguments = requiredArguments ?? Array.Empty<Argument>(),
                OptionalArguments = optionalArguments ?? Array.Empty<Argument>(),
                DescriptionFormat = "fake-description"
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

        public static Parameter CreateParameter(Argument argument)
        {
            var value = argument.RequiresValue
                ? Guid.NewGuid().ToString()
                : null;

            return new Parameter(argument, value);
        }
    }
}