using Bug.Logic.Command;
using Bug.Common.Enums;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Bug.Logic.Interfaces.CompositionRoot;
using Bug.Logic.Interfaces.Repository;

namespace Bug.Logic.Command.FhirApi.Create
{
  public class CreateCommandHandler : ICommandHandler<CreateCommand, FhirApiOutcome>
  {    
    private readonly IFhirResourceSupportFactory IResourceMetaSupportFactory;
    private readonly IResourceStoreRepository IResourceStoreRepository;

    public CreateCommandHandler(IFhirResourceSupportFactory iResourceMetaSupportFactory, IResourceStoreRepository IResourceStoreRepository)
    {      
      this.IResourceMetaSupportFactory = iResourceMetaSupportFactory;
      this.IResourceStoreRepository = IResourceStoreRepository;
    }

    public async Task<FhirApiOutcome> Handle(CreateCommand command)
    {    
      switch (command.FhirMajorVersion)
      {
        case FhirMajorVersion.Stu3:
          var Stu3MetaSupport = IResourceMetaSupportFactory.GetStu3();
          Stu3MetaSupport.SetLastUpdated(DateTimeOffset.Now, command.Resource);
          Stu3MetaSupport.SetVersion("1", command.Resource);
          var x = Bug.Stu3Fhir.SerializationSupport.SerializeToJson()
          break;
        case FhirMajorVersion.R4:
          var R4MetaSupport = IResourceMetaSupportFactory.GetStu3();
          R4MetaSupport.SetLastUpdated(DateTimeOffset.Now, command.Resource);
          R4MetaSupport.SetVersion("1", command.Resource);
          break;
        case FhirMajorVersion.Unknown:
          throw new Bug.Common.Exceptions.FhirFatalException(System.Net.HttpStatusCode.InternalServerError, "Unknown FHIR Version.");
        default:
          throw new Bug.Common.Exceptions.FhirFatalException(System.Net.HttpStatusCode.InternalServerError, $"Unknown FHIR Version of : {command.FhirMajorVersion.GetLiteral()}");
      }

      var ResourceStore = new DomainModel.ResourceStore()
      {
        FhirId = Common.FhirTools.FhirGuidSupport.NewFhirGuid(),
        IsCurrent = true,
        IsDeleted = false,
        VersionId = "1",
        Blob = Common.FhirTools.
      };

      IResourceStoreRepository.Add(ResourceStore);
      await IResourceStoreRepository.SaveChangesAsync();

      var OutCome = new FhirApiOutcome()
      {
        httpStatusCode = System.Net.HttpStatusCode.OK,
        fhirMajorVersion = command.FhirMajorVersion,
        resource = command.Resource
      };

      return OutCome;
    }
  }
}
