using Bug.Common.Enums;
using System;

namespace Bug.Common.ApplicationConfig
{
  public interface IFhirServerConfig
  {
    FhirFormatType DefaultFhirFormat { get; set; }
    Uri ServiceBaseUrl { get; set; }
    int CahceSlidingExpirationMinites { get; set; }
  }
}