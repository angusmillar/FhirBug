using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Bug.Logic.DomainModel
{
  #pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
  public class HttpStatusCode : BaseIntKey
  {
    public System.Net.HttpStatusCode Number { get; set; }
    public string Code { get; set; }
  }
}
