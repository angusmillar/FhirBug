using Bug.Common.Exceptions;
using Bug.Common.FhirTools;
using Bug.R4Fhir.ResourceSupport;
using Bug.Stu3Fhir.ResourceSupport;

namespace Bug.Logic.Service.Fhir
{
  public class KnownResource : IKnownResource
  {
    private readonly IStu3IsKnownResource IStu3IsKnownResource;
    private readonly IR4IsKnownResource IR4IsKnownResource;
    public KnownResource(IStu3IsKnownResource IStu3IsKnownResource, IR4IsKnownResource IR4IsKnownResource)
    {
      this.IStu3IsKnownResource = IStu3IsKnownResource;
      this.IR4IsKnownResource = IR4IsKnownResource;
    }

    public bool IsKnownResource(Bug.Common.Enums.FhirVersion fhirVersion, string resourceName)
    {
      switch (fhirVersion)
      {
        case Common.Enums.FhirVersion.Stu3:
          return IStu3IsKnownResource.IsKnownResource(resourceName);
        case Common.Enums.FhirVersion.R4:
          return IR4IsKnownResource.IsKnownResource(resourceName);
        default:
          throw new FhirVersionFatalException(fhirVersion);
      }
    }
  }
}
