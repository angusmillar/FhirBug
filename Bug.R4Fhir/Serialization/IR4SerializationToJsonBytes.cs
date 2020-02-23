using Hl7.Fhir.Model;
using Bug.Common.Enums;
using Bug.Common.FhirTools;

namespace Bug.R4Fhir.Serialization
{
  public interface IR4SerializationToJsonBytes
  {
    string SerializeToJson(Resource resource, SummaryType summaryType = SummaryType.False);
    byte[] SerializeToJsonBytes(IFhirResourceR4 fhirResource, SummaryType summaryType = SummaryType.False);
    string SerializeToXml(Resource resource, SummaryType summaryType = SummaryType.False);
  }
}