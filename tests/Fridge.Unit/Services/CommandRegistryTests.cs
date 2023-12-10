using FluentAssertions;
using Fridge.Commands;
using Fridge.Exceptions;
using Fridge.Services;
using Fridge.Unit.Fakes;
using Xunit;

namespace Fridge.Unit.Services;

public class CommandRegistryTests
{
    private readonly ICommandRegistry _commandRegistry = new CommandRegistry();

    [Fact]
    public void Register_UnregisteredCommand_ReturnsTrue()
    {
        // Arrange
        var fakeCommand = Arrange.CreateCommand("fake");

        // Act
        var result = _commandRegistry.Register(fakeCommand);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void Register_AlreadyRegisteredCommand_ReturnsFalse()
    {
        // Arrange
        var fakeCommand = RegisterFakeCommand("fake");
        
        // Act
        var result = _commandRegistry.Register(fakeCommand);

        // Assert
        result.Should().BeFalse();
    }
    
    [Fact]
    public void Get_UnregisteredCommand_ThrowsException()
    {
        // Arrange
        const string commandName = "fake";
        
        // Act
        var act = () => _commandRegistry.Get(commandName);

        // Assert
        act.Should().Throw<UnsupportedCommandException>();
    }
    
    [Fact]
    public void Get_RegisteredCommand_ReturnsCommand()
    {
        // Arrange
        var fakeCommand = RegisterFakeCommand("fake");
        
        // Act
        var result = _commandRegistry.Get(fakeCommand.Config.Name);

        // Assert
        result.Should().Be(fakeCommand);
    }

    [Fact]
    public void All_RegisteredCommands_ReturnsCommands()
    {
        // Arrange
        var registeredCommands = Enumerable.Range(0, 100)
            .Select(index => RegisterFakeCommand($"fake-{index}"))
            .ToArray();
        
        // Act
        var result = _commandRegistry.All;

        // Assert
        result.Should().BeEquivalentTo(registeredCommands);
    }
    
    [Fact]
    public void All_RegisteredCommands_ReturnsEmptyArrayOfCommands()
    {
        // Act
        var result = _commandRegistry.All;

        // Assert
        result.Should().BeEquivalentTo(Array.Empty<ICommand>());
    }
    
    private ICommand RegisterFakeCommand(string commandName)
    {
        var fakeCommand = Arrange.CreateCommand(commandName);
        _commandRegistry.Register(fakeCommand);

        return fakeCommand;
    }

    private static class Arrange
    {
        public static FakeCommand CreateCommand(string commandName)
        {
            return new FakeCommand(new CommandConfig
            {
                Name = commandName
            });
        }
    }
}