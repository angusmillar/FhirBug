using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Bug.Common.Enums;
using Bug.Common.FhirTools;

namespace Bug.Logic.Query.FhirApi
{
  public class FhirApiResult
  {
    public string? ResourceId { get; set; }
    public string? VersionId { get; set; }
    public HttpStatusCode? HttpStatusCode { get; set;  }    
    public FhirResource? FhirResource { get; set; }
    public FhirMajorVersion? FhirMajorVersion { get; set; }
        
  }
}
