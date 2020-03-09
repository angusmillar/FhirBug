using System;
using System.Collections.Generic;
using System.Text;

namespace Bug.Logic.DomainModel
{
  #pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
  public class SearchParameterResourceName : BaseIntKey
  {
    public int FkSearchParameterId { get; set; }
    public SearchParameter SearchParameter { get; set; }    
    public int FkResourceNameId { get; set; }    
    public ResourceName ResourceName { get; set; }
  }
}
