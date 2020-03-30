using Bug.Common.Enums;
using System;

namespace Bug.Common.ApplicationConfig
{
  public interface IServiceBaseUrlConfi
  {
    Uri Url(FhirVersion fhirMajorVersion);
  }
}