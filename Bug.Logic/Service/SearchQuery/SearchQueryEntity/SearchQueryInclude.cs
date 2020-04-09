using Bug.Common.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bug.Logic.Service.SearchQuery.SearchQueryEntity
{
  public class SearchQueryInclude
  {
    public SearchQueryInclude(IncludeType IncludeType)
    {
      this.Type = IncludeType;
      this.SearchParameterList = new List<DomainModel.SearchParameter>();
      this.IsIterate = false;
      this.IsRecurse = false;
    }

    public static string IterateName => "iterate";    
    public IncludeType Type { get; private set; }    
    public ResourceType? SourceResourceType { get; set; }
    public List<DomainModel.SearchParameter> SearchParameterList { get; set; }
    public ResourceType? SearchParameterTargetResourceType { get; set; }
    
    public bool IsRecurse { get; set; }
    public bool IsIterate { get; set; }
    public bool IsRecurseIterate
    {
      get
      {
        return (IsIterate || IsRecurse);
      }
    }
  }
}
