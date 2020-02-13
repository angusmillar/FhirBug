using System;
using System.Collections.Generic;
using System.Text;

namespace Bug.Logic.Command.FhirApi
{
  public abstract class FhirApiResourceCommand : FhirApiCommand
  {
    public object Resource { get; set; }
    public string FhirId { get; set; }
    public string VersionId { get; set; }
    public DateTimeOffset LastUpdated { get; set; }

  }
}
