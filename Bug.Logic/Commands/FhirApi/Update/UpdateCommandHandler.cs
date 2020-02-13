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
using Bug.Stu3Fhir.Serialization;
using Bug.R4Fhir.Serialization;
using Bug.Common.Compression;
using Bug.Common.Exceptions;

namespace Bug.Logic.Command.FhirApi.Update
{
  public class UpdateCommandHandler : ICommandHandler<UpdateCommand, FhirApiOutcome>
  {
    private readonly IResourceStoreRepository IResourceStoreRepository;
    private readonly IStu3SerializationToJsonBytes IStu3SerializationToJsonBytes;
    private readonly IR4SerializationToJsonBytes IR4SerializationToJsonBytes;
    private readonly IGZipper IGZipper;

    public UpdateCommandHandler(
      IResourceStoreRepository IResourceStoreRepository,
      IStu3SerializationToJsonBytes IStu3SerializationToJsonBytes,
      IR4SerializationToJsonBytes IR4SerializationToJsonBytes,
      IGZipper IGZipper)
    {
      this.IResourceStoreRepository = IResourceStoreRepository;
      this.IStu3SerializationToJsonBytes = IStu3SerializationToJsonBytes;
      this.IR4SerializationToJsonBytes = IR4SerializationToJsonBytes;
      this.IGZipper = IGZipper;
    }

    public async Task<FhirApiOutcome> Handle(UpdateCommand command)
    {
      byte[] ResourceBytes = null;
      switch (command.FhirMajorVersion)
      {
        case FhirMajorVersion.Stu3:
          ResourceBytes = IStu3SerializationToJsonBytes.SerializeToJsonBytes(command.Resource);
          break;
        case FhirMajorVersion.R4:
          ResourceBytes = IR4SerializationToJsonBytes.SerializeToJsonBytes(command.Resource);
          break;
        default:
          throw new FhirVersionFatalException(command.FhirMajorVersion);
      }

      var OutCome = new FhirApiOutcome()
      {
        HttpStatusCode = System.Net.HttpStatusCode.OK,
        FhirMajorVersion = command.FhirMajorVersion,
        Resource = command.Resource
      };

      return OutCome;
    }
  }
}
