using System;
using System.Collections.Generic;
using System.Text;

namespace Bug.Common.Dto.Indexing
{
  public class ServiceBaseUrl 
  {
    public ServiceBaseUrl()
    {
      this.IsPrimary = false;
    }
    public string? Url { get; set; }
    public bool IsPrimary { get; set; }
  }
}
