using Bug.Logic.Query.FhirApi.Create;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bug.Logic.Query.FhirApi
{
  public abstract class FhirApiResourceQuery : FhirApiQuery
  {
    public object Resource { get; set; }

    public FhirResource FhirResource { get; set; }
    public string RequestResourceName { get; set; }
    public string FhirId { get; set; }
    public string VersionId { get; set; }
    public DateTimeOffset LastUpdated { get; set; }

  }
}
