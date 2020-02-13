using Hl7.Fhir.Model;
//using Hl7.Fhir.Rest;

namespace Bug.Stu3Fhir.Serialization
{
  public interface IStu3SerializationToJson
  {
    string SerializeToJson(Resource resource, Bug.Common.Enums.SummaryType summaryType = Bug.Common.Enums.SummaryType.False);
  }
}