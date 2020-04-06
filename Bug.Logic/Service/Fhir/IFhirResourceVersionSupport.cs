using Bug.Common.FhirTools;

namespace Bug.Logic.Service.Fhir
{
  public interface IFhirResourceVersionSupport
  {
    string? GetVersion(FhirResource fhirResource);
    void SetVersion(FhirResource fhirResource, int versionId);
  }
}