using Bug.Common.Enums;
using Bug.Common.FhirTools;
using Bug.Logic.Service.Fhir;
using Bug.Logic.Service.SearchQuery.SearchQueryEntity;
using Bug.Logic.Service.SearchQuery.Tools;
using System.Collections.Generic;

namespace Bug.Logic.Service.SearchQuery
{
  public interface ISerachQueryServiceOutcome
  {
    bool HasInvalidQuery { get; }
    bool HasUnsupportedQuery { get; }
    FhirVersion FhirVersion { get; set; }
    ContainedSearch? Contained { get; }
    ContainedType? ContainedType { get; }
    int? CountRequested { get; }
    int? PageRequested { get; }
    string? Content { get; }
    string? Text { get; }
    string? Filter { get; }
    string? Query { get; }
    SummaryType? SummaryType { get; }
    IList<SearchQueryHas> HasList { get; set; }
    IList<SearchQueryInclude> IncludeList { get; }
    IList<InvalidSearchQueryParameter> InvalidSearchQueryList { get; }
    IList<Tools.InvalidSearchQueryParameter> UnsupportedSearchQueryList { get; set; }
    ResourceType ResourceContext { get; }
    IList<ISearchQueryBase> SearchQueryList { get; }
    FhirResource InvalidQueryOperationOutCome(IOperationOutcomeSupport IOperationOutcomeSupport);
    FhirResource InvalidAndUnsupportedQueryOperationOutCome(IOperationOutcomeSupport IOperationOutcomeSupport);

  }
}