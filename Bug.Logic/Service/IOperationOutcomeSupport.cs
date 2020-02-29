using Bug.Common.Enums;
using Bug.Common.FhirTools;

namespace Bug.Logic.Service
{
  public interface IOperationOutcomeSupport
  {
    FhirResource GetError(FhirMajorVersion fhirMajorVersion, string[] errorMessages);
    FhirResource GetFatal(FhirMajorVersion fhirMajorVersion, string[] errorMessages);
    FhirResource GetInformation(FhirMajorVersion fhirMajorVersion, string[] errorMessages);
    FhirResource GetWarning(FhirMajorVersion fhirMajorVersion, string[] errorMessages);
  }
}