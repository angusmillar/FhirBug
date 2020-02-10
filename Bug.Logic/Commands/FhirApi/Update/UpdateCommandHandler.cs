using Bug.Logic.Command.FhirApi.Update;
using Bug.Common.Enums;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Bug.Logic.Interfaces.CompositionRoot;
using Bug.Logic.Interfaces.Repository;
using Bug.Logic.DomainModel;

namespace Bug.Logic.Command.FhirApi.Update
{
  public class UpdateCommandHandler : ICommandHandler<UpdateCommand, FhirApiOutcome>
  {    
    private readonly IFhirResourceSupportFactory IResourceUpdateSupportFactory;
    private readonly IResourceStoreRepository IRepository;

    public UpdateCommandHandler(IFhirResourceSupportFactory IResourceUpdateSupportFactory, IResourceStoreRepository IResourceStoreRepository)
    {      
      this.IResourceUpdateSupportFactory = IResourceUpdateSupportFactory;
      this.IRepository = IResourceStoreRepository;
    }

    public async Task<FhirApiOutcome> Handle(UpdateCommand command)
    {

      //IRepository.GetByFhirId(command.)

      switch (command.FhirMajorVersion)
      {
        case FhirMajorVersion.Stu3:
          var Stu3ResourceSupport = IResourceUpdateSupportFactory.GetStu3();
          Stu3ResourceSupport.SetFhirId("The FHIR Id", command.Resource);
          Stu3ResourceSupport.SetLastUpdated(DateTimeOffset.Now, command.Resource);
          Stu3ResourceSupport.SetVersion("1", command.Resource);
          break;
        case FhirMajorVersion.R4:
          var R4ResourceSupport = IResourceUpdateSupportFactory.GetStu3();
          R4ResourceSupport.SetFhirId("The FHIR Id", command.Resource);
          R4ResourceSupport.SetLastUpdated(DateTimeOffset.Now, command.Resource);
          R4ResourceSupport.SetVersion("1", command.Resource);
          break;
        case FhirMajorVersion.Unknown:
          throw new Bug.Common.Exceptions.FhirFatalException(System.Net.HttpStatusCode.InternalServerError, "Unknown FHIR Version.");
        default:
          throw new Bug.Common.Exceptions.FhirFatalException(System.Net.HttpStatusCode.InternalServerError, $"Unknown FHIR Version of : {command.FhirMajorVersion.GetLiteral()}");
      }

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
