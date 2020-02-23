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
using Bug.Common.DateTimeTools;
using Bug.Logic.Service;
using Bug.Logic.DomainModel.Projection;
using Bug.Common.FhirTools;

namespace Bug.Logic.Query.FhirApi.Update
{
  public class UpdateQueryHandler : IQueryHandler<UpdateQuery, FhirApiResult>
  {
    private readonly IResourceStoreRepository IResourceStoreRepository;
    private readonly IFhirResourceIdSupport IFhirResourceIdSupport;
    private readonly IUpdateResourceService IUpdateResourceService;
    private readonly IFhirResourceJsonSerializationService IFhirResourceJsonSerializationService;
    private readonly IGZipper IGZipper;

    public UpdateQueryHandler(
      IResourceStoreRepository IResourceStoreRepository,
      IFhirResourceIdSupport IFhirResourceIdSupport,
      IUpdateResourceService IUpdateResourceService,
      IFhirResourceJsonSerializationService IFhirResourceJsonSerializationService,
      IGZipper IGZipper)
    {
      this.IResourceStoreRepository = IResourceStoreRepository;
      this.IFhirResourceIdSupport = IFhirResourceIdSupport;
      this.IUpdateResourceService = IUpdateResourceService;
      this.IFhirResourceJsonSerializationService = IFhirResourceJsonSerializationService;      
      this.IGZipper = IGZipper;
    }

    public async Task<FhirApiResult> Handle(UpdateQuery query)
    {
      if (query is null)
        throw new NullReferenceException();

      if (query.FhirResource is null)
        throw new NullReferenceException();

      string ResourceId = IFhirResourceIdSupport.GetFhirId(query.FhirResource);
      byte[] ResourceBytes = IFhirResourceJsonSerializationService.SerializeToJsonBytes(query.FhirResource);

      ResourceStore PreviousResourseStore = await IResourceStoreRepository.GetCurrentNoBlobAsync(ResourceId);
      if (PreviousResourseStore != null)
      {
        PreviousResourseStore.IsCurrent = false;
        IResourceStoreRepository.Update(PreviousResourseStore);
      }
      string NewVersionId = FhirGuidSupport.NewFhirGuid();
      DateTime NewLastUpdated = DateTimeOffset.Now.ToZulu();
      FhirResource UpdatedFhirResource = IUpdateResourceService.Process(new UpdateResource()
      {
        FhirResource = query.FhirResource,
        LastUpdated = NewLastUpdated,
        VersionId = NewVersionId
      });

      var ResourceStore = new ResourceStore()
      {
        ResourceId = ResourceId,
        IsCurrent = true,
        IsDeleted = false,
        VersionId = NewVersionId,
        LastUpdated = NewLastUpdated,
        ResourceBlob = IGZipper.Compress(IFhirResourceJsonSerializationService.SerializeToJsonBytes(UpdatedFhirResource))   
      };

      
      IResourceStoreRepository.Add(ResourceStore);      
      await IResourceStoreRepository.SaveChangesAsync();


      var OutCome = new FhirApiResult()
      {
        HttpStatusCode = System.Net.HttpStatusCode.OK,
        FhirMajorVersion = query.FhirMajorVersion,
        FhirResource = query.FhirResource,
        ResourceId = ResourceId,
        VersionId = NewVersionId
      };

      return OutCome;
    }
  }
}
