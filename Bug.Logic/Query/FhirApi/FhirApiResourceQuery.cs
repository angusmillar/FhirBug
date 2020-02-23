using Bug.Common.FhirTools;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bug.Logic.Query.FhirApi
{
  public abstract class FhirApiResourceQuery : FhirApiQuery
  {
    public FhirResource? FhirResource { get; set; }
    public string? RequestResourceName { get; set; }
    public string? ResourceId { get; set; }
    public string? VersionId { get; set; }
    public DateTimeOffset LastUpdated { get; set; }

  }
}
