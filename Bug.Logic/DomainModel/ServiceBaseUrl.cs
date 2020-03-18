using System;
using System.Collections.Generic;
using System.Text;

namespace Bug.Logic.DomainModel
{
  public class ServiceBaseUrl : BaseIntKey
  {
    public string Url { get; set; }
    public bool IsPrimary { get; set; }
  }
}
