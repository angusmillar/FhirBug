using System;
using System.Collections.Generic;
using System.Text;

namespace Bug.Logic.DomainModel
{
  public class FhirVersion : ModelBase
  {
    public string? VersionDescription { get; set; }
    public Bug.Common.Enums.FhirMajorVersion FhirMajorVersion { get; set; }

  }
}
