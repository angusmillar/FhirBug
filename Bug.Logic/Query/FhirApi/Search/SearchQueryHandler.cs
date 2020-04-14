using Bug.Common.Compression;
using Bug.Common.DateTimeTools;
using Bug.Common.Exceptions;
using Bug.Common.FhirTools;
using Bug.Common.FhirTools.Bundle;
using Bug.Logic.DomainModel;
using Bug.Logic.Interfaces.Repository;
using Bug.Logic.Service.Fhir;
using Bug.Logic.Service.Headers;
using Bug.Logic.Service.SearchQuery;
using Bug.Logic.Service.Serach;
using Bug.Logic.Service.ValidatorService;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bug.Logic.Query.FhirApi.Search
{
  public class SearchQueryHandler : IQueryHandler<SearchQuery, FhirApiResult>
  {
    private readonly IValidateQueryService IValidateQueryService;
    private readonly IResourceTypeSupport IResourceTypeSupport;
    private readonly ISearchQueryService ISearchQueryService;
    private readonly IOperationOutcomeSupport IOperationOutcomeSupport;
    private readonly ISearchService ISearchService;
    private readonly IFhirResourceBundleSupport IFhirResourceBundleSupport;
    private readonly ISearchBundleService ISearchBundleService;

    public SearchQueryHandler(
      IValidateQueryService IValidateQueryService,
      IResourceTypeSupport IResourceTypeSupport,
      ISearchQueryService ISearchQueryService,
      IOperationOutcomeSupport IOperationOutcomeSupport,
      ISearchService ISearchService,
      ISearchBundleService ISearchBundleService,
      IFhirResourceBundleSupport IFhirResourceBundleSupport)
    {
      this.IValidateQueryService = IValidateQueryService;
      this.IResourceTypeSupport = IResourceTypeSupport;
      this.ISearchQueryService = ISearchQueryService;
      this.IOperationOutcomeSupport = IOperationOutcomeSupport;
      this.ISearchService = ISearchService;
      this.ISearchBundleService = ISearchBundleService;
      this.IFhirResourceBundleSupport = IFhirResourceBundleSupport;
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


      ISerachQueryServiceOutcome SerachQueryServiceOutcome = await ISearchQueryService.Process(query.FhirVersion, ResourceType.Value, query.RequestQuery);
      if (query.Headers.PreferHandling == Common.Enums.PreferHandlingType.Strict && (SerachQueryServiceOutcome.HasInvalidQuery || SerachQueryServiceOutcome.HasUnsupportedQuery))
      {
        return new FhirApiResult(System.Net.HttpStatusCode.Forbidden, SerachQueryServiceOutcome.FhirVersion, query.CorrelationId)
        {
          ResourceId = null,
          FhirResource = SerachQueryServiceOutcome.InvalidAndUnsupportedQueryOperationOutCome(IOperationOutcomeSupport),
          VersionId = null
        };
      }
      else if (SerachQueryServiceOutcome.HasInvalidQuery)
      {
        return new FhirApiResult(System.Net.HttpStatusCode.Forbidden, SerachQueryServiceOutcome.FhirVersion, query.CorrelationId)
        {
          ResourceId = null,
          FhirResource = SerachQueryServiceOutcome.InvalidQueryOperationOutCome(IOperationOutcomeSupport),
          VersionId = null
        };
      }

      IList<ResourceStore> ResourceStoreList = await ISearchService.Process(SerachQueryServiceOutcome);

      BundleModel Bundle = await ISearchBundleService.GetSearchBundleModel(ResourceStoreList);

      return new FhirApiResult(System.Net.HttpStatusCode.OK, query.FhirVersion, query.CorrelationId)
      {
        FhirResource = IFhirResourceBundleSupport.GetFhirResource(query.FhirVersion, Bundle)
      };



    }
  }
}
