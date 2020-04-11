using Bug.Common.FhirTools;

namespace Bug.Logic.Service.Fhir
{
  public interface IFhirResourceJsonSerializationService
  {
    byte[] SerializeToJsonBytes(FhirResource fhirResource);
    byte[] SerializeToJsonBytes(Common.FhirTools.FhirContainedResource FhirContainedResource);
  }
}