using Bug.Common.FhirTools;

namespace Bug.Logic.Service
{
  public interface IFhirResourceVersionSupport
  {
    string? GetVersion(FhirResource fhirResource);
    void SetVersion(FhirResource fhirResource, int versionId);
  }
}