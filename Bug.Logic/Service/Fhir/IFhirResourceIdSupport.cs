using Bug.Common.FhirTools;

namespace Bug.Logic.Service.Fhir
{
  public interface IFhirResourceIdSupport
  {
    string? GetResourceId(FhirResource fhirResource);
    void SetResourceId(FhirResource fhirResource, string id);
  }
}