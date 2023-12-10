﻿using FluentAssertions;
using Fridge.Commands.Freeze;
using Fridge.Exceptions;
using Fridge.Models;
using Xunit;

namespace Fridge.Unit.Commands;

public class FreezeCommandTests
{
    private readonly FreezeCommand _command = new();

    [Fact]
    public void Execute_RequiredParameterIsMissing_ThrowsException()
    {
        // Arrange
        var parameters = Array.Empty<Parameter>();

        // Act
        var act = () => _command.Execute(parameters);

        // Arrange
        var exceptionAssertions = act.Should().Throw<MissingRequiredArgumentsException>();
        exceptionAssertions.Which.MissingRequiredArguments.Should().NotBeEmpty();
    }
}