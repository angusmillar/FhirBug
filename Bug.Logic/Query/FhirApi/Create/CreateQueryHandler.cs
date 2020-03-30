﻿using AutoMapper;
using Bug.Common.Compression;
using Bug.Common.DateTimeTools;
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
using Bug.Logic.Attributes;
using Bug.Logic.Service.Headers;

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
      IHeaderService IHeaderService)
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
    }

    public async Task<FhirApiTransactionalResult> Handle(CreateQuery query)
    {
      if (query.ResourceName is null)
        throw new ArgumentNullException(paramName: nameof(query.ResourceName));

      if (!IValidateQueryService.IsValid(query, out FhirResource? IsNotValidOperationOutCome))
      {
        return new FhirApiTransactionalResult(System.Net.HttpStatusCode.BadRequest, IsNotValidOperationOutCome!.FhirVersion, query.CorrelationId)
        {
          ResourceId = null,
          FhirResource = IsNotValidOperationOutCome,
          VersionId = null
        };
      }

      Bug.Common.Enums.ResourceType? ResourceType = IResourceTypeSupport.GetTypeFromName(query.ResourceName);
      if (!ResourceType.HasValue)
        throw new ArgumentNullException(nameof(ResourceType));

      var UpdateResource = new UpdateResource(query.FhirResource)
      {
        ResourceId = FhirGuidSupport.NewFhirGuid(),
        VersionId = 1,
        LastUpdated = IServerDefaultDateTimeOffSet.Now()
      };

      FhirResource UpdatedFhirResource = IUpdateResourceService.Process(UpdateResource);
      byte[] ResourceBytes = IFhirResourceJsonSerializationService.SerializeToJsonBytes(UpdatedFhirResource);

      HttpStatusCode? HttpStatusCode = await IHttpStatusCodeCache.GetAsync(System.Net.HttpStatusCode.Created);
      if (HttpStatusCode is null)
        throw new ArgumentNullException(nameof(HttpStatusCode));

      var ResourceStore = new ResourceStore()
      {
        ResourceId = UpdateResource.ResourceId,
        IsCurrent = true,
        IsDeleted = false,
        VersionId = UpdateResource.VersionId.Value,
        LastUpdated = UpdateResource.LastUpdated.Value.ToZulu(),
        ResourceBlob = IGZipper.Compress(ResourceBytes),
        ResourceTypeId = ResourceType.Value,
        FhirVersionId = UpdatedFhirResource.FhirVersion,
        MethodId = query.Method,
        HttpStatusCodeId = HttpStatusCode.Id,
        Created = UpdateResource.LastUpdated.Value.ToZulu(),
        Updated = UpdateResource.LastUpdated.Value.ToZulu()
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
        ResourceId = UpdateResource.ResourceId,
        FhirResource = UpdatedFhirResource,
        VersionId = UpdateResource.VersionId,
        Headers = await IHeaderService.AddForCreateAsync(
          UpdatedFhirResource.FhirVersion,
          query.RequestUri.Scheme,
          UpdateResource.LastUpdated.Value,
          UpdateResource.ResourceId,
          UpdateResource.VersionId.Value),
        CommitTransaction = true
      };
           
      return OutCome;
    }
  }
}
