using System;
using System.Collections.Generic;
using System.Text;
using Bug.Common.Enums;

namespace Bug.Common.FhirTools.SearchQuery
{
  public static class SearchQuerySupport
  {
    public static SearchComparator[] GetPrefixListForSearchType(SearchParamType SearchParamType)
    {
      return SearchParamType switch
      {
        SearchParamType.Number => new SearchComparator[] {
            SearchComparator.Ne,
            SearchComparator.Eq,
            SearchComparator.Gt,
            SearchComparator.Ge,
            SearchComparator.Lt,
            SearchComparator.Le
          },
        SearchParamType.Date => new SearchComparator[] {
            SearchComparator.Ne,
            SearchComparator.Eq,
            SearchComparator.Gt,
            SearchComparator.Ge,
            SearchComparator.Lt,
            SearchComparator.Le
          },
        SearchParamType.String => new SearchComparator[] { },//Any search parameter that's value is a string will not have prefixes
        SearchParamType.Token => new SearchComparator[] { },//Any search parameter that's value is a string will not have prefixes
        SearchParamType.Reference => new SearchComparator[] { },//Any search parameter that's value is a string will not have prefixes
        SearchParamType.Composite => new SearchComparator[] { },//Any search parameter that's value is a string will not have prefixes
        SearchParamType.Quantity => new SearchComparator[] {
            SearchComparator.Ne,
            SearchComparator.Eq,
            SearchComparator.Gt,
            SearchComparator.Ge,
            SearchComparator.Lt,
            SearchComparator.Le
          },
        SearchParamType.Uri => new SearchComparator[] { },//Any search parameter that's value is a string will not have prefixes
        SearchParamType.Special => new SearchComparator[] { },
        _ => throw new System.ComponentModel.InvalidEnumArgumentException(SearchParamType.GetCode(), (int)SearchParamType, typeof(SearchParamType)),
      };
    }

    public static SearchModifierCode[] GetModifiersForSearchType(SearchParamType SearchParamType)
    {
      return SearchParamType switch
      {
        SearchParamType.Number => new SearchModifierCode[] { SearchModifierCode.Missing },
        SearchParamType.Date => new SearchModifierCode[] { SearchModifierCode.Missing },
        SearchParamType.String => new SearchModifierCode[]
        {
            SearchModifierCode.Missing,
            SearchModifierCode.Contains,
            SearchModifierCode.Exact
        },
        SearchParamType.Token => new SearchModifierCode[] { SearchModifierCode.Missing },
        //The modifiers below are supported in the spec for token but not 
        //implemented by this server as yet
        //ReturnList.Add(Conformance.SearchModifierCode.Text.ToString());
        //ReturnList.Add(Conformance.SearchModifierCode.In.ToString());
        //ReturnList.Add(Conformance.SearchModifierCode.Below.ToString());
        //ReturnList.Add(Conformance.SearchModifierCode.Above.ToString());
        //ReturnList.Add(Conformance.SearchModifierCode.In.ToString());
        //ReturnList.Add(Conformance.SearchModifierCode.NotIn.ToString());          
        SearchParamType.Reference => new SearchModifierCode[]
        {
            SearchModifierCode.Missing,
            SearchModifierCode.Type,
        },
        SearchParamType.Composite => new SearchModifierCode[] { },
        SearchParamType.Quantity => new SearchModifierCode[] { SearchModifierCode.Missing },
        SearchParamType.Uri => new SearchModifierCode[]
        {
            SearchModifierCode.Missing,
            SearchModifierCode.Below,
            SearchModifierCode.Above,
            SearchModifierCode.Contains,
            SearchModifierCode.Exact
        },
        SearchParamType.Special => new SearchModifierCode[] { SearchModifierCode.Missing },
        _ => throw new System.ComponentModel.InvalidEnumArgumentException(SearchParamType.ToString(), (int)SearchParamType, typeof(SearchParamType)),
      };
    }
  }
}
