using System;
using System.Collections.Generic;
using System.Text;

#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
namespace Bug.Logic.DomainModel
{  
  public class SearchParameterResourceType : BaseIntKey
  {
    public int FkSearchParameterId { get; set; }
    public SearchParameter SearchParameter { get; set; }    
    public Bug.Common.Enums.ResourceType FkResourceTypeId { get; set; }    
    public ResourceType ResourceType { get; set; }
  }
}
