using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Bug.Common.Enums;

namespace Bug.Logic.Query.FhirApi
{
  public class FhirApiResult
  {
    public string ResourceId { get; set; }
    public string ResourceVersionId { get; set; }
    public HttpStatusCode HttpStatusCode { get; set;  }
    public object Resource { get; set; }    
    public FhirMajorVersion FhirMajorVersion { get; set; }
        
  }
}
