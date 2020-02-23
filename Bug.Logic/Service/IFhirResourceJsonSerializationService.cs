using Bug.Common.FhirTools;

namespace Bug.Logic.Service
{
  public interface IFhirResourceJsonSerializationService
  {
    byte[] SerializeToJsonBytes(FhirResource fhirResource);
  }
}