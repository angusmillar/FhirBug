using System;
using System.Collections.Generic;
using System.Text;
using Bug.Common.Enums;

namespace Bug.Common.ApplicationConfig
{

  public class ServiceBaseUrlConfig : IServiceBaseUrlConfi
  {
    private readonly IFhirServerConfig IFhirServerConfig;
    public ServiceBaseUrlConfig(IFhirServerConfig IFhirServerConfig)
    {
      this.IFhirServerConfig = IFhirServerConfig;
    }

    public Uri Url(FhirVersion fhirMajorVersion)
    {
      return ConstructFullUrl(fhirMajorVersion);
    }

    private Uri ConstructFullUrl(FhirVersion FhirMajorVersion)
    {
      if (FhirMajorVersion == Enums.FhirVersion.Stu3)
      {
        return new Uri($"{this.IFhirServerConfig.ServiceBaseUrl.AbsoluteUri.TrimEnd('/')}/{Constant.EndpointPath.Stu3Fhir}");
      }
      else if (FhirMajorVersion == Enums.FhirVersion.R4)
      {
        return new Uri($"{this.IFhirServerConfig.ServiceBaseUrl.AbsoluteUri.TrimEnd('/')}/{Constant.EndpointPath.R4Fhir}");
      }
      else
      {
        throw new Exceptions.FhirVersionFatalException(FhirMajorVersion);
      }
    }

  }
}