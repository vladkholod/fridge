using System.Text;
using Fridge.Commands;
using Fridge.Commands.Help;
using Fridge.Models;
using Fridge.Services;
using Moq;
using Xunit;

namespace Fridge.Unit.Commands;

public class HelpCommandTests
{
    private readonly Mock<ICommandRegistry> _commandRegistryMock = new();

    private readonly HelpCommand _helpCommand;

    public HelpCommandTests()
    {
        _helpCommand = new HelpCommand(_commandRegistryMock.Object);
    }

    [Fact]
    public void Execute_CallsGetHelpBuilderForEachCommand()
    {
        // Arrange
        var commandMocks = Enumerable.Range(0, 10)
            .Select(_ => Arrange.CreateCommandMock())
            .ToArray();

        _commandRegistryMock.SetupGet(registry => registry.All)
            .Returns(commandMocks.Select(commandMock => commandMock.Object));

        var parameters = Array.Empty<Parameter>();

        // Act
        _helpCommand.Execute(parameters);

        // Assert
        _commandRegistryMock.VerifyGet(registry => registry.All, Times.Once);

        foreach (var commandMock in commandMocks)
        {
            commandMock.Verify(command => command.GetHelpBuilder(), Times.Once);
        }
    }

    private static class Arrange
    {
        public static Mock<ICommand> CreateCommandMock()
        {
            var commandMock = new Mock<ICommand>();

            commandMock
                .Setup(command => command.GetHelpBuilder())
                .Returns(new StringBuilder());

            return commandMock;
        }
    }
}