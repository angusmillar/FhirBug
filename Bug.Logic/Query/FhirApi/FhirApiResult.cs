using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Bug.Common.Enums;
using Bug.Common.FhirTools;
using Bug.Logic.Interfaces.Transaction;
using Microsoft.Extensions.Primitives;

namespace Bug.Logic.Query.FhirApi
{
  public class FhirApiResult
  {
    public FhirApiResult(HttpStatusCode HttpStatusCode, FhirVersion FhirVersion, string CorrelationId)
    {
      this.HttpStatusCode = HttpStatusCode;
      this.FhirVersion = FhirVersion;
      this.CorrelationId = CorrelationId;
      this.Headers = new Dictionary<string, StringValues>();    
    }
    public HttpStatusCode HttpStatusCode { get; set; }
    public FhirVersion FhirVersion { get; set; }
    public string? ResourceId { get; set; }
    public int? VersionId { get; set; }    
    public FhirResource? FhirResource { get; set; }
    public Dictionary<string, StringValues> Headers { get; set; }
    public string CorrelationId { get; set; }
    
  }
}
