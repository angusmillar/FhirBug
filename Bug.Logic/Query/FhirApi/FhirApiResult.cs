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
    public FhirApiResult(HttpStatusCode HttpStatusCode, FhirMajorVersion FhirMajorVersion)
    {
      this.HttpStatusCode = HttpStatusCode;
      this.FhirMajorVersion = FhirMajorVersion;
    }
    public HttpStatusCode HttpStatusCode { get; set; }
    public FhirMajorVersion FhirMajorVersion { get; set; }
    public string? ResourceId { get; set; }
    public int? VersionId { get; set; }    
    public FhirResource? FhirResource { get; set; }
    
        
  }
}
