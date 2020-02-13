namespace Bug.R4Fhir.ResourceSupport
{
  public interface IFhirResourceVersionSupport
  {
    string GetVersion(object resource);
    void SetVersion(string versionId, object resource);
  }
}