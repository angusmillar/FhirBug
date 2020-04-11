using Bug.Common.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bug.Logic.Service.SearchQuery.SearchQueryEntity
{
  public class SearchQueryHas
  {
    public SearchQueryHas()
    {
    }

    
    public SearchQueryHas? ChildSearchQueryHas { get; set; }
    public ResourceType TargetResourceForSearchQuery { get; set; }
    public Bug.Logic.DomainModel.SearchParameter BackReferenceSearchParameter { get; set; }
    public ISearchQueryBase SearchQuery { get; set; }    
  }
}
