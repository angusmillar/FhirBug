using Bug.Logic.Query.FhirApi.Update;
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

namespace Bug.Logic.Query.FhirApi.Update
{
  public class UpdateQueryHandler : IQueryHandler<UpdateQuery, FhirApiResult>
  {
    private readonly IResourceStoreRepository IResourceStoreRepository;
    private readonly IStu3SerializationToJsonBytes IStu3SerializationToJsonBytes;
    private readonly IR4SerializationToJsonBytes IR4SerializationToJsonBytes;
    private readonly IGZipper IGZipper;

    public UpdateQueryHandler(
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

    public async Task<FhirApiResult> Handle(UpdateQuery command)
    {
      byte[] ResourceBytes = null;
      switch (command.FhirMajorVersion)
      {
        case FhirMajorVersion.Stu3:
          ResourceBytes = IStu3SerializationToJsonBytes.SerializeToJsonBytes(command.FhirResource.Stu3);
          break;
        case FhirMajorVersion.R4:
          ResourceBytes = IR4SerializationToJsonBytes.SerializeToJsonBytes(command.FhirResource.R4);
          break;
        default:
          throw new FhirVersionFatalException(command.FhirMajorVersion);
      }

      var OutCome = new FhirApiResult()
      {
        HttpStatusCode = System.Net.HttpStatusCode.OK,
        FhirMajorVersion = command.FhirMajorVersion,
        Resource = command.Resource
      };

      return OutCome;
    }
  }
}
