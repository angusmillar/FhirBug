using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Bug.Logic.DomainModel
{
  public class HttpStatusCode : BaseIntKey
  {
    public System.Net.HttpStatusCode Number { get; set; }
    public string Code { get; set; }
  }
}
