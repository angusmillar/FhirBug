using Bug.Logic.Command;
using Bug.Logic.Command.FhirApi.Update;
using Bug.Logic.Command.FhirApi;
using SimpleInjector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bug.Logic.Interfaces.CompositionRoot;

namespace Bug.Api.CompositionRoot
{
  public class FhirApiCommandHandlerFactory : IFhirApiCommandHandlerFactory
  {
    private readonly Container _container;
    public FhirApiCommandHandlerFactory(Container container)
    {
      this._container = container;
    }
    public ICommandHandler<Logic.Command.FhirApi.Update.UpdateCommand, FhirApiOutcome> GetUpdateCommand()
    {
      return (ICommandHandler<Logic.Command.FhirApi.Update.UpdateCommand, FhirApiOutcome>)_container.GetInstance(typeof(ICommandHandler<Logic.Command.FhirApi.Update.UpdateCommand, FhirApiOutcome>));            
    }
    public ICommandHandler<Logic.Command.FhirApi.Create.CreateCommand, FhirApiOutcome> GetCreateCommand()
    {
      return (ICommandHandler<Logic.Command.FhirApi.Create.CreateCommand, FhirApiOutcome>)_container.GetInstance(typeof(ICommandHandler<Logic.Command.FhirApi.Create.CreateCommand, FhirApiOutcome>));
    }
  }
}
