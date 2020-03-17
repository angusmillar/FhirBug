using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
namespace Bug.Logic.DomainModel
{  
  public class HttpStatusCode : BaseIntKey
  {
    public System.Net.HttpStatusCode Number { get; set; }
    public string Code { get; set; }
  }
}
