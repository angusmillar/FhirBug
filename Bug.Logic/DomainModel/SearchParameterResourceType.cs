using System;
using System.Collections.Generic;
using System.Text;

namespace Bug.Logic.DomainModel
{
  #pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
  public class SearchParameterResourceType : BaseIntKey
  {
    public int FkSearchParameterId { get; set; }
    public SearchParameter SearchParameter { get; set; }    
    public Bug.Common.Enums.ResourceType FkResourceTypeId { get; set; }    
    public ResourceType ResourceType { get; set; }
  }
}
