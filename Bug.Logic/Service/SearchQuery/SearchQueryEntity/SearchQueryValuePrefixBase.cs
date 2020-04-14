using Bug.Common.Enums;
using Bug.Common.FhirTools.SearchQuery;
using Bug.Logic.DomainModel;
using System;
using System.Text.RegularExpressions;

namespace Bug.Logic.Service.SearchQuery.SearchQueryEntity
{
  public abstract class SearchQueryValuePrefixBase : SearchQueryValueBase
  {
    protected SearchQueryValuePrefixBase(bool IsMissing, SearchComparator? Prefix)
      :base(IsMissing)
    {
      this.Prefix = Prefix;
    }

    public SearchComparator? Prefix { get; set; }
    public static bool ValidatePreFix(Bug.Common.Enums.SearchParamType SearchParamType, SearchComparator? Prefix)
    {
      if (Prefix.HasValue)
      {
        return Array.Exists(SearchQuerySupport.GetPrefixListForSearchType(SearchParamType), item => item == Prefix.Value);
      }
      else
      {
        return true;
      }
    }

    public static SearchComparator? GetPrefix(string value)
    {
      if (value.Length > 2)
      {
        //Are the first two char Alpha characters 
        if (Regex.IsMatch(value.Substring(0, 2), @"^[a-zA-Z]+$"))
        {
          var SearchPrefixTypeDictionary = StringToEnumMap<SearchComparator>.GetDictionary();
          if (SearchPrefixTypeDictionary.ContainsKey(value.Substring(0, 2)))
          {
            return SearchPrefixTypeDictionary[value.Substring(0, 2)];
          }
        }
      }
      return null;
    }

    public static string RemovePrefix(string value, SearchComparator? prefix)
    {
      if (!prefix.HasValue)
      {
        return value;
      }
      else
      {
        if (value.Length > 2)
        {
          return value.Substring(prefix.GetCode().Length, value.Length - prefix.GetCode().Length);
        }
        else
        {
          throw new ArgumentException($"Attempt to remove the prefix {prefix.GetCode()} from the value {value} failed as the value is shorter than the prefix being removed");
        }
      }      
    }
    
  }
}
