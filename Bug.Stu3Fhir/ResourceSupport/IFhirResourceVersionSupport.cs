namespace Bug.Stu3Fhir.ResourceSupport
{
  public interface IFhirResourceVersionSupport
  {
    string GetVersion(object resource);
    void SetVersion(string versionId, object resource);
  }
}