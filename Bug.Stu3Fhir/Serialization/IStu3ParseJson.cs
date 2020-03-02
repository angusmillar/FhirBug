using Hl7.Fhir.Model;

namespace Bug.Stu3Fhir.Serialization
{
  public interface IStu3ParseJson
  {
    Resource ParseJson(string jsonResource);
  }
}