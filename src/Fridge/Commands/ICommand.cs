using System.Text;
using Fridge.Models;

namespace Fridge.Commands;

public interface ICommand
{
    CommandConfig Config { get; }
    
    void Execute(Parameter[] parameters);

    StringBuilder GetHelpBuilder();

    void ValidateRequiredArguments(Argument[] arguments);
}