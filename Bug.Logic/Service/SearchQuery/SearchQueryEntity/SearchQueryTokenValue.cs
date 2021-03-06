﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Bug.Logic.Service.SearchQuery.SearchQueryEntity
{
  public class SearchQueryTokenValue : SearchQueryValueBase
  {
    public SearchQueryTokenValue(bool IsMissing, TokenSearchType? searchType, string? code, string? system)
      :base(IsMissing)
    {      
      this.SearchType = searchType;
      this.Code = code;
      this.System = system;
    }

    public enum TokenSearchType
    {      
      /// <summary>
      /// [parameter]=[code]: the value of [code] matches a Coding.code or 
      /// Identifier.value irrespective of the value of the system property
      /// </summary>
      MatchCodeOnly,
      /// <summary>
      /// [parameter]=[system]|: any element where the value of [system] 
      /// matches the system property of the Identifier or Coding
      /// </summary>
      MatchSystemOnly,
      /// <summary>
      /// [parameter]=[system]|[code]: the value of [code] matches a Coding.code 
      /// or Identifier.value, and the value of [system] matches the system property 
      /// of the Identifier or Coding
      /// </summary>
      MatchCodeAndSystem,
      /// <summary>
      /// [parameter]=|[code]: the value of [code] matches a Coding.code or 
      /// Identifier.value, and the Coding/Identifier has no system property
      /// </summary>
      MatchCodeWithNullSystem
    };

    public TokenSearchType? SearchType { get; set; }
    public string? Code { get; set; }
    public string? System { get; set; }
  }
}
