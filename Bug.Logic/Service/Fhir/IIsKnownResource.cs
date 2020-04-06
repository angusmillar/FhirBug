using Bug.Common.FhirTools;

namespace Bug.Logic.Service.Fhir
{
  public interface IKnownResource
  {
    bool IsKnownResource(Bug.Common.Enums.FhirVersion fhirVersion, string resourceName);
  }
}