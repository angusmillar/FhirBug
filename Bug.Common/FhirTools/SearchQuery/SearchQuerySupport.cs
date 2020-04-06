using System;
using System.Collections.Generic;
using System.Text;
using Bug.Common.Enums;

namespace Bug.Common.FhirTools.SearchQuery
{
  public static class SearchQuerySupport
  {
    public static string[] GetPrefixListForSearchType(SearchParamType SearchParamType)
    {
      var ReturnList = new List<string>();
      switch (SearchParamType)
      {
        case SearchParamType.Number:
          return new string[] {
            SearchComparator.Ne.GetCode(),
            SearchComparator.Eq.GetCode(),
            SearchComparator.Gt.GetCode(),
            SearchComparator.Ge.GetCode(),
            SearchComparator.Lt.GetCode(),
            SearchComparator.Le.GetCode()
          };               
        case SearchParamType.Date:
          return new string[] {
            SearchComparator.Ne.GetCode(),
            SearchComparator.Eq.GetCode(),
            SearchComparator.Gt.GetCode(),
            SearchComparator.Ge.GetCode(),
            SearchComparator.Lt.GetCode(),
            SearchComparator.Le.GetCode()
          };          
        case SearchParamType.String:
          //Any search parameter that's value is a string will not have prefixes
          return new string[] { };
        case SearchParamType.Token:
          //Any search parameter that's value is a string will not have prefixes
          return new string[] { };
        case SearchParamType.Reference:
          //Any search parameter that's value is a string will not have prefixes
          return new string[] { };
        case SearchParamType.Composite:
          //Any search parameter that's value is a string will not have prefixes
          return new string[] { };
        case SearchParamType.Quantity:
          return new string[] {
            SearchComparator.Ne.GetCode(),
            SearchComparator.Eq.GetCode(),
            SearchComparator.Gt.GetCode(),
            SearchComparator.Ge.GetCode(),
            SearchComparator.Lt.GetCode(),
            SearchComparator.Le.GetCode()
          };
        case SearchParamType.Uri:
          //Any search parameter that's value is a string will not have prefixes
          return new string[] { };
        default:
          throw new System.ComponentModel.InvalidEnumArgumentException(SearchParamType.GetCode(), (int)SearchParamType, typeof(SearchParamType));
      }
    }

    public static SearchModifierCode[] GetModifiersForSearchType(SearchParamType SearchParamType)
    {
      IList<SearchModifierCode> ReturnList = new List<SearchModifierCode>();
      switch (SearchParamType)
      {
        case SearchParamType.Number:
          return new SearchModifierCode[] { SearchModifierCode.Missing };                    
        case SearchParamType.Date:
          return new SearchModifierCode[] { SearchModifierCode.Missing };          
        case SearchParamType.String:
          return new SearchModifierCode[]
          {
            SearchModifierCode.Missing,
            SearchModifierCode.Contains,
            SearchModifierCode.Exact
          };
        case SearchParamType.Token:
          return new SearchModifierCode[] { SearchModifierCode.Missing };
          //The modifiers below are supported in the spec for token but not 
          //implemented by this server as yet

          //ReturnList.Add(Conformance.SearchModifierCode.Text.ToString());
          //ReturnList.Add(Conformance.SearchModifierCode.In.ToString());
          //ReturnList.Add(Conformance.SearchModifierCode.Below.ToString());
          //ReturnList.Add(Conformance.SearchModifierCode.Above.ToString());
          //ReturnList.Add(Conformance.SearchModifierCode.In.ToString());
          //ReturnList.Add(Conformance.SearchModifierCode.NotIn.ToString());          
        case SearchParamType.Reference:
          return new SearchModifierCode[]
          {
            SearchModifierCode.Missing,
            SearchModifierCode.Type,            
          };          
        case SearchParamType.Composite:
          return new SearchModifierCode[] { };
        case SearchParamType.Quantity:
          return new SearchModifierCode[] { SearchModifierCode.Missing };          
        case SearchParamType.Uri:
          return new SearchModifierCode[]
          {
            SearchModifierCode.Missing,
            SearchModifierCode.Below,
            SearchModifierCode.Above,
            SearchModifierCode.Contains,
            SearchModifierCode.Exact
          };          
        default:
          throw new System.ComponentModel.InvalidEnumArgumentException(SearchParamType.ToString(), (int)SearchParamType, typeof(SearchParamType));
      }
    }
  }
}
