using Bug.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bug.Logic.Service.SearchQuery.SearchQueryEntity
{
  public class SearchQueryReferenceValue : SearchQueryValueBase
  {
    public SearchQueryReferenceValue(bool IsMissing, IFhirUri? FhirUri)
      :base(IsMissing)
    {      
      this.FhirUri = FhirUri;
    }

    public IFhirUri? FhirUri { get; set; }
  }
}
