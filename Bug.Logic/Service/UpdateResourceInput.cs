using Bug.Common.Enums;
using Bug.Common.FhirTools;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bug.Logic.Service
{
  public class UpdateResource
  {
    public UpdateResource(FhirResource FhirResource)
    {
      this.FhirResource = FhirResource;
    }

    public FhirResource FhirResource { get; set; }
    public string? ResourceId { get; set; } 
    public int? VersionId { get; set; }
    public DateTime? LastUpdated { get; set; }

  }
}
