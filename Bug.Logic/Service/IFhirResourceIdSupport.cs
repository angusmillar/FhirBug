using Bug.Common.FhirTools;

namespace Bug.Logic.Service
{
  public interface IFhirResourceIdSupport
  {
    string GetFhirId(FhirResource fhirResource);
    void SetFhirId(FhirResource fhirResource, string id);
  }
}