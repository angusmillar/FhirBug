using Bug.Common.Compression;
using Bug.Common.DateTimeTools;
using Bug.Common.Enums;
using Bug.Common.Exceptions;
using Bug.Common.FhirTools;
using Bug.Logic.CacheService;
using Bug.Logic.DomainModel;
using Bug.Logic.Interfaces.Repository;
using Bug.Logic.Service;
using Bug.Logic.Service.ValidatorService;
using System;
using System.Threading.Tasks;

namespace Bug.Logic.Query.FhirApi.Delete
{
  public class DeleteQueryHandler : IQueryHandler<DeleteQuery, FhirApiResult>
  {
    private readonly IValidateQueryService IValidateQueryService;
    private readonly IResourceStoreRepository IResourceStoreRepository;    
    private readonly IHttpStatusCodeCache IHttpStatusCodeCache;
    private readonly IResourceTypeSupport IResourceTypeSupport;

    public DeleteQueryHandler(
      IValidateQueryService IValidateQueryService,
      IResourceStoreRepository IResourceStoreRepository,      
      IHttpStatusCodeCache IHttpStatusCodeCache,
      IResourceTypeSupport IResourceTypeSupport)
    {
      this.IValidateQueryService = IValidateQueryService;
      this.IResourceStoreRepository = IResourceStoreRepository;      
      this.IHttpStatusCodeCache = IHttpStatusCodeCache;
      this.IResourceTypeSupport = IResourceTypeSupport;
    }

    public async Task<FhirApiResult> Handle(DeleteQuery query)
    {

      if (!IValidateQueryService.IsValid(query, out FhirResource? IsNotValidOperationOutCome))
      {
        return new FhirApiResult(System.Net.HttpStatusCode.BadRequest, IsNotValidOperationOutCome!.FhirMajorVersion)
        {
          ResourceId = null,
          FhirResource = IsNotValidOperationOutCome,
          VersionId = null
        };
      }

      System.Net.HttpStatusCode FinalyHttpStatusCode = System.Net.HttpStatusCode.NoContent;
      HttpStatusCode? HttpStatusCode = await IHttpStatusCodeCache.GetAsync(FinalyHttpStatusCode);
      if (HttpStatusCode is null)
        throw new ArgumentNullException($"Unable to locate {nameof(HttpStatusCode)} of type {FinalyHttpStatusCode.ToString()} in the database.");

      Bug.Common.Enums.ResourceType? ResourceType = IResourceTypeSupport.GetTypeFromName(query.ResourceName);
      if (!ResourceType.HasValue)
        throw new ArgumentNullException(nameof(ResourceType));

      int? NewVersionId = null;
      ResourceStore? PreviousResourseStore = await IResourceStoreRepository.GetCurrentMetaAsync(query.FhirVersion, ResourceType.Value, query.ResourceId);
      if (PreviousResourseStore is object)
      {
        //FinalyHttpStatusCode = System.Net.HttpStatusCode.OK;
        PreviousResourseStore.IsCurrent = false;
        NewVersionId = PreviousResourseStore.VersionId + 1;
        IResourceStoreRepository.UpdateIsCurrent(PreviousResourseStore);

        var ResourceStore = new ResourceStore()
        {
          ResourceId = query.ResourceId,
          IsCurrent = true,
          IsDeleted = true,
          VersionId = NewVersionId.Value,
          LastUpdated = DateTimeOffset.Now.ToZulu(),
          ResourceBlob = null,
          FkResourceTypeId = PreviousResourseStore.FkResourceTypeId,
          FkFhirVersionId = query.FhirVersion,
          FkHttpStatusCodeId = HttpStatusCode.Id,
          FkMethodId = query.Method
        };

        IResourceStoreRepository.Add(ResourceStore);
        await IResourceStoreRepository.SaveChangesAsync();
      }

      var OutCome = new FhirApiResult(FinalyHttpStatusCode, query.FhirVersion)
      {
        FhirResource = null,
        ResourceId = query.ResourceId,
        VersionId = NewVersionId
      };

      return OutCome;

    }
  }
}
