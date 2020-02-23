using Bug.Common.FhirTools;

namespace Bug.R4Fhir.ResourceSupport
{
  public interface IR4FhirResourceVersionSupport
  {
    string? GetVersion(IFhirResourceR4 fhirResource);
    void SetVersion(string versionId, IFhirResourceR4 fhirResource);
  }
}