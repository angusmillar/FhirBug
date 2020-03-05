using Bug.Common.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bug.Common.ApplicationConfig
{
  public class FhirServerConfig : IFhirServerConfig, IServerDefaultTimeZoneTimeSpan
  {
    public FhirFormatType DefaultFhirFormat { get; set; } = FhirFormatType.json;
    public Uri ServiceBaseUrl { get; set; } = default!;
    public TimeSpan ServerDefaultTimeZoneTimeSpan { get; set; }
    public int CahceSlidingExpirationMinites { get; set; } = 5;

  }
}
