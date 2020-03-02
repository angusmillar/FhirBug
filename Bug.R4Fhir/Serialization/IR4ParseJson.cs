using Hl7.Fhir.Model;

namespace Bug.R4Fhir.Serialization
{
  public interface IR4ParseJson
  {
    Resource ParseJson(string jsonResource);
  }
}