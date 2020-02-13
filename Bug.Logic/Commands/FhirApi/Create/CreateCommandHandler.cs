using Bug.Logic.Command;
using Bug.Common.Enums;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Bug.Logic.Interfaces.CompositionRoot;
using Bug.Logic.Interfaces.Repository;
using Bug.Stu3Fhir.Serialization;
using Bug.R4Fhir.Serialization;
using Bug.Common.Compression;
using Bug.Common.Exceptions;

namespace Bug.Logic.Command.FhirApi.Create
{
  public class CreateCommandHandler : ICommandHandler<CreateCommand, FhirApiOutcome>
  {        
    private readonly IResourceStoreRepository IResourceStoreRepository;
    private readonly IStu3SerializationToJsonBytes IStu3SerializationToJsonBytes;
    private readonly IR4SerializationToJsonBytes IR4SerializationToJsonBytes;
    private readonly IGZipper IGZipper;

    public CreateCommandHandler( 
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

    public async Task<FhirApiOutcome> Handle(CreateCommand command)
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

      var ResourceStore = new DomainModel.ResourceStore()
      {
        FhirId = command.FhirId,
        IsCurrent = true,
        IsDeleted = false,
        VersionId = command.VersionId,
        Blob = IGZipper.Compress(ResourceBytes)
      };

      IResourceStoreRepository.Add(ResourceStore);
      await IResourceStoreRepository.SaveChangesAsync();

      var OutCome = new FhirApiOutcome()
      {
        ResourceId = ResourceStore.FhirId,
        HttpStatusCode = System.Net.HttpStatusCode.Created,
        FhirMajorVersion = command.FhirMajorVersion,
        Resource = command.Resource,
        ResourceVersionId = ResourceStore.VersionId
      };

      return OutCome;
    }
  }
}
