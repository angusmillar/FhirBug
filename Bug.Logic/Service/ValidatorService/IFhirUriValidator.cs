using Bug.Common.Enums;
using Bug.Common.FhirTools;
using Bug.Logic.UriSupport;

namespace Bug.Logic.Service.ValidatorService
{
  public interface IFhirUriValidator
  {
    bool IsValid(string Uri, FhirMajorVersion fhirMajorVersion, out IFhirUri? fhirUri, out FhirResource? operationOutcome);
  }
}
