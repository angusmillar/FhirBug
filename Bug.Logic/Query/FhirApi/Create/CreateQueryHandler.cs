using AutoMapper;
using Bug.Common.Compression;
using Bug.Common.DateTimeTools;
using Bug.Common.FhirTools;
using Bug.Logic.CacheService;
using Bug.Logic.DomainModel;
using Bug.Logic.Interfaces.Repository;
using Bug.Logic.Service.Fhir;
using Bug.Logic.Service.Indexing;
using Bug.Logic.Service.ReferentialIntegrity;
using Bug.Logic.Service.ValidatorService;
using System;
using System.Threading.Tasks;
using Bug.Logic.Attributes;
using Bug.Logic.Service.Headers;
using System.Collections.Generic;

namespace Bug.Logic.Query.FhirApi.Create
{
  [Transactional]
  public class CreateQueryHandler : IQueryHandler<CreateQuery, FhirApiTransactionalResult>
  {
    private readonly IValidateQueryService IValidateQueryService;
    private readonly IResourceStoreRepository IResourceStoreRepository;
    private readonly IResourceTypeSupport IResourceTypeSupport;
    private readonly IHttpStatusCodeCache IHttpStatusCodeCache;
    private readonly IFhirResourceJsonSerializationService IFhirResourceJsonSerializationService;
    private readonly IUpdateResourceService IUpdateResourceService;
    private readonly IServerDateTimeSupport IServerDefaultDateTimeOffSet;
    private readonly IReferentialIntegrityService IReferentialIntegrityService;
    private readonly IGZipper IGZipper;
    private readonly IIndexer IIndexer;
    private readonly IMapper IMapper;
    private readonly IHeaderService IHeaderService;
    private readonly IFhirResourceContainedSupport IFhirResourceContainedSupport;

    public CreateQueryHandler(
      IValidateQueryService IValidateQueryService,
      IResourceStoreRepository IResourceStoreRepository,
      IResourceTypeSupport IResourceTypeSupport,
      IHttpStatusCodeCache IHttpStatusCodeCache,
      IFhirResourceJsonSerializationService IFhirResourceJsonSerializationService,
      IUpdateResourceService IUpdateResourceService,
      IServerDateTimeSupport IServerDefaultDateTimeOffSet,
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
      this.IFhirResourceJsonSerializationService = IFhirResourceJsonSerializationService;
      this.IUpdateResourceService = IUpdateResourceService;
      this.IServerDefaultDateTimeOffSet = IServerDefaultDateTimeOffSet;
      this.IReferentialIntegrityService = IReferentialIntegrityService;
      this.IGZipper = IGZipper;
      this.IIndexer = IIndexer;
      this.IMapper = IMapper;
      this.IHeaderService = IHeaderService;
      this.IFhirResourceContainedSupport = IFhirResourceContainedSupport;
    }

    public async Task<FhirApiTransactionalResult> Handle(CreateQuery query)
    {
      if (query.ResourceName is null)
        throw new ArgumentNullException(paramName: nameof(query.ResourceName));

      if (!IValidateQueryService.IsValid(query, out Common.FhirTools.FhirResource? IsNotValidOperationOutCome))
      {
        return new FhirApiTransactionalResult((System.Net.HttpStatusCode)System.Net.HttpStatusCode.BadRequest, (Common.Enums.FhirVersion)IsNotValidOperationOutCome!.FhirVersion, (string)query.CorrelationId)
        {
          ResourceId = null,
          FhirResource = IsNotValidOperationOutCome,
          VersionId = null
        };
      }

      Bug.Common.Enums.ResourceType? ResourceType = IResourceTypeSupport.GetTypeFromName(query.ResourceName);
      if (!ResourceType.HasValue)
        throw new ArgumentNullException(nameof(ResourceType));

      var UpdatedResource = new UpdateResource(query.FhirResource)
      {
        ResourceId = FhirGuidSupport.NewFhirGuid(),
        VersionId = 1,
        LastUpdated = IServerDefaultDateTimeOffSet.Now()
      };

      HttpStatusCode? HttpStatusCode = await IHttpStatusCodeCache.GetAsync(System.Net.HttpStatusCode.Created);
      if (HttpStatusCode is null)
        throw new ArgumentNullException(nameof(HttpStatusCode));

      Common.FhirTools.FhirResource UpdatedFhirResource = IUpdateResourceService.Process(UpdatedResource);
      byte[] ResourceBytes = IFhirResourceJsonSerializationService.SerializeToJsonBytes(UpdatedFhirResource);

      IList<FhirContainedResource> ContainedResourceList = IFhirResourceContainedSupport.GetContainedResourceDictionary(UpdatedFhirResource);

      foreach(var Contained in ContainedResourceList)
      {
        byte[] ContainedResourceBytes = IFhirResourceJsonSerializationService.SerializeToJsonBytes(Contained);
        
        var ContainedResourceStore = new ResourceStore()
        {
          ResourceId = UpdatedResource.ResourceId,
          ContainedId = Contained.ResourceId,
          IsCurrent = true,
          IsDeleted = false,
          VersionId = UpdatedResource.VersionId.Value,
          LastUpdated = UpdatedResource.LastUpdated.Value.ToZulu(),
          ResourceBlob = IGZipper.Compress(ContainedResourceBytes),
          ResourceTypeId = Contained.ResourceType,
          FhirVersionId = Contained.FhirVersion,
          MethodId = query.Method,
          HttpStatusCodeId = HttpStatusCode.Id,
          Created = UpdatedResource.LastUpdated.Value.ToZulu(),
          Updated = UpdatedResource.LastUpdated.Value.ToZulu()
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

      var ResourceStore = new ResourceStore()
      {
        ResourceId = UpdatedResource.ResourceId,
        ContainedId = null,
        IsCurrent = true,
        IsDeleted = false,
        VersionId = UpdatedResource.VersionId.Value,
        LastUpdated = UpdatedResource.LastUpdated.Value.ToZulu(),
        ResourceBlob = IGZipper.Compress(ResourceBytes),
        ResourceTypeId = ResourceType.Value,
        FhirVersionId = UpdatedFhirResource.FhirVersion,
        MethodId = query.Method,
        HttpStatusCodeId = HttpStatusCode.Id,
        Created = UpdatedResource.LastUpdated.Value.ToZulu(),
        Updated = UpdatedResource.LastUpdated.Value.ToZulu()
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

      var OutCome = new FhirApiTransactionalResult(System.Net.HttpStatusCode.Created, query.FhirVersion, query.CorrelationId)
      {
        ResourceId = UpdatedResource.ResourceId,
        FhirResource = UpdatedFhirResource,
        VersionId = UpdatedResource.VersionId,
        Headers = await IHeaderService.AddForCreateAsync(
          UpdatedFhirResource.FhirVersion,
          query.RequestUri.Scheme,
          UpdatedResource.LastUpdated.Value,
          UpdatedResource.ResourceId,
          UpdatedResource.VersionId.Value),
        CommitTransaction = true
      };
           
      return OutCome;
    }
  }
}
