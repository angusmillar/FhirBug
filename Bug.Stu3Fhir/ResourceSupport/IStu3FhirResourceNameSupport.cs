using Bug.Common.FhirTools;

namespace Bug.Stu3Fhir.ResourceSupport
{
  public interface IStu3FhirResourceNameSupport
  {
    string GetName(IFhirResourceStu3 fhirResource);
  }
}