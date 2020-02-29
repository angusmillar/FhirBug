using Bug.Common.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bug.Logic.DomainModel
{
  #pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
  public class Method : ModelBase
  {
    public string Code { get; set; }
    public HttpVerb HttpVerb { get; set; }
  }
}
