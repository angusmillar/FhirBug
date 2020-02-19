using Bug.Common.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bug.Common.ApplicationConfig
{
  public class FhirServerConfig : IFhirServerConfig
  {
    public FhirFormatType DefaultFhirFormat { get; set; }
    public Uri ServiceBaseUrl { get; set; }
  }
}
