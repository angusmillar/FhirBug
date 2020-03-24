using Bug.Common.Enums;

namespace Bug.Common.Interfaces
{
  public interface IFhirUriFactory
  {
    bool TryParse(string requestUri, FhirVersion fhirMajorVersion, out IFhirUri? fhirUri, out string errorMessage);
  }
}