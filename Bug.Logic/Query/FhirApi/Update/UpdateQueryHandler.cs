using Bug.Common.Compression;
using Bug.Common.DateTimeTools;
using Bug.Common.Enums;
using Bug.Common.FhirTools;
using Bug.Logic.CacheService;
using Bug.Logic.DomainModel;
using Bug.Logic.Interfaces.Repository;
using Bug.Logic.Service;
using Bug.Logic.Service.TableService;
using Bug.Logic.Service.ValidatorService;
using System;
using System.Threading.Tasks;

namespace Bug.Logic.Query.FhirApi.Update
{
  public class UpdateQueryHandler : IQueryHandler<UpdateQuery, FhirApiResult>
  {
    private readonly IValidateQueryService IValidateQueryService;
    private readonly IResourceStoreRepository IResourceStoreRepository;
    private readonly IFhirVersionTableService IFhirVersionTableService;
    private readonly IResourceNameTableService IResourceNameTableService;
    private readonly IHttpStatusCodeCache IHttpStatusCodeCache;
    private readonly IFhirResourceIdSupport IFhirResourceIdSupport;
    private readonly IUpdateResourceService IUpdateResourceService;
    private readonly IServerDateTimeSupport IServerDefaultDateTimeOffSet;
    private readonly IFhirResourceJsonSerializationService IFhirResourceJsonSerializationService;
    private readonly IGZipper IGZipper;

    public UpdateQueryHandler(
      IValidateQueryService IValidateQueryService,
      IResourceStoreRepository IResourceStoreRepository,
      IFhirVersionTableService IFhirVersionTableService,      
      IResourceNameTableService IResourceNameTableService,
      IHttpStatusCodeCache IHttpStatusCodeCache,
      IFhirResourceIdSupport IFhirResourceIdSupport,
      IUpdateResourceService IUpdateResourceService,
      IServerDateTimeSupport IServerDefaultDateTimeOffSet,
      IFhirResourceJsonSerializationService IFhirResourceJsonSerializationService,
      IGZipper IGZipper)
    {
      this.IValidateQueryService = IValidateQueryService;
      this.IResourceStoreRepository = IResourceStoreRepository;
      this.IFhirVersionTableService = IFhirVersionTableService;      
      this.IResourceNameTableService = IResourceNameTableService;
      this.IHttpStatusCodeCache = IHttpStatusCodeCache;
      this.IFhirResourceIdSupport = IFhirResourceIdSupport;
      this.IUpdateResourceService = IUpdateResourceService;
      this.IServerDefaultDateTimeOffSet = IServerDefaultDateTimeOffSet;
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
      
      System.Net.HttpStatusCode FinalyHttpStatusCode = System.Net.HttpStatusCode.Created;      
      int NewVersionId = 1;
      
      ResourceStore? PreviousResourseStore = await IResourceStoreRepository.GetCurrentMetaAsync(query.FhirResource.FhirMajorVersion, query.ResourceName, ResourceId);
      if (PreviousResourseStore is object)
      {
        FinalyHttpStatusCode = System.Net.HttpStatusCode.OK;
        PreviousResourseStore.IsCurrent = false;
        NewVersionId = PreviousResourseStore.VersionId + 1;
        IResourceStoreRepository.UpdateIsCurrent(PreviousResourseStore);
      }

      DateTimeOffset NewLastUpdated = IServerDefaultDateTimeOffSet.Now();
      FhirResource UpdatedFhirResource = IUpdateResourceService.Process(
        new UpdateResource(query.FhirResource)
        {          
          VersionId = NewVersionId,
          LastUpdated = NewLastUpdated
        });

      HttpStatusCode? HttpStatusCode = await IHttpStatusCodeCache.GetAsync(FinalyHttpStatusCode);
      if (HttpStatusCode is null)
        throw new ArgumentNullException($"Unable to locate {nameof(HttpStatusCode)} of type {FinalyHttpStatusCode.GetCode()} in the database.");

      var ResourceStore = new ResourceStore()
      {
        ResourceId = ResourceId,
        IsCurrent = true,
        IsDeleted = false,
        VersionId = NewVersionId,
        LastUpdated = NewLastUpdated.ToZulu(),
        ResourceBlob = IGZipper.Compress(IFhirResourceJsonSerializationService.SerializeToJsonBytes(UpdatedFhirResource)),
        FkResourceNameId = ResourceName.Id,
        FkFhirVersionId = FhirVersion.Id,
        FkHttpStatusCodeId = HttpStatusCode.Id,
        FkMethodId = query.Method
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
