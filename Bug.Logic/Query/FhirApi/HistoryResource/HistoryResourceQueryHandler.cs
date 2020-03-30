﻿using Bug.Common.Compression;
using Bug.Common.Enums;
using Bug.Common.Exceptions;
using Bug.Logic.DomainModel;
using Bug.Logic.Interfaces.Repository;
using Bug.Logic.Service;
using Bug.Logic.Service.ValidatorService;
using System.Collections.Generic;
using System.Threading.Tasks;
using Bug.Common.FhirTools.Bundle;
using Bug.Common.FhirTools;
using Bug.Common.DateTimeTools;
using Bug.Logic.Service.FhirResourceService;

namespace Bug.Logic.Query.FhirApi.HistoryResource
{
  public class HistoryResourceQueryHandler : IQueryHandler<HistoryResourceQuery, FhirApiResult>
  {
    private readonly IValidateQueryService IValidateQueryService;
    private readonly IResourceStoreRepository IResourceStoreRepository;    
    private readonly IFhirResourceBundleSupport IFhirResourceBundleSupport;
    private readonly IHistoryBundleService IHistoryBundleService;
    private readonly IResourceTypeSupport IResourceTypeSupport;

    public HistoryResourceQueryHandler(
      IValidateQueryService IValidateQueryService,
      IResourceStoreRepository IResourceStoreRepository,
      IFhirResourceBundleSupport IFhirResourceBundleSupport,
      IHistoryBundleService IHistoryBundleService,
      IResourceTypeSupport IResourceTypeSupport)
    {
      this.IValidateQueryService = IValidateQueryService;
      this.IResourceStoreRepository = IResourceStoreRepository;
      this.IFhirResourceBundleSupport = IFhirResourceBundleSupport;
      this.IHistoryBundleService = IHistoryBundleService;
      this.IResourceTypeSupport = IResourceTypeSupport;
    }

    public async Task<FhirApiResult> Handle(HistoryResourceQuery query)
    {

      if (!IValidateQueryService.IsValid(query, out FhirResource? IsNotValidOperationOutCome))
      {
        return new FhirApiResult(System.Net.HttpStatusCode.BadRequest, IsNotValidOperationOutCome!.FhirVersion, query.CorrelationId)
        {
          ResourceId = null,
          FhirResource = IsNotValidOperationOutCome,
          VersionId = null
        };
      }

      Bug.Common.Enums.ResourceType? ResourceType = IResourceTypeSupport.GetTypeFromName(query.ResourceName);
      if (!ResourceType.HasValue)
        throw new System.ArgumentNullException(nameof(ResourceType));

      IList<ResourceStore> ResourceStoreList = await IResourceStoreRepository.GetResourceHistoryListAsync(query.FhirVersion, ResourceType.Value);

      //Construct the History Bundle
      var BundleModel = IHistoryBundleService.GetHistoryBundleModel(ResourceStoreList);      

      return new FhirApiResult(System.Net.HttpStatusCode.OK, query.FhirVersion, query.CorrelationId)
      {
        FhirResource = IFhirResourceBundleSupport.GetFhirResource(query.FhirVersion, BundleModel)
      };

    }
  }
}
