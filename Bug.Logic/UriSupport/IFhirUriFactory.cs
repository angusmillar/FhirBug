using Bug.Common.Enums;

namespace Bug.Logic.UriSupport
{
  public interface IFhirUriFactory
  {
    bool TryParse(string requestUri, FhirVersion fhirMajorVersion, out IFhirUri? fhirUri, out string errorMessage);
  }
}