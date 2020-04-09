using Bug.Common.Enums;
using Microsoft.Extensions.Primitives;
using System.Collections.Generic;
using static Bug.Logic.Service.SearchQuery.Tools.FhirSearchQuery;

namespace Bug.Logic.Service.SearchQuery.Tools
{
  public interface IFhirSearchQuery
  {
    ContainedSearch? Contained { get; }
    ContainedType? ContainedType { get;}
    string? Content { get; }
    int? Page { get; set; }
    int? Count { get; }
    string? Filter { get; }
    IList<IncludeParameter> Include { get; }
    IList<IncludeParameter> RevInclude { get; }
    IList<InvalidSearchQueryParameter> InvalidParameterList { get; }
    Dictionary<string, StringValues> ParametersDictionary { get; }
    string? Query { get; }    
    IList<FhirSearchQuery.SortParameter> Sort { get; }
    SummaryType? SummaryType { get; }
    string? Text { get; }
    /// <summary>
    /// If false then the InvalidParameterList property will have at least one error.
    /// </summary>
    /// <param name="query"></param>
    /// <returns></returns>
    bool Parse(Dictionary<string, StringValues> query);
  }
}