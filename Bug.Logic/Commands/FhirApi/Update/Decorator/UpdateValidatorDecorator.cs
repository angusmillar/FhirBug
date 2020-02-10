using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Bug.Logic.Command;
using Bug.Common.Enums;
using Bug.Logic.Command.FhirApi;
using Microsoft.Extensions.Logging;
using Bug.Logic.Command.FhirApi.Update;
using Bug.Logic.Interfaces.Repository;
using Bug.Logic.Interfaces.CompositionRoot;
using Bug.Logic.DomainModel;

namespace Bug.Logic.Command.FhirApi.Update.Decorator
{
  public class UpdateValidatorDecorator<TCommand, TOutcome> : ICommandHandler<TCommand, TOutcome>
    where TCommand : UpdateCommand
    where TOutcome : FhirApiOutcome
  {
    private readonly ICommandHandler<TCommand, TOutcome> Decorated;
    private readonly ILogger ILogger;
    private readonly IResourceStoreRepository IResourceStoreRepository;
    private readonly IFhirResourceIdSupportFactory IFhirResourceIdSupportFactory;


    public UpdateValidatorDecorator(ICommandHandler<TCommand, TOutcome> decorated, ILogger logger, IResourceStoreRepository IResourceStoreRepository, IFhirResourceIdSupportFactory IFhirResourceIdSupportFactory)
    {
      this.Decorated = decorated;
      this.ILogger = logger;
      this.IResourceStoreRepository = IResourceStoreRepository;
      this.IFhirResourceIdSupportFactory = IFhirResourceIdSupportFactory;
    }

    public async Task<TOutcome> Handle(TCommand command)
    {
      if (command.FhirMajorVersion == FhirMajorVersion.Stu3)
      {
        var FhirResourceIdSupport = IFhirResourceIdSupportFactory.GetStu3();
        command.FhirId = FhirResourceIdSupport.GetFhirId(command.Resource);
        ResourceStore resourceStore = IResourceStoreRepository.GetByFhirId(command.FhirId);

      }

      return await this.Decorated.Handle(command);
    }
  }
}
