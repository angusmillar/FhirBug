using Bug.Common.Enums;
using Microsoft.Extensions.Primitives;
using System.Collections.Generic;

namespace Bug.Logic.Service.SearchQuery.Tools
{
  public interface IFhirSearchQuery
  {
    ContainedSearch? Contained { get; }
    ContainedType? ContainedType { get;}
    string? Content { get; }
    int? Count { get; }
    string? Filter { get; }
    IList<string> Include { get; }
    IList<InvalidSearchQueryParameter> InvalidParameterList { get; }
    Dictionary<string, StringValues> ParametersDictionary { get; }
    string? Query { get; }
    IList<string> RevInclude { get; }
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