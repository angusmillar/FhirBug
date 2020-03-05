using Bug.Common.Enums;
using System;

namespace Bug.Common.ApplicationConfig
{
  public interface IServiceBaseUrl
  {
    Uri Url(FhirVersion fhirMajorVersion);
  }
}