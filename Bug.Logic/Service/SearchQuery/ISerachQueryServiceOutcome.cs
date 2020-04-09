using Bug.Common.Enums;
using Bug.Common.FhirTools;
using Bug.Logic.Service.SearchQuery.SearchQueryEntity;
using Bug.Logic.Service.SearchQuery.Tools;
using System.Collections.Generic;

namespace Bug.Logic.Service.SearchQuery
{
  public interface ISerachQueryServiceOutcome
  {
    ContainedSearch? Contained { get; }
    ContainedType? ContainedType { get; }
    int? CountRequested { get; }
    int? PageRequested { get; }
    string? Content { get; }
    string? Text { get; }
    string? Filter { get; }
    string? Query { get; }
    SummaryType? SummaryType { get; }
    IList<SearchQueryInclude> IncludeList { get; }
    IList<InvalidSearchQueryParameter> InvalidSearchQueryList { get; }       
    ResourceType ResourceContext { get; }
    IList<ISearchQueryBase> SearchQueryList { get; }

  }
}