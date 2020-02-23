using Bug.Common.FhirTools;

namespace Bug.Stu3Fhir.ResourceSupport
{
  public interface IStu3FhirResourceVersionSupport
  {
    string? GetVersion(IFhirResourceStu3 fhirResource);
    void SetVersion(string versionId, IFhirResourceStu3 fhirResource);
  }
}