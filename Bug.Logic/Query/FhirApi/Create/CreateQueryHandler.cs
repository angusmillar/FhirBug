using Bug.Logic.Query;
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
using Bug.Logic.DomainModel;

namespace Bug.Logic.Query.FhirApi.Create
{
  public class CreateQueryHandler<TResource> : IQueryHandler<CreateQuery, FhirApiResult>
  {        
    private readonly IResourceStoreRepository IResourceStoreRepository;
    private readonly IStu3SerializationToJsonBytes IStu3SerializationToJsonBytes;
    private readonly IR4SerializationToJsonBytes IR4SerializationToJsonBytes;    
    private readonly IGZipper IGZipper;

    public CreateQueryHandler( 
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

    public async Task<FhirApiResult> Handle(CreateQuery query)
    {

      byte[] ResourceBytes = null;
      switch (query.FhirMajorVersion)
      {
        case FhirMajorVersion.Stu3:                    
          ResourceBytes = IStu3SerializationToJsonBytes.SerializeToJsonBytes(query.FhirResource.Stu3);
          break;
        case FhirMajorVersion.R4:          
          ResourceBytes = IR4SerializationToJsonBytes.SerializeToJsonBytes(query.FhirResource.R4);
          break;
        default:
          throw new FhirVersionFatalException(query.FhirMajorVersion);
      }

      //var FhirUri = IFhirUriFactory.Get();
      //if (FhirUri.TryParse(this.Request.GetUrl(), _FhirMajorVersion, out FhirUri))
      //{
      //  string x = FhirUri.ResourseName; 
      //}


      var ResourceStore = new DomainModel.ResourceStore()
      {
        ResourceId = query.FhirId,
        IsCurrent = true,
        IsDeleted = false,
        VersionId = query.VersionId,
        ResourceBlob = IGZipper.Compress(ResourceBytes)
      };

      IResourceStoreRepository.Add(ResourceStore);
      await IResourceStoreRepository.SaveChangesAsync();

      var OutCome = new FhirApiResult()
      {
        ResourceId = ResourceStore.ResourceId,
        HttpStatusCode = System.Net.HttpStatusCode.Created,
        FhirMajorVersion = query.FhirMajorVersion,
        Resource = query.Resource,
        ResourceVersionId = ResourceStore.VersionId
      };

      return OutCome;
    }
  }
}
