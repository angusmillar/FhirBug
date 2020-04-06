using Bug.Common.Compression;
using Bug.Common.DateTimeTools;
using Bug.Common.Enums;
using Bug.Common.Exceptions;
using Bug.Common.FhirTools;
using Bug.Logic.Attributes;
using Bug.Logic.CacheService;
using Bug.Logic.DomainModel;
using Bug.Logic.Interfaces.Repository;
using Bug.Logic.Service;
using Bug.Logic.Service.Headers;
using Bug.Logic.Service.ReferentialIntegrity;
using Bug.Logic.Service.ValidatorService;
using System;
using System.Threading.Tasks;

namespace Bug.Logic.Query.FhirApi.Delete
{
  [Transactional]
  public class DeleteQueryHandler : IQueryHandler<DeleteQuery, FhirApiTransactionalResult>
  {
    private readonly IValidateQueryService IValidateQueryService;
    private readonly IResourceStoreRepository IResourceStoreRepository;    
    private readonly IHttpStatusCodeCache IHttpStatusCodeCache;
    private readonly IResourceTypeSupport IResourceTypeSupport;
    private readonly IHeaderService IHeaderService;
    private readonly IReferentialIntegrityService IReferentialIntegrityService;

    public DeleteQueryHandler(
      IValidateQueryService IValidateQueryService,
      IResourceStoreRepository IResourceStoreRepository,      
      IHttpStatusCodeCache IHttpStatusCodeCache,
      IResourceTypeSupport IResourceTypeSupport,
      IHeaderService IHeaderService,
      IReferentialIntegrityService IReferentialIntegrityService)
    {
      this.IValidateQueryService = IValidateQueryService;
      this.IResourceStoreRepository = IResourceStoreRepository;      
      this.IHttpStatusCodeCache = IHttpStatusCodeCache;
      this.IResourceTypeSupport = IResourceTypeSupport;
      this.IHeaderService = IHeaderService;
      this.IReferentialIntegrityService = IReferentialIntegrityService;
    }

    public async Task<FhirApiTransactionalResult> Handle(DeleteQuery query)
    {
      if (!IValidateQueryService.IsValid(query, out Common.FhirTools.FhirResource? IsNotValidOperationOutCome))
      {
        return new FhirApiTransactionalResult((System.Net.HttpStatusCode)System.Net.HttpStatusCode.BadRequest, (Common.Enums.FhirVersion)IsNotValidOperationOutCome!.FhirVersion, (string)query.CorrelationId)
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
        var ReferentialIntegrityOutcome = await IReferentialIntegrityService.CheckOnDelete(query.FhirVersion, ResourceType.Value, query.ResourceId);
        if (ReferentialIntegrityOutcome.IsError)
        {
          return new FhirApiTransactionalResult(System.Net.HttpStatusCode.Conflict, query.FhirVersion, query.CorrelationId)
          {
            ResourceId = null,
            FhirResource = ReferentialIntegrityOutcome.FhirResource,
            VersionId = null
          };
        }

        PreviousResourseStore.IsCurrent = false;
        NewVersionId = PreviousResourseStore.VersionId + 1;
        IResourceStoreRepository.UpdateCurrent(PreviousResourseStore);

        DateTime Now = DateTimeOffset.Now.ToZulu();
        var ResourceStore = new ResourceStore()
        {
          ResourceId = query.ResourceId,
          IsCurrent = true,
          IsDeleted = true,
          VersionId = NewVersionId.Value,
          LastUpdated = Now,
          ResourceBlob = null,
          ResourceTypeId = PreviousResourseStore.ResourceTypeId,
          FhirVersionId = query.FhirVersion,
          HttpStatusCodeId = HttpStatusCode.Id,
          MethodId = query.Method,
          Created = Now,
          Updated = Now
        };

        IResourceStoreRepository.Add(ResourceStore);
        await IResourceStoreRepository.SaveChangesAsync();
      }

      var OutCome = new FhirApiTransactionalResult(FinalyHttpStatusCode, query.FhirVersion, query.CorrelationId)
      {
        FhirResource = null,
        ResourceId = query.ResourceId,
        VersionId = NewVersionId,
        Headers = IHeaderService.GetForDelete(NewVersionId),
        CommitTransaction = true
      };

      return OutCome;

    }
  }
}
