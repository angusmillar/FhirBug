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
using Bug.Logic.Service;
using Bug.Common.DateTimeTools;
using Bug.Common.FhirTools;

namespace Bug.Logic.Query.FhirApi.Create
{
  public class CreateQueryHandler : IQueryHandler<CreateQuery, FhirApiResult>
  {        
    private readonly IResourceStoreRepository IResourceStoreRepository;
    private readonly IFhirResourceJsonSerializationService IFhirResourceJsonSerializationService;
    private readonly IUpdateResourceService IUpdateResourceService;
    private readonly IGZipper IGZipper;

    public CreateQueryHandler( 
      IResourceStoreRepository IResourceStoreRepository,
      IFhirResourceJsonSerializationService IFhirResourceJsonSerializationService,     
      IUpdateResourceService IUpdateResourceService,
      IGZipper IGZipper)
    {            
      this.IResourceStoreRepository = IResourceStoreRepository;
      this.IFhirResourceJsonSerializationService = IFhirResourceJsonSerializationService;      
      this.IUpdateResourceService = IUpdateResourceService;
      this.IGZipper = IGZipper;      
    }

    public async Task<FhirApiResult> Handle(CreateQuery query)
    {
      var UpdateResource = new UpdateResource();
      UpdateResource.ResourceId = FhirGuidSupport.NewFhirGuid();
      UpdateResource.VersionId = FhirGuidSupport.NewFhirGuid();
      UpdateResource.LastUpdated = DateTimeOffset.Now.ToZulu();
      UpdateResource.FhirResource = query.FhirResource;

      FhirResource UpdatedFhirResource = IUpdateResourceService.Process(UpdateResource);
      byte[] ResourceBytes = IFhirResourceJsonSerializationService.SerializeToJsonBytes(UpdatedFhirResource);      

      //var FhirUri = IFhirUriFactory.Get();
      //if (FhirUri.TryParse(this.Request.GetUrl(), _FhirMajorVersion, out FhirUri))
      //{
      //  string x = FhirUri.ResourseName; 
      //}


      var ResourceStore = new ResourceStore()
      {
        ResourceId = UpdateResource.ResourceId,
        IsCurrent = true,
        IsDeleted = false,
        VersionId = UpdateResource.VersionId,
        LastUpdated = UpdateResource.LastUpdated.Value,
        ResourceBlob = IGZipper.Compress(ResourceBytes)
      };

      IResourceStoreRepository.Add(ResourceStore);
      await IResourceStoreRepository.SaveChangesAsync();

      var OutCome = new FhirApiResult()
      {
        ResourceId = UpdateResource.ResourceId,
        HttpStatusCode = System.Net.HttpStatusCode.Created,
        FhirMajorVersion = UpdatedFhirResource.FhirMajorVersion,
        FhirResource = UpdatedFhirResource,
        VersionId = UpdateResource.VersionId
      };

      return OutCome;
    }
  }
}
