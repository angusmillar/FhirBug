using Hl7.Fhir.Model;

namespace Bug.R4Fhir.Serialization
{
  public interface IR4SerializationToJson
  {
    string SerializeToJson(Resource resource, Bug.Common.Enums.SummaryType summaryType = Bug.Common.Enums.SummaryType.False);
  }
}