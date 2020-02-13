using Hl7.Fhir.Model;

namespace Bug.R4Fhir.Serialization
{
  public interface IR4SerializationToXml
  {
    string SerializeToXml(Resource resource, Bug.Common.Enums.SummaryType summaryType = Bug.Common.Enums.SummaryType.False);
  }
}