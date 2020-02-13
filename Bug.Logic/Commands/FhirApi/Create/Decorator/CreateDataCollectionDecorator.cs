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
using Bug.Common.Exceptions;
using Microsoft.Extensions.Caching.Distributed;

namespace Bug.Logic.Command.FhirApi.Create.Decorator
{
  public class CreateDataCollectionDecorator<TCommand, TOutcome> : ICommandHandler<TCommand, TOutcome>
    where TCommand : CreateCommand
    where TOutcome : FhirApiOutcome
  {
    private readonly ICommandHandler<TCommand, TOutcome> Decorated;   
    private readonly IFhirResourceIdSupportFactory IFhirResourceIdSupportFactory;
    private readonly IFhirResourceVersionSupportFactory IFhirResourceVersionSupportFactory;
    private readonly IFhirResourceLastUpdatedSupportFactory IFhirResourceLastUpdatedSupportFactory;
    //private readonly IDistributedCache IDistributedCache;

    public CreateDataCollectionDecorator(ICommandHandler<TCommand, TOutcome> decorated, 
      IFhirResourceIdSupportFactory IFhirResourceIdSupportFactory,
      IFhirResourceVersionSupportFactory IFhirResourceVersionSupportFactory,   
      IFhirResourceLastUpdatedSupportFactory IFhirResourceLastUpdatedSupportFactory)
    {
      this.Decorated = decorated;      
      this.IFhirResourceIdSupportFactory = IFhirResourceIdSupportFactory;
      this.IFhirResourceVersionSupportFactory = IFhirResourceVersionSupportFactory;      
      this.IFhirResourceLastUpdatedSupportFactory = IFhirResourceLastUpdatedSupportFactory;
      //this.IDistributedCache = IDistributedCache;
    }

    public async Task<TOutcome> Handle(TCommand command)
    {

      //string LastValue = await IDistributedCache.GetStringAsync("angus");

      //await IDistributedCache.SetStringAsync("angus", DateTime.Now.ToString());

      


      if (command.FhirMajorVersion == FhirMajorVersion.Stu3)
      {        
        command.FhirId = Bug.Common.FhirTools.FhirGuidSupport.NewFhirGuid();
        command.VersionId = "1";
        command.LastUpdated = DateTimeOffset.Now;

        var FhirResourceIdSupport = IFhirResourceIdSupportFactory.GetStu3();
        var FhirResourceVersionSupport = IFhirResourceVersionSupportFactory.GetStu3();
        var FhirResourceLastUpdatedSupport = IFhirResourceLastUpdatedSupportFactory.GetStu3();

        FhirResourceIdSupport.SetFhirId(command.FhirId, command.Resource);
        FhirResourceVersionSupport.SetVersion(command.VersionId, command.Resource);        
        FhirResourceLastUpdatedSupport.SetLastUpdated(command.LastUpdated, command.Resource);               
      } 
      else if (command.FhirMajorVersion == FhirMajorVersion.R4)
      {       
        command.FhirId = Bug.Common.FhirTools.FhirGuidSupport.NewFhirGuid();
        command.VersionId = "1";
        command.LastUpdated = DateTimeOffset.Now;

        var FhirResourceIdSupport = IFhirResourceIdSupportFactory.GetR4();
        var FhirResourceVersionSupport = IFhirResourceVersionSupportFactory.GetR4();
        var FhirResourceLastUpdatedSupport = IFhirResourceLastUpdatedSupportFactory.GetR4();

        FhirResourceIdSupport.SetFhirId(command.FhirId, command.Resource);
        FhirResourceVersionSupport.SetVersion(command.VersionId, command.Resource);
        FhirResourceLastUpdatedSupport.SetLastUpdated(command.LastUpdated, command.Resource);             
      }
      else
      {
        throw new FhirVersionFatalException(command.FhirMajorVersion);
      }

      return await this.Decorated.Handle(command);
    }
  }
}
