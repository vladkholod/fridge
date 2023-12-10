using Fridge.Commands;
using Fridge.Models;

namespace Fridge.Unit.Fakes;

public class FakeCommand : BaseCommand
{
    public FakeCommand(CommandConfig config) : base(config)
    {
    }

    public override void Execute(Parameter[] parameters)
    {
    }
}