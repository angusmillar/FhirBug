using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Bug.Common.Enums;

namespace Bug.Logic.Command.FhirApi
{
  public class FhirApiOutcome
  {
    public string ResourceId { get; set; }
    public string ResourceVersionId { get; set; }
    public HttpStatusCode HttpStatusCode { get; set;  }
    public object Resource { get; set; }    
    public FhirMajorVersion FhirMajorVersion { get; set; }
        
  }
}
