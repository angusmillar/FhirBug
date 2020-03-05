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
    public FhirApiResult(HttpStatusCode HttpStatusCode, FhirVersion FhirVersion)
    {
      this.HttpStatusCode = HttpStatusCode;
      this.FhirVersion = FhirVersion;
    }
    public HttpStatusCode HttpStatusCode { get; set; }
    public FhirVersion FhirVersion { get; set; }
    public string? ResourceId { get; set; }
    public int? VersionId { get; set; }    
    public FhirResource? FhirResource { get; set; }
    
        
  }
}
