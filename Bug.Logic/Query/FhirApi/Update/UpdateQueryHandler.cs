using AutoMapper;
using Bug.Common.Compression;
using Bug.Common.DateTimeTools;
using Bug.Common.Enums;
using Bug.Common.FhirTools;
using Bug.Logic.CacheService;
using Bug.Logic.DomainModel;
using Bug.Logic.Interfaces.Repository;
using Bug.Logic.Service;
using Bug.Logic.Service.Indexing;
using Bug.Logic.Service.ReferentialIntegrity;
using Bug.Logic.Service.ValidatorService;
using System;
using System.Threading.Tasks;

namespace Bug.Logic.Query.FhirApi.Update
{
  public class UpdateQueryHandler : IQueryHandler<UpdateQuery, FhirApiResult>
  {
    private readonly IValidateQueryService IValidateQueryService;
    private readonly IResourceStoreRepository IResourceStoreRepository;    
    private readonly IResourceTypeSupport IResourceTypeSupport;
    private readonly IHttpStatusCodeCache IHttpStatusCodeCache;    
    private readonly IUpdateResourceService IUpdateResourceService;
    private readonly IServerDateTimeSupport IServerDefaultDateTimeOffSet;
    private readonly IFhirResourceJsonSerializationService IFhirResourceJsonSerializationService;
    private readonly IReferentialIntegrityService IReferentialIntegrityService;
    private readonly IGZipper IGZipper;
    private readonly IIndexer IIndexer;
    private readonly IMapper IMapper;


    public UpdateQueryHandler(
      IValidateQueryService IValidateQueryService,
      IResourceStoreRepository IResourceStoreRepository,
      IResourceTypeSupport IResourceTypeSupport,
      IHttpStatusCodeCache IHttpStatusCodeCache,      
      IUpdateResourceService IUpdateResourceService,
      IServerDateTimeSupport IServerDefaultDateTimeOffSet,
      IFhirResourceJsonSerializationService IFhirResourceJsonSerializationService,
      IReferentialIntegrityService IReferentialIntegrityService,
      IGZipper IGZipper,
      IIndexer IIndexer,
      IMapper IMapper)
    {
      this.IValidateQueryService = IValidateQueryService;
      this.IResourceStoreRepository = IResourceStoreRepository;      
      this.IResourceTypeSupport = IResourceTypeSupport;
      this.IHttpStatusCodeCache = IHttpStatusCodeCache;      
      this.IUpdateResourceService = IUpdateResourceService;
      this.IServerDefaultDateTimeOffSet = IServerDefaultDateTimeOffSet;
      this.IFhirResourceJsonSerializationService = IFhirResourceJsonSerializationService;
      this.IReferentialIntegrityService = IReferentialIntegrityService;
      this.IGZipper = IGZipper;
      this.IIndexer = IIndexer;
      this.IMapper = IMapper;
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

      System.Net.HttpStatusCode FinalyHttpStatusCode = System.Net.HttpStatusCode.Created;      
      int NewVersionId = 1;
     
      Bug.Common.Enums.ResourceType? ResourceType = IResourceTypeSupport.GetTypeFromName(query.ResourceName);
      if (!ResourceType.HasValue)
        throw new ArgumentNullException(nameof(ResourceType));

      DateTimeOffset NewLastUpdated = IServerDefaultDateTimeOffSet.Now();

      ResourceStore? PreviousResourseStore = await IResourceStoreRepository.GetCurrentMetaAsync(query.FhirResource.FhirMajorVersion, ResourceType.Value, query.ResourceId);
      if (PreviousResourseStore is object)
      {
        FinalyHttpStatusCode = System.Net.HttpStatusCode.OK;
        PreviousResourseStore.IsCurrent = false;
        PreviousResourseStore.Updated = NewLastUpdated.ToZulu();
        NewVersionId = PreviousResourseStore.VersionId + 1;        
        IResourceStoreRepository.UpdateCurrent(PreviousResourseStore);
      }

      
      FhirResource UpdatedFhirResource = IUpdateResourceService.Process(
        new UpdateResource(query.FhirResource)
        {          
          VersionId = NewVersionId,
          LastUpdated = NewLastUpdated
        });
      
      
      HttpStatusCode? HttpStatusCode = await IHttpStatusCodeCache.GetAsync(FinalyHttpStatusCode);
      if (HttpStatusCode is null)
        throw new ArgumentNullException($"Unable to locate {nameof(HttpStatusCode)} of type {FinalyHttpStatusCode.ToString()} in the database.");

      
      var ResourceStore = new ResourceStore()
      {
        ResourceId = query.ResourceId,
        IsCurrent = true,
        IsDeleted = false,
        VersionId = NewVersionId,
        LastUpdated = NewLastUpdated.ToZulu(),
        ResourceBlob = IGZipper.Compress(IFhirResourceJsonSerializationService.SerializeToJsonBytes(UpdatedFhirResource)),
        ResourceTypeId = ResourceType.Value,
        FhirVersionId = query.FhirVersion,
        HttpStatusCodeId = HttpStatusCode.Id,
        MethodId = query.Method,
        Created = NewLastUpdated.ToZulu(),
        Updated = NewLastUpdated.ToZulu(),
      };

      IndexerOutcome IndexerOutcome = await IIndexer.Process(query.FhirResource, ResourceType.Value);

      var ReferentialIntegrityOutcome = await IReferentialIntegrityService.Check(query.FhirVersion, IndexerOutcome.ReferenceIndexList);
      if (ReferentialIntegrityOutcome.IsError)
      {
        return new FhirApiResult(System.Net.HttpStatusCode.BadRequest, query.FhirVersion)
        {
          ResourceId = null,
          FhirResource = ReferentialIntegrityOutcome.FhirResource,
          VersionId = null
        };
      }
      
      IMapper.Map(IndexerOutcome, ResourceStore);

      IResourceStoreRepository.Add(ResourceStore);
      await IResourceStoreRepository.SaveChangesAsync();
      
      var OutCome = new FhirApiResult(FinalyHttpStatusCode, query.FhirVersion)
      {
        FhirResource = query.FhirResource,
        ResourceId = query.ResourceId,
        VersionId = NewVersionId
      };

      return OutCome;
    }
  }
}
