using Bug.Common.FhirTools;

namespace Bug.Logic.Service
{
  public interface IFhirResourceIdSupport
  {
    string GetResourceId(FhirResource fhirResource);
    void SetResourceId(FhirResource fhirResource, string id);
  }
}