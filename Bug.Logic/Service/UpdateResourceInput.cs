using Bug.Common.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bug.Logic.Service
{
  public class UpdateResource
  {
    public FhirMajorVersion FhirMajorVersion { get; set; }
    public object Resource { get; set; }
    public string ResourceId { get; set; }
    public string VersionId { get; set; }
    public DateTime LastUpdated { get; set; }

  }
}
