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
using Bug.Logic.Service.TableService;

namespace Bug.Logic.Query.FhirApi.Update
{
  public class UpdateQueryHandler : IQueryHandler<UpdateQuery, FhirApiResult>
  {
    private readonly IValidateQueryService IValidateQueryService;
    private readonly IResourceStoreRepository IResourceStoreRepository;
    private readonly IFhirVersionTableService IFhirVersionTableService;
    private readonly IResourceNameTableService IResourceNameTableService;
    private readonly IMethodTableService IMethodTableService;
    private readonly IFhirResourceIdSupport IFhirResourceIdSupport;
    private readonly IUpdateResourceService IUpdateResourceService;
    private readonly IFhirResourceJsonSerializationService IFhirResourceJsonSerializationService;
    private readonly IGZipper IGZipper;

    public UpdateQueryHandler(
      IValidateQueryService IValidateQueryService,
      IResourceStoreRepository IResourceStoreRepository,
      IFhirVersionTableService IFhirVersionTableService,      
      IResourceNameTableService IResourceNameTableService,
      IMethodTableService IMethodTableService,
      IFhirResourceIdSupport IFhirResourceIdSupport,
      IUpdateResourceService IUpdateResourceService,
      IFhirResourceJsonSerializationService IFhirResourceJsonSerializationService,
      IGZipper IGZipper)
    {
      this.IValidateQueryService = IValidateQueryService;
      this.IResourceStoreRepository = IResourceStoreRepository;
      this.IFhirVersionTableService = IFhirVersionTableService;      
      this.IResourceNameTableService = IResourceNameTableService;
      this.IMethodTableService = IMethodTableService;
      this.IFhirResourceIdSupport = IFhirResourceIdSupport;
      this.IUpdateResourceService = IUpdateResourceService;
      this.IFhirResourceJsonSerializationService = IFhirResourceJsonSerializationService;
      this.IGZipper = IGZipper;
    }

    public async Task<FhirApiResult> Handle(UpdateQuery query)
    {
      if (query is null)
        throw new NullReferenceException();
      
      if (!IValidateQueryService.IsValid(query, out FhirResource? IsNotValidOperationOutCome))
      {
        return new FhirApiResult(System.Net.HttpStatusCode.BadRequest, IsNotValidOperationOutCome!.FhirMajorVersion)
        {
          ResourceId = null,
          FhirResource = IsNotValidOperationOutCome,
          VersionId = null
        };
      }

      string ResourceId = IFhirResourceIdSupport.GetResourceId(query.FhirResource);
      ResourceName ResourceName = await IResourceNameTableService.GetSetResourceName(query.ResourceName);
      FhirVersion FhirVersion = await IFhirVersionTableService.GetSetFhirVersion(query.FhirResource.FhirMajorVersion);
      Method Method = await IMethodTableService.GetSetMethod(query.Method);

      int NewVersionId = 1;
      System.Net.HttpStatusCode FinalyHttpStatusCode = System.Net.HttpStatusCode.Created;
      ResourceStore? PreviousResourseStore = await IResourceStoreRepository.GetCurrentMetaAsync(query.FhirResource.FhirMajorVersion, query.ResourceName, ResourceId);
      if (PreviousResourseStore is object)
      {
        FinalyHttpStatusCode = System.Net.HttpStatusCode.OK;
        PreviousResourseStore.IsCurrent = false;
        NewVersionId = PreviousResourseStore.VersionId + 1;
        IResourceStoreRepository.UpdateIsCurrent(PreviousResourseStore);
      }

      DateTime NewLastUpdated = DateTimeOffset.Now.ToZulu();
      FhirResource UpdatedFhirResource = IUpdateResourceService.Process(
        new UpdateResource(query.FhirResource)
        {          
          VersionId = NewVersionId,
          LastUpdated = NewLastUpdated
        });

      var ResourceStore = new ResourceStore()
      {
        ResourceId = ResourceId,
        IsCurrent = true,
        IsDeleted = false,
        VersionId = NewVersionId,
        LastUpdated = NewLastUpdated,
        ResourceBlob = IGZipper.Compress(IFhirResourceJsonSerializationService.SerializeToJsonBytes(UpdatedFhirResource)),
        FkResourceNameId = ResourceName.Id,
        FkFhirVersionId = FhirVersion.Id,
        FkMethodId = Method.Id
      };

      IResourceStoreRepository.Add(ResourceStore);
      await IResourceStoreRepository.SaveChangesAsync();

      
      var OutCome = new FhirApiResult(FinalyHttpStatusCode, query.FhirMajorVersion)
      {
        FhirResource = query.FhirResource,
        ResourceId = ResourceId,
        VersionId = NewVersionId
      };

      return OutCome;
    }
  }
}
