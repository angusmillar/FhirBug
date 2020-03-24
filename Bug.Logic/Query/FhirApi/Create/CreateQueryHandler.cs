﻿using AutoMapper;
using Bug.Common.Compression;
using Bug.Common.DateTimeTools;
using Bug.Common.FhirTools;
using Bug.Logic.CacheService;
using Bug.Logic.DomainModel;
using Bug.Logic.Interfaces.Repository;
using Bug.Logic.Service;
using Bug.Logic.Service.Indexing;
using Bug.Logic.Service.ValidatorService;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bug.Logic.Query.FhirApi.Create
{
  public class CreateQueryHandler : IQueryHandler<CreateQuery, FhirApiResult>
  {
    
    private readonly IValidateQueryService IValidateQueryService;
    private readonly IResourceStoreRepository IResourceStoreRepository;
    private readonly IResourceTypeSupport IResourceTypeSupport;
    private readonly IHttpStatusCodeCache IHttpStatusCodeCache;    
    private readonly IFhirResourceJsonSerializationService IFhirResourceJsonSerializationService;
    private readonly IUpdateResourceService IUpdateResourceService;
    private readonly IServerDateTimeSupport IServerDefaultDateTimeOffSet;
    private readonly IGZipper IGZipper;
    private readonly IIndexer IIndexer;
    private readonly IMapper IMapper;

    public CreateQueryHandler(
      IValidateQueryService IValidateQueryService,
      IResourceStoreRepository IResourceStoreRepository,
      IResourceTypeSupport IResourceTypeSupport,      
      IHttpStatusCodeCache IHttpStatusCodeCache,      
      IFhirResourceJsonSerializationService IFhirResourceJsonSerializationService,     
      IUpdateResourceService IUpdateResourceService,
      IServerDateTimeSupport IServerDefaultDateTimeOffSet,
      IGZipper IGZipper,
      IIndexer IIndexer,
      IMapper IMapper)
    {
      this.IValidateQueryService = IValidateQueryService;
      this.IResourceStoreRepository = IResourceStoreRepository;
      this.IResourceTypeSupport = IResourceTypeSupport;      
      this.IHttpStatusCodeCache = IHttpStatusCodeCache;
      this.IFhirResourceJsonSerializationService = IFhirResourceJsonSerializationService;      
      this.IUpdateResourceService = IUpdateResourceService;
      this.IServerDefaultDateTimeOffSet = IServerDefaultDateTimeOffSet;
      this.IGZipper = IGZipper;
      this.IIndexer = IIndexer;
      this.IMapper = IMapper;
    }

    public async Task<FhirApiResult> Handle(CreateQuery query)
    {
      if (query.ResourceName is null)
        throw new ArgumentNullException(paramName: nameof(query.ResourceName));
      
      if (!IValidateQueryService.IsValid(query, out FhirResource? IsNotValidOperationOutCome))
      {
        return new FhirApiResult(System.Net.HttpStatusCode.BadRequest, IsNotValidOperationOutCome!.FhirMajorVersion)
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

      var x = await IIndexer.Process(query.FhirResource, ResourceType.Value);

      //var DateTimeList = new List<IndexDateTime>();
      //IMapper.Map(x.DateTimeIndexList, DateTimeList);

      var ResourceStore = new ResourceStore()
      {
        ResourceId = UpdateResource.ResourceId,
        IsCurrent = true,
        IsDeleted = false,
        VersionId = UpdateResource.VersionId.Value,
        LastUpdated = UpdateResource.LastUpdated.Value.ToZulu(),
        ResourceBlob = IGZipper.Compress(ResourceBytes),
        ResourceTypeId = ResourceType.Value,
        FhirVersionId = UpdatedFhirResource.FhirMajorVersion,
        MethodId = query.Method,
        HttpStatusCodeId = HttpStatusCode.Id,  
        Created = UpdateResource.LastUpdated.Value.ToZulu(),
        Updated = UpdateResource.LastUpdated.Value.ToZulu()
      };
      //ResourceStore.DateTimeIndexList = new List<IndexDateTime>() { new IndexDateTime() { Low = DateTime.Now, High = DateTime.Now, ResourceStore = ResourceStore, SearchParameterId = DateTimeList[0].SearchParameterId } };
      IMapper.Map(x, ResourceStore);

      IResourceStoreRepository.Add(ResourceStore);
      await IResourceStoreRepository.SaveChangesAsync();

      var OutCome = new FhirApiResult(System.Net.HttpStatusCode.Created, query.FhirVersion)
      {
        ResourceId = UpdateResource.ResourceId,
        FhirResource = UpdatedFhirResource,
        VersionId = UpdateResource.VersionId
      };

      return OutCome;
    }
  }
}
