using AutoMapper;
using Bug.Common.Compression;
using Bug.Common.DateTimeTools;
using Bug.Common.FhirTools;
using Bug.Logic.Attributes;
using Bug.Logic.CacheService;
using Bug.Logic.DomainModel;
using Bug.Logic.Interfaces.Repository;
using Bug.Logic.Service.Fhir;
using Bug.Logic.Service.Headers;
using Bug.Logic.Service.Indexing;
using Bug.Logic.Service.ReferentialIntegrity;
using Bug.Logic.Service.ValidatorService;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bug.Logic.Query.FhirApi.Update
{
  [Transactional]
  public class UpdateQueryHandler : IQueryHandler<UpdateQuery, FhirApiTransactionalResult>
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
    private readonly IHeaderService IHeaderService;
    private readonly IFhirResourceContainedSupport IFhirResourceContainedSupport;


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
      IMapper IMapper,
      IHeaderService IHeaderService,
      IFhirResourceContainedSupport IFhirResourceContainedSupport)
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
      this.IHeaderService = IHeaderService;
      this.IFhirResourceContainedSupport = IFhirResourceContainedSupport;
    }

    public async Task<FhirApiTransactionalResult> Handle(UpdateQuery query)
    {
      if (query is null)
        throw new NullReferenceException();
      
      if (!IValidateQueryService.IsValid(query, out Common.FhirTools.FhirResource? IsNotValidOperationOutCome))
      {
        return new FhirApiTransactionalResult((System.Net.HttpStatusCode)System.Net.HttpStatusCode.BadRequest, (Common.Enums.FhirVersion)IsNotValidOperationOutCome!.FhirVersion, (string)query.CorrelationId)
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

      ResourceStore? PreviousResourseStore = await IResourceStoreRepository.GetCurrentMetaAsync(query.FhirResource.FhirVersion, ResourceType.Value, query.ResourceId);
      if (PreviousResourseStore is object)
      {
        FinalyHttpStatusCode = System.Net.HttpStatusCode.OK;
        PreviousResourseStore.IsCurrent = false;
        PreviousResourseStore.Updated = NewLastUpdated.ToZulu();
        NewVersionId = PreviousResourseStore.VersionId + 1;        
        await IResourceStoreRepository.UpdateCurrentAsync(PreviousResourseStore);
      }

      Common.FhirTools.FhirResource UpdatedResource = IUpdateResourceService.Process(
        new UpdateResource(query.FhirResource)
        {          
          VersionId = NewVersionId,
          LastUpdated = NewLastUpdated
        });      
      
      HttpStatusCode? HttpStatusCode = await IHttpStatusCodeCache.GetAsync(FinalyHttpStatusCode);
      if (HttpStatusCode is null)
        throw new ArgumentNullException($"Unable to locate {nameof(HttpStatusCode)} of type {FinalyHttpStatusCode.ToString()} in the database.");


      //Add any Contained resources and indexes
      IList<FhirContainedResource> ContainedResourceList = IFhirResourceContainedSupport.GetContainedResourceDictionary(UpdatedResource);

      foreach (var Contained in ContainedResourceList)
      {
        byte[] ContainedResourceBytes = IFhirResourceJsonSerializationService.SerializeToJsonBytes(Contained);

        var ContainedResourceStore = new ResourceStore()
        {
          ResourceId = query.ResourceId,
          ContainedId = Contained.ResourceId,
          IsCurrent = true,
          IsDeleted = false,
          VersionId = NewVersionId,
          LastUpdated = NewLastUpdated.ToZulu(),
          ResourceBlob = IGZipper.Compress(ContainedResourceBytes),
          ResourceTypeId = Contained.ResourceType,
          FhirVersionId = Contained.FhirVersion,
          MethodId = query.Method,
          HttpStatusCodeId = HttpStatusCode.Id,
          Created = NewLastUpdated.ToZulu(),
          Updated = NewLastUpdated.ToZulu()
        };

        IndexerOutcome ContainedIndexerOutcome = await IIndexer.Process(Contained, Contained.ResourceType);

        var ContainedReferentialIntegrityOutcome = await IReferentialIntegrityService.CheckOnCommit(Contained.FhirVersion, ContainedIndexerOutcome.ReferenceIndexList);
        if (ContainedReferentialIntegrityOutcome.IsError)
        {
          return new FhirApiTransactionalResult(System.Net.HttpStatusCode.Conflict, Contained.FhirVersion, query.CorrelationId)
          {
            ResourceId = null,
            FhirResource = ContainedReferentialIntegrityOutcome.FhirResource,
            VersionId = null
          };
        }

        IMapper.Map(ContainedIndexerOutcome, ContainedResourceStore);
        IResourceStoreRepository.Add(ContainedResourceStore);
      }

      //Add the main resource and indexes
      var ResourceStore = new ResourceStore()
      {
        ResourceId = query.ResourceId,
        IsCurrent = true,
        IsDeleted = false,
        VersionId = NewVersionId,
        LastUpdated = NewLastUpdated.ToZulu(),
        ResourceBlob = IGZipper.Compress(IFhirResourceJsonSerializationService.SerializeToJsonBytes(UpdatedResource)),
        ResourceTypeId = ResourceType.Value,
        FhirVersionId = query.FhirVersion,
        HttpStatusCodeId = HttpStatusCode.Id,
        MethodId = query.Method,
        Created = NewLastUpdated.ToZulu(),
        Updated = NewLastUpdated.ToZulu(),
      };

      IndexerOutcome IndexerOutcome = await IIndexer.Process(query.FhirResource, ResourceType.Value);

      var ReferentialIntegrityOutcome = await IReferentialIntegrityService.CheckOnCommit(query.FhirVersion, IndexerOutcome.ReferenceIndexList);
      if (ReferentialIntegrityOutcome.IsError)
      {
        return new FhirApiTransactionalResult(System.Net.HttpStatusCode.Conflict, query.FhirVersion, query.CorrelationId)
        {
          ResourceId = null,
          FhirResource = ReferentialIntegrityOutcome.FhirResource,
          VersionId = null
        };
      }
      
      IMapper.Map(IndexerOutcome, ResourceStore);

      IResourceStoreRepository.Add(ResourceStore);
      await IResourceStoreRepository.SaveChangesAsync();

      var OutCome = new FhirApiTransactionalResult(FinalyHttpStatusCode, query.FhirVersion, query.CorrelationId)
      {
        FhirResource = query.FhirResource,
        ResourceId = query.ResourceId,
        VersionId = NewVersionId,
        Headers = await IHeaderService.GetForUpdateAsync(
          query.FhirResource.FhirVersion,
          query.RequestUri.Scheme,
          NewLastUpdated,
          query.ResourceId,
          NewVersionId),
        CommitTransaction = true
      };

      return OutCome;
    }
  }
}
