using System;
using System.Collections.Generic;
using System.Text;

namespace Bug.Logic.DomainModel
{
  public class SearchParameterComponent : BaseIntKey
  {
   #pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
    public int FkSearchParameterId { get; set; }
    public SearchParameter SearchParameter { get; set; }
    public string Definition { get; set; }
    public string Expression { get; set; }
  }
}
