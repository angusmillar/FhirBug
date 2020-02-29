using Bug.Common.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bug.Common.ApplicationConfig
{
  public class FhirServerConfig : IFhirServerConfig
  {
    public FhirFormatType DefaultFhirFormat { get; set; } = FhirFormatType.json;
    public Uri ServiceBaseUrl { get; set; } = default!;
    public int CahceSlidingExpirationMinites { get; set; } = 5;

  }
}
