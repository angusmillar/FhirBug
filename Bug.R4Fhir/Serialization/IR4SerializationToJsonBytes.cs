using Hl7.Fhir.Model;
using Bug.Common.Enums;

namespace Bug.R4Fhir.Serialization
{
  public interface IR4SerializationToJsonBytes
  {
    string SerializeToJson(Resource resource, SummaryType summaryType = SummaryType.False);
    byte[] SerializeToJsonBytes(object resource, SummaryType summaryType = SummaryType.False);
    string SerializeToXml(Resource resource, SummaryType summaryType = SummaryType.False);
  }
}