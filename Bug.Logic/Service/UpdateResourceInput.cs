using Bug.Common.Enums;
using Bug.Common.FhirTools;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bug.Logic.Service
{
  public class UpdateResource
  {
    public FhirResource? FhirResource { get; set; }
    public string? ResourceId { get; set; } 
    public string? VersionId { get; set; }
    public DateTime? LastUpdated { get; set; }

  }
}
