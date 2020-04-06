using Bug.Common.Enums;
using Bug.Common.FhirTools;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bug.Logic.Service.Fhir
{
  public class UpdateResource
  {
    public UpdateResource(Common.FhirTools.FhirResource FhirResource)
    {
      this.FhirResource = FhirResource;
    }

    public Common.FhirTools.FhirResource FhirResource { get; set; }
    public string? ResourceId { get; set; } 
    public int? VersionId { get; set; }
    public DateTimeOffset? LastUpdated { get; set; }

  }
}
