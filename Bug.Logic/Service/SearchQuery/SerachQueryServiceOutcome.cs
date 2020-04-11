using Bug.Common.Enums;
using Bug.Common.FhirTools;
using Bug.Logic.Service.Fhir;
using Bug.Logic.Service.SearchQuery.SearchQueryEntity;
using Bug.Logic.Service.SearchQuery.Tools;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bug.Logic.Service.SearchQuery
{
  public class SerachQueryServiceOutcome : ISerachQueryServiceOutcome
  {
    public SerachQueryServiceOutcome(FhirVersion FhirVersion,  ResourceType ResourceContext, IFhirSearchQuery IFhirSearchQuery)
    {
      this.FhirVersion = FhirVersion;
      this.ResourceContext = ResourceContext;
      this.CountRequested = IFhirSearchQuery.Count;
      this.PageRequested = IFhirSearchQuery.Page;
      this.SummaryType = IFhirSearchQuery.SummaryType;
      this.Contained = IFhirSearchQuery.Contained;
      this.ContainedType = IFhirSearchQuery.ContainedType;
      
      this.Text = IFhirSearchQuery.Text;
      this.Content = IFhirSearchQuery.Content;
      this.Filter = IFhirSearchQuery.Filter;
      this.Query = IFhirSearchQuery.Query;

      this.InvalidSearchQueryList = IFhirSearchQuery.InvalidParameterList;
      this.UnsupportedSearchQueryList = new List<InvalidSearchQueryParameter>();
      this.IncludeList = new List<SearchQueryInclude>();
      this.SearchQueryList = new List<ISearchQueryBase>();
      this.HasList = new List<SearchQueryHas>();
    }
    public FhirVersion FhirVersion { get; set; }     
    public bool HasInvalidQuery
    {
      get
      {
        return (this.InvalidSearchQueryList.Count > 0);
      }
    }
    public bool HasUnsupportedQuery
    {
      get
      {
        return (this.UnsupportedSearchQueryList.Count > 0);
      }
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
    public IList<SearchQueryHas> HasList { get; set; }
    public IList<ISearchQueryBase> SearchQueryList { get; set; }
    public IList<SearchQueryInclude> IncludeList { get; set; }
    public IList<Tools.InvalidSearchQueryParameter> InvalidSearchQueryList { get; set; }
    public IList<Tools.InvalidSearchQueryParameter> UnsupportedSearchQueryList { get; set; }
    public FhirResource InvalidQueryOperationOutCome(IOperationOutcomeSupport IOperationOutcomeSupport)
    {
      var MessageList = new List<string>();
      foreach (var Invalid in this.InvalidSearchQueryList)
      {
        MessageList.Add($"Query parameter: {Invalid.Name}={Invalid.Value} was invalid for the following reason: {Invalid.ErrorMessage} ");
      }
      return IOperationOutcomeSupport.GetError(this.FhirVersion, MessageList.ToArray());
    }

    public FhirResource InvalidAndUnsupportedQueryOperationOutCome(IOperationOutcomeSupport IOperationOutcomeSupport)
    {
      var MessageList = new List<string>();
      foreach(var Invalid in this.InvalidSearchQueryList)
      {        
        MessageList.Add($"Invalid query parameter: {Invalid.Name}={Invalid.Value} was invalid for the following reason: {Invalid.ErrorMessage} ");
      }
      foreach (var Invalid in this.UnsupportedSearchQueryList)
      {
        MessageList.Add($"Unsupported query parameter: {Invalid.Name}={Invalid.Value} was unsupported for the following reason: {Invalid.ErrorMessage} ");
      }
      return IOperationOutcomeSupport.GetError(this.FhirVersion, MessageList.ToArray());
    }
    
  }
}
