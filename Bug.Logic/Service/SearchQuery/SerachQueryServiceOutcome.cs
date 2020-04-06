using Bug.Common.Enums;
using Bug.Common.FhirTools;
using Bug.Logic.Service.SearchQuery.SearchQueryEntity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bug.Logic.Service.SearchQuery
{  
  public class SerachQueryServiceOutcome
  {
    public SerachQueryServiceOutcome(ResourceType ResourceContext, int? CountRequested, SummaryType? SummaryType)
    {
      this.ResourceContext = ResourceContext;
      this.CountRequested = CountRequested;
      this.SummaryType = SummaryType;
      this.SearchQueryList = new List<ISearchQueryBase>();
      this.InvalidSearchQueryList = new List<Tools.InvalidSearchQueryParameter>();
    }

    private ResourceType ResourceContext { get; set; }
    private int? CountRequested { get; set; }
    SummaryType? SummaryType { get; }
    public IList<ISearchQueryBase> SearchQueryList { get; set; }
    public IList<Tools.InvalidSearchQueryParameter> InvalidSearchQueryList { get; set; }
    public Common.FhirTools.FhirResource? OperationOutcome { get; set; }  
  }
}
