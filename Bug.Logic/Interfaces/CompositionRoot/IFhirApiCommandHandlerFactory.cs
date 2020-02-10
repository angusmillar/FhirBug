using Bug.Logic.Command;
using Bug.Logic.Command.FhirApi;
using Bug.Logic.Command.FhirApi.Update;

namespace Bug.Logic.Interfaces.CompositionRoot
{
  public interface IFhirApiCommandHandlerFactory
  {
    ICommandHandler<Command.FhirApi.Update.UpdateCommand, FhirApiOutcome> GetUpdateCommand();
    ICommandHandler<Command.FhirApi.Create.CreateCommand, FhirApiOutcome> GetCreateCommand();
  }
}