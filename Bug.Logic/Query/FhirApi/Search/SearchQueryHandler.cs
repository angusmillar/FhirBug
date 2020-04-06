﻿using Bug.Common.Compression;
using Bug.Common.DateTimeTools;
using Bug.Common.Exceptions;
using Bug.Common.FhirTools;
using Bug.Logic.DomainModel;
using Bug.Logic.Interfaces.Repository;
using Bug.Logic.Service.Fhir;
using Bug.Logic.Service.Headers;
using Bug.Logic.Service.SearchQuery;
using Bug.Logic.Service.ValidatorService;
using System.Threading.Tasks;

namespace Bug.Logic.Query.FhirApi.Search
{
  public class SearchQueryHandler : IQueryHandler<SearchQuery, FhirApiResult>
  {
    private readonly IValidateQueryService IValidateQueryService;
    private readonly IResourceStoreRepository IResourceStoreRepository;
    private readonly IFhirResourceParseJsonService IFhirResourceParseJsonService;
    private readonly IResourceTypeSupport IResourceTypeSupport;
    private readonly IGZipper IGZipper;
    private readonly IServerDateTimeSupport IServerDateTimeSupport;
    private readonly IHeaderService IHeaderService;
    private readonly ISearchQueryService ISearchQueryService;

    public SearchQueryHandler(
      IValidateQueryService IValidateQueryService,
      IResourceStoreRepository IResourceStoreRepository,
      IFhirResourceParseJsonService IFhirResourceParseJsonService,
      IResourceTypeSupport IResourceTypeSupport,
      IGZipper IGZipper,
      IServerDateTimeSupport IServerDateTimeSupport,
      IHeaderService IHeaderService,
      ISearchQueryService ISearchQueryService)
    {
      this.IValidateQueryService = IValidateQueryService;
      this.IResourceStoreRepository = IResourceStoreRepository;
      this.IFhirResourceParseJsonService = IFhirResourceParseJsonService;
      this.IResourceTypeSupport = IResourceTypeSupport;
      this.IGZipper = IGZipper;
      this.IServerDateTimeSupport = IServerDateTimeSupport;
      this.IHeaderService = IHeaderService;
      this.ISearchQueryService = ISearchQueryService;
    }

    public async Task<FhirApiResult> Handle(SearchQuery query)
    {

      if (!IValidateQueryService.IsValid(query, out Common.FhirTools.FhirResource? IsNotValidOperationOutCome))
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


      SerachQueryServiceOutcome SerachQueryServiceOutcome = await ISearchQueryService.Process(query.FhirVersion, ResourceType.Value, query.RequestQuery);


    

      
      throw new System.NotImplementedException();

    }
  }
}