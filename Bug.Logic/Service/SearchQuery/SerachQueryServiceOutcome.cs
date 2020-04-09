using Bug.Common.Enums;
using Bug.Common.FhirTools;
using Bug.Logic.Service.SearchQuery.SearchQueryEntity;
using Bug.Logic.Service.SearchQuery.Tools;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bug.Logic.Service.SearchQuery
{
  public class SerachQueryServiceOutcome : ISerachQueryServiceOutcome
  {
    public SerachQueryServiceOutcome(ResourceType ResourceContext, IFhirSearchQuery IFhirSearchQuery)
    {
      this.ResourceContext = ResourceContext;
      this.CountRequested = IFhirSearchQuery.Count;
      this.PageRequested = IFhirSearchQuery.Page;
      this.SummaryType = IFhirSearchQuery.SummaryType;
      this.Contained = IFhirSearchQuery.Contained;
      this.ContainedType = IFhirSearchQuery.ContainedType;
      this.InvalidSearchQueryList = IFhirSearchQuery.InvalidParameterList;
      this.Text = IFhirSearchQuery.Text;
      this.Content = IFhirSearchQuery.Content;
      this.Filter = IFhirSearchQuery.Filter;
      this.Query = IFhirSearchQuery.Query;

      this.IncludeList = new List<SearchQueryInclude>();
      this.SearchQueryList = new List<ISearchQueryBase>();
    }

    public ResourceType ResourceContext { get; set; }
    public int? CountRequested { get; set; }
    public int? PageRequested { get; set; }
    public string? Content { get; set; }
    public string? Text { get; set; }
    public string? Filter { get; set; }
    public string? Query { get; set; }
    public ContainedSearch? Contained { get; set; }
    public ContainedType? ContainedType { get; set; }
    public SummaryType? SummaryType { get; set; }
    public IList<ISearchQueryBase> SearchQueryList { get; set; }
    public IList<SearchQueryInclude> IncludeList { get; set; }
    public IList<Tools.InvalidSearchQueryParameter> InvalidSearchQueryList { get; set; }
    
  }
}
