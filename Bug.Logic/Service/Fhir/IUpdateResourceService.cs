using Bug.Common.FhirTools;

namespace Bug.Logic.Service.Fhir
{
  public interface IUpdateResourceService
  {
    FhirResource Process(UpdateResource UpdateResource);
  }
}