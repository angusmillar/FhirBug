namespace Bug.Common.FhirTools
{
  public interface IResourceVersionIdSupport
  {
    int AsInterger(string VersionNumber);
    string Decrement(string VersionNumber, int ValueToSubtract = 1);
    string FirstVersion();
    string Increment(string VersionNumber, int ValueToAdd = 1);
  }
}